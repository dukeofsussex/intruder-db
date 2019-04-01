import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { forkJoin, Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AuthService } from '../auth.service';
import { DataService } from '../data.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {

    agent: any;
    badges = [];
    error = false;
    friends = [];
    maps = [];
    nextBadge: any;
    tunings = [];

    private ngUnsubscribe = new Subject();

    constructor(private authService: AuthService, private dataService: DataService, private titleService: Title) { }

    ngOnInit(): void {
        this.titleService.setTitle('Dashboard - Intruder DB');
        this.getDashboardData();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getDashboardData() {
        forkJoin(
            this.dataService.get('/agents/' + this.authService.getUser().id),
            this.dataService.get('/agents/' + this.authService.getUser().id + '/maps'),
            this.dataService.get('/agents/' + this.authService.getUser().id + '/badges'),
            this.dataService.get('/agents/friends', null, true),
            this.dataService.get('/tunings', null, true)
        ).pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe((data: any) => {
            this.agent = data[0];
            this.maps = data[1];
            this.badges = data[2];
            this.friends = data[3];
            this.tunings = data[4];
            this.calculateNextBadge();
        }, err => {
            let errorResponse = err.error;

            if (errorResponse === null) {
                errorResponse = {
                    message: 'Unable to retrieve all agent data'
                };
            }

            this.error = errorResponse;
        });
    }

    calculateNextBadge() {
        const badges = this.badges.sort((a, b) => {
            if (a.progress !== b.progress) {
                return a.progress - b.progress;
            }

            return a.id - b.id;
        });

        this.nextBadge = badges[0];
    }
}
