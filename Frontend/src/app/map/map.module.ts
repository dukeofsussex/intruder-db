import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { MapRoutingModule } from './map-routing.module';
import { MapListComponent } from './map-list.component';
import { MapDetailsComponent } from './map-details.component';

@NgModule({
    imports: [
        CommonModule,
        ErrorModule,
        FormsModule,
        MapRoutingModule,
        SharedModule
    ],
    declarations: [MapListComponent, MapDetailsComponent]
})
export class MapModule { }
