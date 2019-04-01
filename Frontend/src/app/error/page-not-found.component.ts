import { Component, Input, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'not-found',
    templateUrl: './page-not-found.component.html',
    styleUrls: ['./page-not-found.component.scss']
})
export class PageNotFoundComponent implements OnInit {
    @Input() item = 'Page';

    constructor(private titleService: Title) { }

    ngOnInit(): void {
        this.titleService.setTitle('404 Not Found - Intruder DB');
    }
}
