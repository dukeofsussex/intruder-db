import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';

declare let $: any;

@Component({
    selector: 'app-legal',
    templateUrl: './legal.component.html',
    styleUrls: ['./legal.component.scss']
})
export class LegalComponent implements OnInit {

    template = '';

    constructor(private route: ActivatedRoute, private router: Router, private titleService: Title) { }

    ngOnInit() {
        this.setActiveTab();
    }

    setActiveTab() {
        let title;

        switch (this.router.url) {
            case '/legal/disclaimer':
                title = 'Disclaimer';
                break;
            case '/legal/privacy-policy':
                title = 'Privacy Policy';
                break;
            case '/legal/terms-of-service':
            default:
                title = 'Terms of Service';

        }

        this.titleService.setTitle(`${title} - Intruder DB`);
        this.template = title.toLowerCase().replace(/\s/g, '-');
        setTimeout(() => $('ul.tabs').tabs(), 100);
    }
}
