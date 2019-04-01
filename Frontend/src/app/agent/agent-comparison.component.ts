import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { empty, from, Observable, Subject } from 'rxjs';
import { catchError, mergeMap, takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

declare let Materialize: any;

@Component({
    selector: 'app-agents',
    templateUrl: './agent-comparison.component.html',
    styleUrls: ['./agent.component.scss']
})

export class AgentComparisonComponent implements OnInit, OnDestroy {

    agents = [];
    agentIDs: number[];
    agentSearchQuery: string;
    comparedAgent = {
        id: -1,
        index: -1
    };
    maxAgents = 20;
    updatingAgents = {};

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private route: ActivatedRoute, private router: Router, private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle('Compare Agents - Intruder DB');
        this.setup();
        this.getAgents();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    setup() {
        this.route.queryParamMap.pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(params => {
                const agentIDs = params.has('agents') ? params.get('agents').split(',') : [];

                this.agentIDs = agentIDs.map((value, index) => {
                        return parseInt(value, 10);
                    })
                    .filter((element, index, arr) => arr.indexOf(element) === index); // Remove duplicates

                if (agentIDs.length > this.maxAgents) {
                    this.agentIDs = this.agentIDs.splice(this.maxAgents - 1, agentIDs.length - this.maxAgents); // Set the maximum count
                    Materialize.toast('Removed excessive agents, max agents is ' + this.maxAgents, 3000, 'red darken-2');
                }

                this.comparedAgent.id = params.has('to') ? parseInt(params.get('to'), 10) : -1;
                this.comparedAgent.index = this.agentIDs.indexOf(this.comparedAgent.id);

                if (this.comparedAgent.id !== -1 && this.comparedAgent.index === -1) {
                    this.comparedAgent.id = -1;
                    Materialize.toast('Compared agent not in comparison list', 3000, 'red darken-2');
                }

                this.updateRoute();
            });
    }

    getAgent(id: number, index: number, addingAgent: boolean) {
        this.dataService.get('/agents/' + id, addingAgent ? null : { update: true })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(agent => {
                if (index === -1) {
                    this.agents.push(agent);
                } else {
                    this.agents.splice(index, 1, agent);
                }

                if (!this.agentIDs.includes(agent.id)) {
                    this.agentIDs.push(agent.id);
                }

                this.updateRoute();

                if (addingAgent) {
                    Materialize.toast('Added ' + agent.name, 3000, 'green darken-2');
                } else {
                    Materialize.toast('Updated ' + agent.name, 3000, 'orange darken-2');
                    this.updatingAgents[id] = false;
                }
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'Unable to retrieve agent'
                    };
                }

                Materialize.toast(errorResponse.message, 3000, 'red darken-2');
            });
    }

    getAgents() {
        from(this.agentIDs).pipe(
            mergeMap((id, index) => {
                return this.dataService.get('/agents/' + id)
                    .pipe(
                        catchError(err => {
                            let errorResponse = err.error;

                            if (errorResponse === null) {
                                errorResponse = {
                                    message: 'Unable to retrieve agent'
                                };
                            }

                            Materialize.toast(errorResponse.message, 3000, 'red darken-2');

                            if (errorResponse && errorResponse.code === 404) {
                                this.agentIDs.splice(index, 1);
                                this.updateRoute();
                            }

                            return empty();
                        }));
            }),
            takeUntil(this.ngUnsubscribe)
        ).subscribe((agents: any) => {
            this.agents.push(agents);
            this.agents.sort((a, b) => {
                const aIndex = this.agentIDs.findIndex(id => id === a.id);
                const bIndex = this.agentIDs.findIndex(id => id === b.id);
                return aIndex - bIndex;
            });
        });
    }

    canUpdate(index: number) {
        if (!this.agents[index] || !this.agents[index].lastUpdate) {
            return;
        }

        const then = new Date(this.agents[index].lastUpdate);
        const now = new Date();
        const minutes = Math.floor((now.valueOf() - then.valueOf()) / (60 * 1000));

        return minutes >= 5 && !this.updatingAgents[this.agents[index].id];
    }

    compareToAgent(index: number) {
        if (this.comparedAgent.index === index) {
            this.comparedAgent.id = -1;
            this.comparedAgent.index = -1;
        } else {
            this.comparedAgent.id = this.agentIDs[index];
            this.comparedAgent.index = index;
        }
        this.updateRoute();
    }

    onSelect(suggestion: any) {
        if (this.agentIDs.includes(suggestion.id)) {
            Materialize.toast('This agent has already been added', 3000, 'red darken-2');
            return;
        }

        this.agentSearchQuery = '';
        this.getAgent(suggestion.id, -1, true);
    }

    removeAgent(index: number) {
        this.agents.splice(index, 1);
        this.agentIDs.splice(index, 1);

        if (this.comparedAgent.index === index) {
            this.comparedAgent.id = -1;
            this.comparedAgent.index = -1;
        }

        this.updateRoute();
    }

    updateAgent(index: number) {
        this.updatingAgents[this.agents[index].id] = true;
        this.getAgent(this.agents[index].id, index, false);
    }

    updateRoute() {
        this.router.navigate(['/compare'], {
            queryParams:
                (this.agentIDs.length > 0)
                ?
                {
                    agents: this.agentIDs.join(','),
                    to: this.comparedAgent.id !== -1 ? this.comparedAgent.id : null
                }
                :
                {}
        });
    }

}
