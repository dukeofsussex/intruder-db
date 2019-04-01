import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LegalComponent } from './legal.component';

const routes: Routes = [
    {
        path: 'legal',
        children: [
            {
                path: 'disclaimer',
                component: LegalComponent
            },
            {
                path: 'privacy-policy',
                component: LegalComponent
            },
            {
                path: 'terms-of-service',
                component: LegalComponent
            }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class LegalRoutingModule { }
