import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

@Component({
    selector: 'app-server',
    templateUrl: './server-details.component.html',
    styleUrls: ['./server.component.scss']
})

export class ServerDetailsComponent implements OnInit, OnDestroy {

    id: number;
    error = false;
    server: { name: string, map: { name: string } };
    notFound = false;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private route: ActivatedRoute, private titleService: Title) { }

    ngOnInit() {
        this.getID();
        this.getServer();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    currentMap(map: string) {
        return (map === this.server.map.name) ? 'active' : '';
    }

    getID() {
        this.route.params.pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(params => this.id = params.id);
    }

    getServer() {
        this.dataService.get('/servers/' + this.id)
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(server => {
                this.server = server;
                this.titleService.setTitle(this.server.name + ' - Servers - Intruder DB');
            }, err => {
                let errorResponse = err.error;

                if (errorResponse && errorResponse.code === 404) {
                    this.notFound = errorResponse;
                } else {
                    if (errorResponse === null) {
                        errorResponse = {
                            code: 500,
                            message: 'Unable to retrieve server'
                        };
                    }

                    this.error = errorResponse;
                }

                this.titleService.setTitle(errorResponse.code + ' - Servers - Intruder DB');
            });
    }
}

