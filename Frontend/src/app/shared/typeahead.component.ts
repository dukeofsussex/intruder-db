// Original: github.com/orizens/echoes-player/blob/07c009b9784fd727042b880dffb99f293c4bf22f/src/app/youtube-videos/typeahead.directive.ts

import {
    ChangeDetectorRef,
    Component,
    ElementRef,
    EventEmitter,
    HostListener,
    Input,
    OnInit,
    OnDestroy,
    Output,
    ViewChild,
    ViewContainerRef
} from '@angular/core';
import { fromEvent, Observable, Subject, Subscription } from 'rxjs';
import { concat, debounceTime, distinctUntilChanged, filter, map, switchMap, takeUntil } from 'rxjs/operators';

import { DataService } from '../data.service';

declare let Materialize: any;

enum Key {
    Backspace = 8,
    Tab = 9,
    Enter = 13,
    Shift = 16,
    Escape = 27,
    ArrowLeft = 37,
    ArrowRight = 39,
    ArrowUp = 38,
    ArrowDown = 40
}

@Component({
    selector: '[typeahead]',
    templateUrl: './typeahead.component.html',
    styleUrls: ['./typeahead.component.scss']
})
export class TypeAheadComponent implements OnInit, OnDestroy {

    @Output() suggestionSelected = new EventEmitter<object>();

    private activeSuggestion: object;
    private showSuggestions = false;
    private subscriptions: Subscription[];
    private suggestionIndex = 0;
    private suggestions: object[] = [];

    private ngUnsubscribe = new Subject();

    @ViewChild('suggestionsTplRef') suggestionsTplRef;

    @HostListener('keydown', ['$event'])
    handleEsc(event: KeyboardEvent) {
        if (event.keyCode === Key.Escape) {
            this.hideSuggestions();
            event.preventDefault();
        }
    }

    constructor(private element: ElementRef,
        private viewContainer: ViewContainerRef,
        private cdr: ChangeDetectorRef,
        private dataService: DataService
    ) { }

    ngOnInit() {
        this.subscriptions = [
            this.filterEnterEvent(),
            this.listenAndSuggest(),
            this.navigateWithArrows()
        ];
        this.renderTemplate();
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
        this.subscriptions.length = 0;
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    renderTemplate() {
        this.viewContainer.createEmbeddedView(this.suggestionsTplRef);
    }

    filterEnterEvent() {
        return fromEvent(this.element.nativeElement, 'keydown').pipe(
            filter((e: KeyboardEvent) => e.keyCode === Key.Enter),
            takeUntil(this.ngUnsubscribe)
        ).subscribe((event: any) => {
            event.preventDefault();
            this.handleSelectSuggestion(this.activeSuggestion);
        });
    }

    listenAndSuggest() {
        return fromEvent(this.element.nativeElement, 'keyup').pipe(
            filter(this.validateKeyCode),
            map((event: any) => event.target.value),
            debounceTime(200),
            concat(),
            distinctUntilChanged(),
            filter((query: string) => {
                if (query.trim().length === 0) {
                    this.activeSuggestion = null;
                    this.hideSuggestions();
                }

                return query.trim().length > 0;
            }),
            map((query: string) => query.trim()),
            switchMap((query: string) => this.suggest(query)),
            takeUntil(this.ngUnsubscribe)
        ).subscribe((results: object[]) => {
            this.suggestions = results;
            this.showSuggestions = true;

            if (this.suggestions.length === 0) {
                this.activeSuggestion = null;
            }
        }, err => {
            let errorResponse = err.error;

            if (errorResponse === null) {
                errorResponse = {
                    message: 'Unable to retrieve agent'
                };
            }

            Materialize.toast(errorResponse.message, 3000, 'red darken-2');
            this.showSuggestions = false;
        });
    }

    navigateWithArrows() {
        return fromEvent(this.element.nativeElement, 'keydown').pipe(
            filter((e: any) => e.keyCode === Key.ArrowDown || e.keyCode === Key.ArrowUp),
            map((e: any) => {
                e.preventDefault();
                return e.keyCode;
            }),
            takeUntil(this.ngUnsubscribe)
        ).subscribe((keyCode: number) => {
            const step = keyCode === Key.ArrowDown ? 1 : -1;
            const topLimit = this.suggestions.length - 1;
            const bottomLimit = 0;
            this.suggestionIndex += step;
            if (this.suggestionIndex > topLimit) {
                this.suggestionIndex = bottomLimit;
            }
            if (this.suggestionIndex < bottomLimit) {
                this.suggestionIndex = topLimit;
            }
            this.showSuggestions = true;
        });
    }

    suggest(query: string): any {
        return this.dataService.get('/agents/search', { q: query });
    }

    handleSelectSuggestion(suggestion: object) {
        if (!this.activeSuggestion) {
            return;
        }

        this.hideSuggestions();
        this.suggestionSelected.emit(suggestion);
    }

    validateKeyCode(event: KeyboardEvent) {
        return event.keyCode !== Key.Tab
            && event.keyCode !== Key.Shift
            && event.keyCode !== Key.ArrowLeft
            && event.keyCode !== Key.ArrowUp
            && event.keyCode !== Key.ArrowRight
            && event.keyCode !== Key.ArrowDown;
    }

    hideSuggestions() {
        this.suggestionIndex = 0;
        this.showSuggestions = false;
    }

    markIsActive(index: number, suggestion: object) {
        const isActive = index === this.suggestionIndex;

        if (isActive) {
            this.activeSuggestion = suggestion;
        }

        return isActive;
    }

}
