import { Component, EventEmitter, Input, Output, OnChanges, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AuthService } from '../auth.service';
import { DataService } from '../data.service';

declare const Materialize: any;

@Component({
    selector: 'rating',
    templateUrl: './rating.component.html',
    styleUrls: ['./rating.component.scss']
})
export class RatingComponent implements OnChanges, OnDestroy {

    @Input() allowDetails = true;
    @Input() allowInteraction = true;
    @Input() averageRating = 0;
    @Input() ratingBackend = '';
    @Input() ratings = [0, 0, 0, 0, 0];
    @Input() showVoteCount = true;

    @Output() onChange = new EventEmitter<number>();

    displayedRating = [];
    showDetails = false;
    ratingDetails = [];
    userRating = {};
    totalRatings = 0;

    private ngUnsubscribe = new Subject();

    constructor(private authService: AuthService, private dataService: DataService) { }

    ngOnChanges(changes: any) {
        if (this.averageRating === undefined) {
            return;
        }

        this.convertRating();

        this.totalRatings = this.ratings.reduce((a, b) => a + b);

        this.ratingDetails = this.ratings.reverse();

        if (this.authService.isLoggedIn()) {
            this.getUserRating();
        }
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    convertRating() {
        this.displayedRating = Array(Math.floor(this.averageRating)).fill(2);

        if (this.averageRating % Math.floor(this.averageRating) === 0.5) {
            this.displayedRating.push(1);
        }

        this.displayedRating = this.displayedRating.concat(Array(5 - Math.ceil(this.averageRating)).fill(0));
    }

    onClick(rating: number) {
        if (!this.allowInteraction || !this.authService.isLoggedIn()) {
            return;
        }

        if (this.userRating > 0 && rating === this.userRating) {
            this.dataService.delete(this.ratingBackend)
                .pipe(
                    takeUntil(this.ngUnsubscribe)
                ).subscribe(() => {
                    this.userRating = 0;
                    this.onChange.emit(rating);
                    Materialize.toast('Rating deleted', 2500, 'green darken-2');
                }, err => this.handleError(err, 'Unable to delete your rating'));
        } else if (this.userRating > 0) {
            this.dataService.put(this.ratingBackend, { value: rating })
                .pipe(
                    takeUntil(this.ngUnsubscribe)
                ).subscribe(userRating => {
                    this.userRating = userRating.value;
                    this.onChange.emit(userRating);
                    Materialize.toast('Rating updated', 2500, 'green darken-2');
                }, err => this.handleError(err, 'Unable to update your rating'));
        } else {
            this.dataService.post(this.ratingBackend, { value: rating }, true)
                .pipe(
                    takeUntil(this.ngUnsubscribe)
                ).subscribe(userRating => {
                    this.userRating = userRating.value;
                    this.onChange.emit(userRating);
                    Materialize.toast('Rating saved', 2500, 'green darken-2');
                }, err => this.handleError(err, 'Unable to save your rating'));
        }
    }

    onHover(index: number) {
        this.showDetails = true;

        if (!this.allowInteraction || !this.authService.isLoggedIn()) {
            return;
        }

        let hoverRating = Array(index).fill(2);
        hoverRating = hoverRating.concat(Array(5 - (index)).fill(0));

        this.displayedRating = hoverRating;
    }

    ratingTrackingFunction(index) {
        return index;
    }

    unselect() {
        this.showDetails = false;

        if (!this.allowInteraction || !this.authService.isLoggedIn()) {
            return;
        }

        this.convertRating();
    }

    private getUserRating() {
        this.dataService.get(this.ratingBackend, null, true)
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(rating => {
                this.userRating = rating.value;
            }, err => {
                // User hasn't rated the entity yet
                if (err.error !== null && err.error.code === 404) {
                    return;
                }

                this.handleError(err, 'Unable to retrieve your rating');
            });
    }

    private handleError(err: any, defaultMessage: string) {
        let errorResponse = err.error;

        if (errorResponse === null) {
            errorResponse = {
                message: defaultMessage
            };
        }

        Materialize.toast(errorResponse.message, 2500, 'red darken-2');
    }
}
