import { Component, Input } from '@angular/core';

@Component({
    selector: 'flag',
    templateUrl: './flag.component.html'
})
export class FlagComponent {

    @Input()
    set country(country: string) {
        this._country = country;
        this.getFlagCode();
    }

    get country() {
        return this._country;
    }

    _country: string;
    flagCode: string;

    constructor() { }

    getFlagCode() {
        if (this._country === null) {
            return;
        }

        switch (this._country) {
            case 'As':
                this.flagCode = 'cn';
                break;
            case 'Ja':
                this.flagCode = 'jp';
                break;
            default:
                this.flagCode = this._country.toLowerCase();
        }
    }
}
