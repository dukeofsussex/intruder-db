import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChangelogComponent } from './changelog.component';

const routes: Routes = [
    {
        path: 'changelog',
        children: [
            {
                path: '',
                component: ChangelogComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ChangelogRoutingModule { }
