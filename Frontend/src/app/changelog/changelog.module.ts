import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ChangelogRoutingModule } from './changelog-routing.module';
import { ChangelogComponent } from './changelog.component';

@NgModule({
    imports: [
        CommonModule,
        ChangelogRoutingModule
    ],
    declarations: [ChangelogComponent]
})
export class ChangelogModule { }
