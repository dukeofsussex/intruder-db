import { Directive, ElementRef, Input, OnChanges } from '@angular/core';

@Directive({
    selector: '[comparison-highlighting]'
})
export class ComparisonHighlightingDirective implements OnChanges {
    @Input() a: number;
    @Input() b: number;

    constructor(private el: ElementRef) {}

    ngOnChanges(changes: any): void {
        if (this.a === null || this.b === null) {
            this.el.nativeElement.style.backgroundColor = 'transparent';
            return;
        }

        this.setHighlighting();
    }

    setHighlighting() {
        let colour = '';

        if (this.a > this.b) {
            if (this.a > (1.5 * this.b)) {
                colour = '#a5d6a7';
            } else {
                colour = '#c8e6c9';
            }
        } else if (this.a < this.b) {
            if ((1.5 * this.a) < this.b) {
                colour = '#ef9a9a';
            } else {
                colour = '#ffcdd2';
            }
        }

        this.el.nativeElement.style.backgroundColor = colour;
    }
}
