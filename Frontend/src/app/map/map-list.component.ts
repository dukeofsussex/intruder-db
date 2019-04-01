import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

@Component({
    selector: 'app-map',
    templateUrl: './map-list.component.html',
    styleUrls: ['./map.component.scss']
})

export class MapListComponent implements OnInit, OnDestroy {

    private searchQueryChanged: Subject<string> = new Subject<string>();

    error: object;
    loading = false;
    maps = [];
    currentPage = 1;
    searchQuery = '';
    sorting = {
        column: 'name',
        order: 0
    };
    tableHeaders = [
        {
            column: 'name',
            name: 'Name',
            sortAsc: true,
            sortDesc: false
        },
        {
            column: 'author',
            name: 'Author',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'stage',
            name: 'Stage',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'played',
            name: 'Played',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'averageRating',
            name: 'Average Rating',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'lastUpdate',
            name: 'Last Update',
            sortAsc: false,
            sortDesc: false
        }
    ];
    totalPages: number;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle('Maps - Intruder DB');
        this.getMaps();
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
            this.getMaps();
        });
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getMaps() {
        this.loading = true;
        this.dataService
            .get('/maps', {
                page: this.currentPage,
                name: this.searchQuery,
                column: this.sorting.column,
                order: this.sorting.order
            })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(data => {
                this.maps = data.data;
                this.totalPages = data.totalPages;
                this.loading = false;
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'Unable to retrieve maps'
                    };
                }

                this.error = errorResponse;
            });
    }

    onChange(page: number): void {
        this.currentPage = page;
        this.getMaps();
    }

    onChangeSearchQuery(query: string) {
        this.searchQueryChanged.next(query);
    }

    onChangeSort(sorting: { column: string, order: number }) {
        this.sorting = sorting;
        this.getMaps();
    }
}
