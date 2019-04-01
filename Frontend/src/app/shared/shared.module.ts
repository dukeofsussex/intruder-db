import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ErrorModule } from '../error/error.module';

import { ActivityGraphComponent } from './graph/activity-graph.component';
import { AvatarDirective } from './directive/avatar.directive';
import { ClipboardDirective } from './directive/clipboard.directive';
import { ComparisonComponent } from './comparison.component';
import { ComparisonHighlightingDirective } from './directive/comparison-highlighting.directive';
import { FlagComponent } from './flag.component';
import { HtmlConvertPipe } from './pipe/html-convert.pipe';
import { MapGraphComponent } from './graph/map-graph.component';
import { PaginationComponent } from './pagination.component';
import { PrettySecondsPipe } from './pipe/pretty-seconds.pipe';
import { RatingComponent } from './rating.component';
import { RoleComponent } from './role.component';
import { TableSortingComponent } from './table-sorting.component';
import { TimePassedPipe } from './pipe/time-passed.pipe';
import { TypeAheadComponent } from './typeahead.component';

@NgModule({
    imports: [
        CommonModule,
        ErrorModule,
        RouterModule
    ],
    exports: [
        ActivityGraphComponent,
        AvatarDirective,
        ClipboardDirective,
        ComparisonComponent,
        ComparisonHighlightingDirective,
        FlagComponent,
        HtmlConvertPipe,
        MapGraphComponent,
        PaginationComponent,
        PrettySecondsPipe,
        RatingComponent,
        RoleComponent,
        TableSortingComponent,
        TimePassedPipe,
        TypeAheadComponent
    ],
    declarations: [
        ActivityGraphComponent,
        AvatarDirective,
        ClipboardDirective,
        ComparisonComponent,
        ComparisonHighlightingDirective,
        FlagComponent,
        HtmlConvertPipe,
        MapGraphComponent,
        PaginationComponent,
        PrettySecondsPipe,
        RatingComponent,
        RoleComponent,
        TableSortingComponent,
        TimePassedPipe,
        TypeAheadComponent
    ]
})

export class SharedModule { }
