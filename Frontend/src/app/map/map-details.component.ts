import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

declare let $: any;

@Component({
    selector: 'app-map',
    templateUrl: './map-details.component.html',
    styleUrls: ['./map.component.scss']
})

export class MapDetailsComponent implements OnInit, OnDestroy {

    id: number;
    error = false;
    map = {
        author: {},
        name: '',
        images: 0
    };
    mapImages: Array<number>;
    notFound = false;

    private ngUnsubscribe = new Subject();

    constructor(private dataService: DataService, private route: ActivatedRoute, private titleService: Title) { }

    ngOnInit() {
        this.getID();
        this.getMap();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getID() {
        this.route.params.pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(params => this.id = params.id);
    }

    getMap() {
        this.dataService.get('/maps/' + this.id)
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(map => {
                this.map = map;
                if (this.map.images > 0) {
                    this.mapImages = Array.apply(0, Array(this.map.images)).map((x, i) => i);
                }
                this.titleService.setTitle(this.map.name + ' - Maps - Intruder DB');
                setTimeout(() => $('.slider').slider({ height: 700, interval: 5000}), 50);
                $('ul.tabs').tabs();
            }, err => {
                let errorResponse = err.error;

                if (errorResponse && errorResponse.code === 404) {
                    this.notFound = errorResponse;
                } else {
                    if (errorResponse === null) {
                        errorResponse = {
                            code: 500,
                            message: 'Unable to retrieve map'
                        };
                    }

                    this.error = errorResponse;
                }

                this.titleService.setTitle(errorResponse.code + ' - Maps - Intruder DB');
            });
    }
}
