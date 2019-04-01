import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { TuningRoutingModule } from './tuning-routing.module';
import { TuningListComponent } from './tuning-list.component';
import { TuningDetailsComponent } from './tuning-details.component';
import { TuningTableComponent } from './tuning-table.component';

import { TuningService } from './tuning.service';

@NgModule({
    imports: [
        CommonModule,
        ErrorModule,
        FormsModule,
        ReactiveFormsModule,
        SharedModule,
        TuningRoutingModule
    ],
    providers: [TuningService],
    declarations: [TuningListComponent, TuningDetailsComponent, TuningTableComponent]
})

export class TuningModule { }
