import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'pagination',
    templateUrl: './pagination.component.html',
    styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent {

    private readonly itemsPerPage: number = 25;
    private readonly pagesToShow: number = 5;

    @Input() loading: boolean;
    @Input() currentPage: number;
    @Input() totalPages: number;

    @Output() onChange: EventEmitter<number> = new EventEmitter<number>();

    constructor() { }

    getMin(): number {
        return ((this.itemsPerPage * this.currentPage) - this.itemsPerPage) + 1;
    }

    getMax(): number {
        let max = this.itemsPerPage * this.currentPage;

        if (max > (this.totalPages * this.itemsPerPage)) {
            max = this.totalPages * this.itemsPerPage;
        }

        return max;
    }

    onPage(page: number): void {
        if (page === this.currentPage) {
            return;
        }

        this.onChange.emit(page);
    }

    onPrev(): void {
        if (this.currentPage === 1) {
            return;
        }

        this.onChange.emit(this.currentPage - 1);
    }

    onNext(): void {
        if (this.lastPage()) {
            return;
        }

        this.onChange.emit(this.currentPage + 1);
    }

    showPagination(): boolean {
        return this.totalPages > 1;
    }

    totalItems(): number {
        return this.itemsPerPage * this.totalPages;
    }

    lastPage(): boolean {
        return this.currentPage >= this.totalPages;
    }

    getPages(): number[] {
        const pages: number[] = [];

        pages.push(this.currentPage);

        for (let i = 0; i < this.pagesToShow - 1; i++) {
            if (pages.length < this.pagesToShow) {
                if (Math.min.apply(null, pages) > 1) {
                    pages.push(Math.min.apply(null, pages) - 1);
                }
                if (Math.max.apply(null, pages) < this.totalPages) {
                    pages.push(Math.max.apply(null, pages) + 1);
                }
            }
        }

        pages.sort((a, b) => a - b);

        return pages;
    }
}
