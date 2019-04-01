import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ControlContainer, NgForm } from '@angular/forms';

@Component({
    selector: 'tuning-table',
    templateUrl: './tuning-table.component.html',
    styleUrls: ['./tuning.component.scss'],
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }],
    encapsulation: ViewEncapsulation.None
})
export class TuningTableComponent {
    @Input() name: string;
    @Input() settings: object[];

    constructor() { }

    getMultiple(group: object) {
        return Object.keys(group);
    }
}
