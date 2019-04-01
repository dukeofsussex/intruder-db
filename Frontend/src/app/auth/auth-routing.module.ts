import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ClaimGuardService } from '../claim-guard.service';

import { AuthenticateComponent } from './authenticate.component';
import { ClaimComponent } from './claim.component';
import { ClaimConfirmationComponent } from './claim-confirmation.component';
import { LoginComponent } from './login.component';

const routes: Routes = [
    {
        path: 'authenticate',
        component: AuthenticateComponent
    },
    {
        path: 'claim',
        children: [
            {
                path: '',
                component: ClaimComponent,
                canActivate: [ClaimGuardService]
            },
            {
                path: 'confirm',
                component: ClaimConfirmationComponent
            }
        ]
    },
    {
        path: 'login',
        component: LoginComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AuthRoutingModule { }
