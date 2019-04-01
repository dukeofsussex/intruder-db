import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Subject } from 'rxjs';

import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

@Component({
    selector: 'app-agent',
    templateUrl: './agent-list.component.html',
    styleUrls: ['./agent.component.scss']
})

export class AgentListComponent implements OnInit, OnDestroy {

    private searchQueryChanged: Subject<string> = new Subject<string>();

    error: object;
    loading = false;
    agents = [];
    currentPage = 1;
    searchQuery = '';
    sorting = {
        column: 'name',
        order: 0
    };
    tableHeaders = [
        {
            name: '#'
        },
        {
            column: 'name',
            name: 'Name',
            sortAsc: true,
            sortDesc: false
        },
        {
            column: 'role',
            name: 'Role',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'xp',
            name: 'XP',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'xpPerMatch',
            name: 'XP per match',
            shortName: 'XP p.m.',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'timePlayed',
            name: 'Time',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'timePerMatch',
            name: 'Time per match',
            shortName: 'Time p.m.',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'matchesPlayed',
            name: 'Matches',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'matchesWon',
            name: 'Won',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'winRate',
            name: 'Winrate',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'matchesLost',
            name: 'Lost',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'matchesTied',
            name: 'Tied',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'matchesSurvived',
            name: 'Survived',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'survivalRate',
            name: 'Survivalrate',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'arrests',
            name: 'Arrests',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'arrestsPerMatch',
            name: 'Arrests per match',
            shortName: 'Arrests p.m.',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'captures',
            name: 'Captures',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'capturesPerMatch',
            name: 'Captures per match',
            shortName: 'Captures p.m.',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'lastSeen',
            name: 'Last seen',
            sortAsc: false,
            sortDesc: false
        }
    ];
    totalPages: number;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle('Agents - Intruder DB');
        this.getAgents();
        this.searchQueryChanged.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            takeUntil(this.ngUnsubscribe)
        ).subscribe(searchQuery => {
            this.searchQuery = searchQuery;
            if (this.searchQuery !== '' && this.searchQuery.trim().length === 0) {
                return;
            }
            this.currentPage = 1;
            this.getAgents();
        });
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getAgents() {
        this.loading = true;
        this.dataService
            .get('/agents', {
                page: this.currentPage,
                name: this.searchQuery,
                column: this.sorting.column,
                order: this.sorting.order
            })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(data => {
                this.agents = data.data;
                this.totalPages = data.totalPages;
                this.loading = false;
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'Unable to retrieve agents'
                    };
                }

                this.error = errorResponse;
            });
    }

    onChange(page: number): void {
        this.currentPage = page;
        this.getAgents();
    }

    onChangeSearchQuery(query: string) {
        this.searchQueryChanged.next(query);
    }

    onChangeSort(sorting: { column: string, order: number }) {
        this.sorting = sorting;
        this.getAgents();
    }
}

