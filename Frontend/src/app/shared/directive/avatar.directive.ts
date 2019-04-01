import { Directive, ElementRef, Input, OnChanges } from '@angular/core';

@Directive({
    selector: '[avatar]'
})
export class AvatarDirective implements OnChanges {
    @Input() avatar: string;

    constructor(private el: ElementRef) {}

    ngOnChanges(changes: any): void {
        if (this.avatar === null) {
            return;
        }

        this.setAvatarUrl();
    }

    setAvatarUrl() {
        this.el.nativeElement.src = this.avatar;
    }
}
