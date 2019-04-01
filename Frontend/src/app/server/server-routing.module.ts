import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ServerListComponent } from './server-list.component';
import { ServerDetailsComponent } from './server-details.component';

const routes: Routes = [
    {
        path: 'servers',
        children: [
            {
                path: '',
                component: ServerListComponent
            },
            {
                path: ':id',
                component: ServerDetailsComponent
            }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServerRoutingModule { }
