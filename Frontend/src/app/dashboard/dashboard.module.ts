import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';

@NgModule({
    imports: [
        CommonModule,
        ErrorModule,
        DashboardRoutingModule,
        SharedModule
    ],
    declarations: [DashboardComponent]
})

export class DashboardModule { }
