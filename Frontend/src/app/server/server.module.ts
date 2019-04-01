import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ErrorModule } from '../error/error.module';
import { SharedModule } from '../shared/shared.module';

import { ServerRoutingModule } from './server-routing.module';
import { ServerListComponent } from './server-list.component';
import { ServerDetailsComponent } from './server-details.component';

@NgModule({
    imports: [
        CommonModule,
        ErrorModule,
        FormsModule,
        ServerRoutingModule,
        SharedModule
    ],
    declarations: [ServerListComponent, ServerDetailsComponent]
})
export class ServerModule { }
