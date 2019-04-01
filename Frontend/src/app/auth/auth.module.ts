import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { AuthRoutingModule } from './auth-routing.module';
import { AuthenticateComponent } from './authenticate.component';
import { ClaimComponent } from './claim.component';
import { ClaimConfirmationComponent } from './claim-confirmation.component';
import { LoginComponent } from './login.component';

@NgModule({
    imports: [
        AuthRoutingModule,
        CommonModule,
        ErrorModule,
        FormsModule,
        SharedModule
    ],
    declarations: [AuthenticateComponent, ClaimComponent, ClaimConfirmationComponent, LoginComponent]
})
export class AuthModule { }
