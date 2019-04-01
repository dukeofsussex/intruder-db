import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AuthService } from '../auth.service';
import { DataService } from '../data.service';

declare var $: any;
declare var Materialize: any;

@Component({
    selector: 'app-claim-confirmation',
    templateUrl: './claim-confirmation.component.html',
    styleUrls: ['./auth.component.scss']
})

export class ClaimConfirmationComponent implements OnInit, OnDestroy {

    claimProcessed = true;
    id: number;
    uid: string;

    private ngUnsubscribe = new Subject();

    error: any;

    constructor(private authService: AuthService,
        private dataService: DataService,
        private route: ActivatedRoute,
        private router: Router,
        private titleService: Title
    ) { }

    ngOnInit() {
        this.titleService.setTitle('Claim confirmation - Intruder DB');

        this.route.queryParamMap.pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(params => {
            this.id = params.has('id') ? parseInt(params.get('id'), 10) : 0;
            this.uid = params.has('uid') ? params.get('uid') : '';
        });

        this.confirmClaim();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    confirmClaim() {
        this.dataService
            .put('/agents/' + this.id + '/claim/' + this.uid, null)
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(details => {
                this.claimProcessed = true;
                return setTimeout(() => this.router.navigate(['login']), 5000);
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'An error occurred while confirming the claim'
                    };
                }

                this.error = errorResponse;
            });
    }
}
