import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { GuideComponent } from './guide.component';

const routes: Routes = [
    {
        path: 'guides',
        children: [
            {
                path: '',
                component: GuideComponent
            },
            {
                path: ':id',
                component: GuideComponent
            }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GuideRoutingModule { }
