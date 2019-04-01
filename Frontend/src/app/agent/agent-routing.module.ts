import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AgentListComponent } from './agent-list.component';
import { AgentDetailsComponent } from './agent-details.component';
import { AgentComparisonComponent } from './agent-comparison.component';
import { AgentImportComponent } from './agent-import.component';

const routes: Routes = [
    {
        path: 'agents',
        children: [
            {
                path: '',
                component: AgentListComponent
            },
            {
                path: ':id',
                component: AgentDetailsComponent
            }
        ]
    },
    {
        path: 'compare',
        component: AgentComparisonComponent
    },
    {
        path: 'import',
        component: AgentImportComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class AgentRoutingModule { }
