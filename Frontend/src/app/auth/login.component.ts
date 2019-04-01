import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { AuthService } from '../auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./auth.component.scss']
})
export class LoginComponent implements OnInit {

    authRoute: string;

    constructor(private authService: AuthService, private titleService: Title) { }

    ngOnInit(): void {
        this.titleService.setTitle('Sign in - Intruder DB');
        this.authRoute = this.authService.authenticationRoute;
    }
}
