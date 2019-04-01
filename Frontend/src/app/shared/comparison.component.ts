import { Component, Input, OnChanges } from '@angular/core';

declare let $: any;

@Component({
    selector: 'comparison',
    templateUrl: './comparison.component.html',
    styleUrls: [ './comparison.component.scss' ]
})
export class ComparisonComponent implements OnChanges {
    @Input() a: number;
    @Input() b: number;
    @Input() customClass: string;
    @Input() customTooltip: string;

    colour: string;
    icon: string;
    rotation: string;

    constructor() { }

    ngOnChanges(changes: any) {
        if (this.a === null || this.b === null) {
            return;
        }

        this.compare();
        setTimeout(() => $('.tooltipped').tooltip(), 100);
    }

    compare() {
        if (this.a === this.b) {
            this.rotation = '';
            this.colour = 'grey-text';
            this.icon = 'remove';
        } else if (this.a > this.b) {
            this.colour = 'green-text';
            this.rotation = 'rotate(-90deg)';

            if (this.a > (1.5 * this.b)) {
                this.icon = 'fast_forward';
            } else {
                this.icon = 'play_arrow';
            }
        } else {
            this.colour = 'red-text';
            this.rotation = 'rotate(90deg)';

            if ((1.5 * this.a) < this.b) {
                this.icon = 'fast_forward';
            } else {
                this.icon = 'play_arrow';
            }
        }
    }

    getTooltip() {
        return this.customTooltip || this.b;
    }
}
