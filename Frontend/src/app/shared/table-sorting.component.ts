import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: '[thead-sorting]',
    templateUrl: './table-sorting.component.html'
})
export class TableSortingComponent {

    @Input() sortingColumnIndex = 0;
    @Input() tableHeaders: object;

    @Output() onChange: EventEmitter<object> = new EventEmitter<object>();

    constructor() { }

    sort(index) {
        if (!this.tableHeaders[index].column) {
            return;
        }

        if (index === this.sortingColumnIndex) {
            if (this.tableHeaders[index].sortAsc) {
                this.tableHeaders[index].sortAsc = false;
                this.tableHeaders[index].sortDesc = true;
            } else {
                this.tableHeaders[index].sortAsc = true;
                this.tableHeaders[index].sortDesc = false;
            }
        } else {
            this.tableHeaders[this.sortingColumnIndex].sortAsc = false;
            this.tableHeaders[this.sortingColumnIndex].sortDesc = false;
            this.tableHeaders[index].sortAsc = true;
            this.tableHeaders[index].sortDesc = false;
            this.sortingColumnIndex = index;
        }

        this.onChange.emit({
            column: this.tableHeaders[index].column,
            order: this.tableHeaders[index].sortAsc ? 0 : 1
        });
    }
}
