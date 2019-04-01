import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AuthService } from '../auth.service';
import { DataService } from '../data.service';

declare var $: any;

@Component({
    selector: 'app-authenticate',
    templateUrl: './authenticate.component.html',
    styleUrls: ['./auth.component.scss']
})
export class AuthenticateComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    error: any;

    constructor(private authService: AuthService,
        private dataService: DataService,
        private route: ActivatedRoute,
        private router: Router,
        private titleService: Title
    ) { }

    ngOnInit() {
        this.titleService.setTitle('Authenticating - Intruder DB');
        this.route.queryParamMap.pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(params => {
            this.completeAuthentication(params);
        });
    }

    completeAuthentication(params: any) {
        if (!params.has('openid.claimed_id')
            || !params.has('openid.identity')
            || !params.has('openid.response_nonce')
            || !params.has('openid.assoc_handle')
            || !params.has('openid.return_to')
            || !params.has('openid.signed')
            || !params.has('openid.sig')) {
            alert('SOMETHING IS MISSING');
            return;
        }

        this.dataService
            .get('/authenticate', {
                claimedId: params.get('openid.claimed_id'),
                identity: params.get('openid.identity'),
                responseNonce: params.get('openid.response_nonce'),
                assocHandle: params.get('openid.assoc_handle'),
                returnTo: params.get('openid.return_to'),
                signed: params.get('openid.signed'),
                sig: params.get('openid.sig')
            })
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(details => {
                details.profile.token = details.token;
                this.authService.setUser(details.profile);
                if (details.profile.id > 0) {
                    setTimeout(() => {
                        $('.dropdown-button').dropdown({
                            constrainWidth: false,
                            hover: false,
                            belowOrigin: true
                        });
                    }, 100);
                    return setTimeout(() => this.router.navigate(['dashboard']), 2500);
                }
                this.router.navigate(['claim']);
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'An error occurred while trying to sign you in'
                    };
                }

                this.error = errorResponse;
            });
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
}
