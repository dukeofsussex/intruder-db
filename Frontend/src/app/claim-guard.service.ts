import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

import { AuthService } from './auth.service';

@Injectable()
export class ClaimGuardService implements CanActivate {

    constructor(private authService: AuthService, private router: Router) { }

    canActivate(): boolean {
        if (this.authService.canClaimAgent()) {
            return true;
        }

        if (this.authService.isLoggedIn()) {
            this.router.navigate(['dashboard']);
        } else {
            this.router.navigate(['login']);
        }

        return false;
    }
}
