import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { forkJoin, Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

@Component({
    selector: 'app-stats',
    templateUrl: './stats.component.html',
    styleUrls: ['./stats.component.scss']
})
export class StatsComponent implements OnInit, OnDestroy {

    error = false;
    stats: any;
    top = {
        maps: null,
        agents: null
    };

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private titleService: Title) {}

    ngOnInit(): void {
        this.titleService.setTitle('Global Stats - Intruder DB');
        this.getStats();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getStats(): void {
        forkJoin(
            this.dataService.get('/stats'),
            this.dataService.get('/stats/top/maps'),
            this.dataService.get('/stats/top/agents')
        ).pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(data => {
            this.stats = data[0];
            this.top.maps = data[1];
            this.top.agents = data[2];
        }, err => {
            let errorResponse = err.error;

            if (errorResponse === null) {
                errorResponse = {
                    message: 'Unable to retrieve all stats data'
                };
            }

            this.error = errorResponse;
        });
    }
}
