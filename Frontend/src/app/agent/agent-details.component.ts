import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { forkJoin, Observable, Subject } from 'rxjs';
import { filter, switchMap, takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

declare let $: any;
declare let Materialize: any;

@Component({
    selector: 'app-agent',
    templateUrl: './agent-details.component.html',
    styleUrls: ['./agent.component.scss']
})

export class AgentDetailsComponent implements OnInit, OnDestroy {

    agent: any;
    averages: any;
    completedBadges: any;
    incompleteBadges: any;
    id: number;
    error = false;
    loading = false;
    maps: any;
    notFound = false;
    updating = false;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private route: ActivatedRoute, private router: Router, private titleService: Title) { }

    ngOnInit() {
        this.router.events.pipe(
            takeUntil(this.ngUnsubscribe),
            filter((event) => event instanceof NavigationEnd)
        ).subscribe((event: NavigationEnd) => {
            this.getAgent();
        });

        this.getID();
        this.getAgent();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    canUpdate() {
        if (!this.agent) {
            return;
        }

        const then = new Date(this.agent.lastUpdate);
        const now = new Date();
        const minutes = Math.floor((now.valueOf() - then.valueOf()) / (60 * 1000));

        return minutes >= 5;
    }

    getID() {
        this.route.params.pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(params => this.id = params.id);
    }

    getAgent(showToastOnUpdate: boolean = false) {
        this.loading = true;

        forkJoin(
            this.dataService.get('/agents/' + this.id, { update: this.updating }),
            this.dataService.get('/agents/' + this.id + '/maps'),
            this.dataService.get('/stats/averages')
        ).pipe(
            takeUntil(this.ngUnsubscribe),
            switchMap(data => {
                this.agent = data[0];
                this.maps = data[1];
                this.averages = data[2];

                return this.dataService.get('/agents/' + this.id + '/badges');
            })
        ).subscribe((badges: any) => {
            if (showToastOnUpdate) {
                Materialize.toast('Update successful', 3000, 'green darken-2');
            }

            this.completedBadges = [];
            this.incompleteBadges = [];

            for (let i = 0; i < badges.length; i += 1) {
                if (badges[i].progress === 1) {
                    this.completedBadges.push(badges[i]);
                } else {
                    this.incompleteBadges.push(badges[i]);
                }
            }

            this.incompleteBadges.sort((a, b) => b.progress - a.progress);

            this.titleService.setTitle(this.agent.name + ' - Agents - Intruder DB');
            this.loading = false;
            this.updating = false;
            setTimeout(() => $('.tooltipped').tooltip(), 100);
        }, err => {
            let errorResponse = err.error;

            this.loading = false;
            this.updating = false;

            if (errorResponse && errorResponse.code === 404) {
                this.notFound = errorResponse;
            } else {
                if (errorResponse === null) {
                    errorResponse = {
                        code: 500,
                        message: 'Unable to retrieve all agent data'
                    };
                }

                if (showToastOnUpdate) {
                    return Materialize.toast(errorResponse.message, 3000, 'red darken-2');
                }

                this.error = errorResponse;
            }

            this.titleService.setTitle(errorResponse.code + ' - Agents - Intruder DB');
        });
    }

    updateAgent() {
        this.updating = true;
        this.getAgent(true);
    }

}
