import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ErrorComponent } from './error.component';
import { InfoComponent } from './info.component';
import { PageNotFoundComponent } from './page-not-found.component';

@NgModule({
    imports: [
        CommonModule
    ],
    exports: [ErrorComponent, InfoComponent, PageNotFoundComponent],
    declarations: [ErrorComponent, InfoComponent, PageNotFoundComponent]
})
export class ErrorModule { }
