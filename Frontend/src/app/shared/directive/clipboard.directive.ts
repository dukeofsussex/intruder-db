// Original: https://www.bennadel.com/blog/3235-creating-a-simple-copy-to-clipboard-directive-in-angular-2-4-9.htm
import { Directive, ElementRef, EventEmitter, HostListener, Inject, Injectable, Input, Output } from '@angular/core';
import { DOCUMENT } from '@angular/platform-browser';

@Directive({
    selector: '[clipboard]'
})
@Injectable()
export class ClipboardDirective {

    @Input() clipboard: string;

    @Output() onCopied: EventEmitter<boolean> = new EventEmitter<boolean>();

    @HostListener('click') onClick() {
        const result = this.copy();
        this.onCopied.emit(result);
    }

    constructor(@Inject(DOCUMENT) private dom: Document, private element: ElementRef) { }


    private copy() {

        const textarea = this.dom.createElement('textarea');

        textarea.style.height = '0px';
        textarea.style.left = '-100px';
        textarea.style.opacity = '0';
        textarea.style.position = 'fixed';
        textarea.style.top = '-100px';
        textarea.style.width = '0px';

        this.dom.body.appendChild(textarea);

        textarea.value = this.clipboard;
        textarea.select();

        this.dom.execCommand('copy');

        if (textarea && textarea.parentNode) {
            textarea.parentNode.removeChild(textarea);
        }

        return true;
    }
}
