import { Component, OnInit, OnDestroy } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AuthService } from '../auth.service';
import { DataService } from '../data.service';

declare var $: any;

@Component({
    selector: 'app-claim',
    templateUrl: './claim.component.html',
    styleUrls: ['./auth.component.scss']
})
export class ClaimComponent implements OnInit, OnDestroy {

    claiming = false;
    claimProcessed = false;
    selectedAgent = {
        id: 0,
        name: '',
        avatarURL: ''
    };

    private ngUnsubscribe = new Subject();

    error: any;

    constructor(private authService: AuthService,
        private dataService: DataService,
        private router: Router,
        private titleService: Title
    ) { }

    ngOnInit() {
        this.titleService.setTitle('Claim agent - Intruder DB');
        this.selectedAgent = this.authService.getUser();
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    claim() {
        this.claiming = true;
        this.dataService
            .put('/agents/' + this.selectedAgent.id + '/claim', null)
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(details => {
                this.claiming = false;
                this.claimProcessed = true;
                this.authService.signOut();
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'An error occurred while claiming the agent'
                    };
                }

                this.error = errorResponse;
                this.claiming = false;
            });
    }

    onSelect(suggestion: any) {
        this.selectedAgent = suggestion;
    }
}
