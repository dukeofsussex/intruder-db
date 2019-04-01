import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Observable, Subject, timer } from 'rxjs';
import { map, take, takeUntil, timeInterval } from 'rxjs/operators';

import { DataService } from '../data.service';

declare let Materialize: any;

@Component({
    selector: 'app-agent',
    templateUrl: './agent-import.component.html',
    styleUrls: ['./agent.component.scss']
})

export class AgentImportComponent implements OnInit, OnDestroy {

    displayedLoadingMessages: Array<string>;
    error: any = false;
    loading = false;
    loadingMessages = [
        'Sneaking through the vents...',
        'Grabbing the package...',
        'Ragdolling to extraction...',
        'Examining the intel...'
    ];
    suggestions: object[];

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle('Import Agents - Intruder DB');
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    onSubmit(form: any) {
        let queryString = form.query.trim();

        if (queryString.trim().length === 0) {
            return;
        } else if (form.query.indexOf('http') !== -1) {
            if (form.query.indexOf('intruder') === -1 || form.query.indexOf('profile') === -1) {
                this.error = { message: 'Invalid profile url' };
                return;
            }
            queryString = this.extractUsername(form.query);
        }
        this.searchAgent(queryString);
    }

    searchAgent(queryString: string) {
        this.displayedLoadingMessages = [];
        this.loading = true;
        this.error = false;
        this.displayLoadingMessages();

        this.dataService.get('/agents/search', { q: queryString, deep: true })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(suggestions => {
                this.suggestions = suggestions;
                this.error = false;
                this.loading = false;
                this.updateLabels();

            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'Unable to import agent'
                    };
                }

                this.error = errorResponse;

                this.loading = false;
                this.updateLabels();
            });
    }

    private displayLoadingMessages() {
        timer(0, 1250).pipe(
            timeInterval(),
            take(this.loadingMessages.length),
            map(x => x.value),
            takeUntil(this.ngUnsubscribe)
        ).subscribe((i) => this.displayedLoadingMessages.push(this.loadingMessages[i]));
    }

    private updateLabels() {
        return timer(100).pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(i => {
            Materialize.updateTextFields();
        });
    }

    private extractUsername(profileUrl: string): string {
        return profileUrl.substring(profileUrl.lastIndexOf('=') + 1);
    }
}
