import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

@Component({
    selector: 'app-tuning',
    templateUrl: './tuning-list.component.html',
    styleUrls: ['./tuning.component.scss']
})

export class TuningListComponent implements OnInit, OnDestroy {

    private searchQueryChanged: Subject<string> = new Subject<string>();

    error: object;
    loading = false;
    tunings = [];
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
            column: 'description',
            name: 'Description',
            sortAsc: false,
            sortDesc: false
        },
        {
            column: 'author',
            name: 'Author',
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
        this.titleService.setTitle('Shared tuning settings - Intruder DB');
        this.getSharedTuningSettings();
        this.searchQueryChanged.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            takeUntil(this.ngUnsubscribe)
        ).subscribe(searchQuery => {
            this.searchQuery = searchQuery.toString();
            if (this.searchQuery !== '' && this.searchQuery.trim().length === 0) {
                return;
            }
            this.currentPage = 1;
            this.getSharedTuningSettings();
        });
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getSharedTuningSettings() {
        this.loading = true;
        this.dataService
            .get('/tunings/shared', {
                page: this.currentPage,
                name: this.searchQuery,
                column: this.sorting.column,
                order: this.sorting.order
            })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(data => {
                this.tunings = data.data;
                this.totalPages = data.totalPages;
                this.loading = false;
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'Unable to retrieve tuning settings'
                    };
                }

                this.error = errorResponse;
            });
    }

    onChange(page: number): void {
        this.currentPage = page;
        this.getSharedTuningSettings();
    }

    onChangeSearchQuery(query: string) {
        this.searchQueryChanged.next(query);
    }

    onChangeSort(sorting: { column: string, order: number }) {
        this.sorting = sorting;
        this.getSharedTuningSettings();
    }
}
