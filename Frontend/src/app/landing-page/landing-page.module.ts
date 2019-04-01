import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { LandingPageRoutingModule } from './landing-page-routing.module';
import { LandingPageComponent } from './landing-page.component';

@NgModule({
    imports: [
        CommonModule,
        ErrorModule,
        LandingPageRoutingModule,
        SharedModule
    ],
    declarations: [LandingPageComponent]
})

export class LandingPageModule { }
