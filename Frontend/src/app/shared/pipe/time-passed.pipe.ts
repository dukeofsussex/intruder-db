import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'timePassed'
})
export class TimePassedPipe implements PipeTransform {

    constructor() { }

    transform(timestamp: string, args?: any): any {
        const then = new Date(timestamp);
        const now = new Date();
        const difference = now.valueOf() - then.valueOf();
        const seconds = Math.floor(difference / 1000);
        const minutes = Math.floor(difference / (60 * 1000));
        const hours = Math.floor(difference / (60 * 60 * 1000));
        const days = Math.floor(difference / (24 * 60 * 60 * 1000));
        const months = Math.floor(difference / (31 * 24 * 60 * 60 * 1000));

        if (months >= 12) {
            return 'Over a year ago';
        } else if (months !== 0 && months < 12) {
            return months + ' month' + (months === 1 ? '' : 's') + ' ago';
        } else if (days !== 0 && days < 31) {
            return days + 'd ago';
        } else if (hours !== 0 && hours < 24) {
            return hours + 'h ago';
        } else if (minutes !== 0 && minutes < 60) {
            return minutes + 'm ago';
        } else if (seconds !== 0 && seconds < 60) {
            return seconds + 's ago';
        }
    }
}

