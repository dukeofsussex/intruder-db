import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

import { environment } from '../environments/environment';

declare let Materialize: any;

@Injectable()
export class AuthService {

    private storageIdentifier = 'idb_user';
    private defaultUser = {
        id: 0,
        token: '',
        name: '',
        role: 'None',
        avatarURL: ''
    };
    private jwtHelperService: JwtHelperService;
    private user: any;

    constructor() {
        this.jwtHelperService = new JwtHelperService();
        this.user = JSON.parse(localStorage.getItem(this.storageIdentifier)) || this.defaultUser;
    }

    authenticationRoute = 'https://steamcommunity.com/openid/login?'
        + 'openid.ns=http://specs.openid.net/auth/2.0'
        + '&openid.response_type=code'
        + '&openid.mode=checkid_setup'
        + '&openid.realm=' + environment.frontendURL
        + '&openid.claimed_id=http://specs.openid.net/auth/2.0/identifier_select'
        + '&openid.identity=http://specs.openid.net/auth/2.0/identifier_select'
        + '&openid.return_to=' + environment.frontendURL + '/authenticate';

    canClaimAgent(): boolean {
        return this.user.id === 0 && this.user.token.length > 0;
    }

    isLoggedIn(): boolean {
        if (!this.user.token) {
            return false;
        }

        if (this.jwtHelperService.isTokenExpired(this.user.token)) {
            Materialize.toast('Session expired', 3000, 'red darken-2');
            this.signOut();
            return false;
        }

        return this.user.id !== 0;
    }

    setUser(user): void {
        this.user = user;
        localStorage.setItem(this.storageIdentifier, JSON.stringify(this.user));
    }

    getUser(): any {
        return this.user;
    }

    getAuthorizationHeaderValue(): string {
        return `Bearer ${this.user.token}`;
    }

    signOut(): void {
        this.user = this.defaultUser;
        localStorage.removeItem(this.storageIdentifier);
    }
}
