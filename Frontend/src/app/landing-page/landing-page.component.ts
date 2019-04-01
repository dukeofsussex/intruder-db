import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { forkJoin, Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

@Component({
    selector: 'app-landing-page',
    templateUrl: './landing-page.component.html',
    styleUrls: ['./landing-page.component.scss']
})
export class LandingPageComponent implements OnInit, OnDestroy {

    currentlyPlaying: number;
    dayUnique: number;
    error = false;
    gameVersion: number;
    news: object[];

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle('Home - Intruder DB');
        this.getData();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getData() {
        forkJoin(
            this.dataService.get('/stats'),
            this.dataService.get('/sbg/news')
        ).pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe((data: any) => {
            this.currentlyPlaying = data[0].currentlyPlaying.value;
            this.dayUnique = data[0].dayUnique.value;
            this.gameVersion = data[0].gameVersion.value;
            this.news = data[1].blocks;
        }, err => {
            let errorResponse = err.error;

            if (errorResponse === null) {
                errorResponse = {
                    message: 'Unable to retrieve all landing page data'
                };
            }

            this.error = errorResponse;
        });
    }
}
