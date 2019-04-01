import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { AgentRoutingModule } from './agent-routing.module';
import { AgentListComponent } from './agent-list.component';
import { AgentDetailsComponent } from './agent-details.component';
import { AgentComparisonComponent } from './agent-comparison.component';
import { AgentImportComponent } from './agent-import.component';

@NgModule({
    imports: [
        AgentRoutingModule,
        CommonModule,
        ErrorModule,
        FormsModule,
        SharedModule
    ],
    declarations: [AgentListComponent, AgentDetailsComponent, AgentComparisonComponent, AgentImportComponent]
})
export class AgentModule { }
