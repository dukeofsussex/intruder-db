import { Component, Input, OnChanges } from '@angular/core';

@Component({
    selector: 'role',
    templateUrl: './role.component.html',
    styleUrls: [ './role.component.scss' ]
})
export class RoleComponent implements OnChanges {
    @Input() name: string;
    @Input() role: string;
    @Input() showBadge = true;
    @Input() showName = true;

    roleColour: string;

    constructor() { }

    ngOnChanges(changes: any) {
        this.setRoleColour();
    }

    setRoleColour() {
        switch (this.role) {
            case 'Dev':
                this.roleColour = 'deep-orange';
                break;
            case 'AUG':
                this.roleColour = 'blue';
                break;
            case 'Demoted':
                this.roleColour = 'grey';
                break;
            default:
                this.roleColour = '';
        }
    }

    getNameColour() {
        return (this.roleColour !== '') ? this.roleColour + '-text' : '';
    }

    showRoleBadge() {
        return this.showBadge && this.role !== 'Agent';
    }
}
