import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LegalRoutingModule } from './legal-routing.module';
import { LegalComponent } from './legal.component';

@NgModule({
    imports: [
        CommonModule,
        LegalRoutingModule
    ],
    declarations: [LegalComponent]
})
export class LegalModule { }
