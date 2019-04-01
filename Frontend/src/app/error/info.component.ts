import { Component, Input } from '@angular/core';

@Component({
    selector: 'info',
    templateUrl: './info.component.html'
})
export class InfoComponent {

    @Input() info: any;

    constructor() { }
}
