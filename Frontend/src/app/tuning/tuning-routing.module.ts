import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TuningListComponent } from './tuning-list.component';
import { TuningDetailsComponent } from './tuning-details.component';

const routes: Routes = [
    {
        path: 'tunings',
        children: [
            {
                path: '',
                component: TuningListComponent
            },
            {
                path: ':id',
                component: TuningDetailsComponent
            }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class TuningRoutingModule { }
