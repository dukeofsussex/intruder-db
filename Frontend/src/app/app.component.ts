import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Angulartics2Piwik } from 'angulartics2/piwik';

import { AuthService } from './auth.service';

declare var $: any;

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

    searchVisible = false;

    public constructor(private authService: AuthService, private angulartics2Piwik: Angulartics2Piwik, private router: Router) { }

    ngOnInit() {
        $('.button-collapse').sideNav();
    }

    toggleSearch() {
        this.searchVisible = !this.searchVisible;
        if (!this.searchVisible) {
            setTimeout(() => {
                $('.dropdown-button').dropdown({
                    constrainWidth: false,
                    hover: false,
                    belowOrigin: true
                });
            }, 100);
        }
    }

    onSelect(suggestion: any) {
        this.router.navigate(['/agents', suggestion.id]);
        this.toggleSearch();
    }
}
