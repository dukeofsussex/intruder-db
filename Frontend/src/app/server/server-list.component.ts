import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

@Component({
    selector: 'app-server',
    templateUrl: './server-list.component.html',
    styleUrls: ['./server.component.scss']
})

export class ServerListComponent implements OnInit, OnDestroy {

    private searchQueryChanged: Subject<string> = new Subject<string>();

    error: object;
    loading = false;
    servers = [];
    currentPage = 1;
    searchQuery = '';
    sorting = {
        column: 'name',
        order: 0
    };
    tableHeaders = [
        {
            column: 'passworded',
            icon: 'lock',
            name: 'Passworded',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'ranked',
            name: 'Ranked',
            icon: 'keyboard_arrow_up',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'name',
            name: 'Name',
            sortAsc: true,
            sortDesc: false
        },
        {
            column: 'region',
            name: 'Region',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'type',
            name: 'Type',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'style',
            name: 'Style',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'map',
            name: 'Map',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'mapType',
            name: 'Map type',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'agents',
            name: 'Agents / Max agents',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'gamemode',
            name: 'Gamemode',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'timemode',
            name: 'Timemode',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'time',
            name: 'Time',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'inProgress',
            name: 'In progress',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'joinableBy',
            name: 'Joinable by',
            sortAsc: false,
            sortDesc: false
        }
    ];
    totalPages: number;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle('Servers - Intruder DB');
        this.getServers();
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
            this.getServers();
        });
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getServers() {
        this.loading = true;
        this.dataService
            .get('/servers', {
                page: this.currentPage,
                region: this.searchQuery,
                column: this.sorting.column,
                order: this.sorting.order
            })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(data => {
                this.servers = data.data;
                this.totalPages = data.totalPages;
                this.loading = false;
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'Unable to retrieve servers'
                    };
                }

                this.error = errorResponse;
            });
    }

    onChange(page: number): void {
        this.currentPage = page;
        this.getServers();
    }

    onChangeSearchQuery(query: string) {
        this.searchQueryChanged.next(query);
    }

    onChangeSort(sorting: { column: string, order: number }) {
        this.sorting = sorting;
        this.getServers();
    }
}
