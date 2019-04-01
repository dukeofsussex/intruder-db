import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { StatsRoutingModule } from './stats-routing.module';
import { StatsComponent } from './stats.component';

@NgModule({
    imports: [
        CommonModule,
        ErrorModule,
        SharedModule,
        StatsRoutingModule
    ],
    declarations: [StatsComponent]
})
export class StatsModule { }
