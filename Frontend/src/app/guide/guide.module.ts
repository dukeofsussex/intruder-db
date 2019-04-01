import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GuideRoutingModule } from './guide-routing.module';
import { GuideComponent } from './guide.component';


@NgModule({
    imports: [
        CommonModule,
        GuideRoutingModule
    ],
    declarations: [GuideComponent]
})

export class GuideModule { }
