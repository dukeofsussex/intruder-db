import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-guide',
    templateUrl: './guide.component.html',
    styleUrls: ['./guide.component.scss']
})
export class GuideComponent implements OnInit {

    constructor(private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle('Guides - Intruder DB');
    }

}
