import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
    name: 'htmlConvert'
})
export class HtmlConvertPipe implements PipeTransform {

    private colorRegex: RegExp = /<color=([#A-F0-9]+)>/g;

    constructor(private sanitized: DomSanitizer) { }

    transform(value: string, args?: any): any {
        console.log(value);
        const occurrences = (value.match(/<color=/g) || []).length;
        for (let i = 0; i < occurrences; i++) {
            value = value.replace(this.colorRegex, '');
            // let temp = value.split("<color=");
            // temp[1] = temp[1].replace(">", "\">");
            // value = temp.join("<span style=\"color: ");
            value = value.replace('</color>', '');
        }
        console.log(value);
        return value;
    }
}
