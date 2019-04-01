import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'prettySeconds'
})
export class PrettySecondsPipe implements PipeTransform {

    constructor() { }

    transform(seconds: number, args?: any): any {
        const minutes = Math.floor(seconds / 60);
        const minutesRemaining = Math.floor((seconds % 3600) / 60);
        const hours = Math.floor(seconds / 3600);

        if (!seconds) {
            return '-';
        } else if (minutes === 0) {
            return '< 1m';
        } else if (hours === 0) {
            return minutes + 'm';
        }

        return hours + 'h' + (minutesRemaining !== 0 ? (':' + minutesRemaining + 'm') : '');
    }
}
