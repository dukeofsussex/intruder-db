import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MapListComponent } from './map-list.component';
import { MapDetailsComponent } from './map-details.component';

const routes: Routes = [
    {
        path: 'maps',
        children: [
            {
                path: '',
                component: MapListComponent
            },
            {
                path: ':id',
                component: MapDetailsComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class MapRoutingModule { }
