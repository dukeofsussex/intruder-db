import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { AuthService } from '../auth.service';
import { DataService } from '../data.service';
import { TuningService } from './tuning.service';

declare let $: any;
declare let Materialize: any;

@Component({
    selector: 'app-tuning',
    templateUrl: './tuning-details.component.html',
    styleUrls: ['./tuning.component.scss']
})

export class TuningDetailsComponent implements OnInit, OnDestroy {

    canDelete = false;
    canEdit = false;
    canReset = false;
    copied = false;
    defaultTuningValues: any;
    error = false;
    generatedTuning: any;
    notFound = false;
    processing = false;
    stringifiedGeneratedTuning: string;
    tuningFormSettings: any;
    tuning = {
        id: 0,
        name: '',
        description: '',
        author: {
            id: 0,
            name: '',
            avatarURL: ''
        },
        settings: {},
        share: false,
        averageRating: 0,
        lastUpdate: null,
        ratings: [0, 0, 0, 0, 0]
    };
    tuningGroup = 'general';

    @ViewChild('tuningForm') tuningForm: any;

    private ngUnsubscribe = new Subject();

    constructor(private authService: AuthService,
        private dataService: DataService,
        private route: ActivatedRoute,
        private router: Router,
        private titleService: Title,
        private tuningService: TuningService) { }

    ngOnInit() {
        setTimeout(() => {
            $('ul.tabs').tabs();
            $('.modal').modal();
            $('.tooltipped').tooltip();
            $('#name, #description').characterCounter();
        }, 100);

        this.defaultTuningValues = this.tuningService.getDefaultTuningValues(this.tuningService.defaultTuningSettings);
        this.tuningFormSettings = {
            staminaDrainModifier: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.staminaDrainModifier),
            gunSwerveThreshold: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.gunSwerveThreshold),
            updateStaminaRate: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.updateStaminaRate),
            rigidGravity: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.rigidGravity),
            energyToSwayOnMove: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.energyToSwayOnMove),
            MatchMode: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.MatchMode),
            PlayerLife: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.GuardTuning.PlayerLife),
            PlayerMovement: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.GuardTuning.PlayerMovement),
            CharacterMotor: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.GuardTuning.CharacterMotor),
            Pistol: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.GuardTuning.Pistol),
            SMG: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.GuardTuning.SMG),
            Sniper: this.tuningService.generateTuningForm(this.tuningService.defaultTuningSettings.GuardTuning.Sniper),
        };

        this.router.events.pipe(
            takeUntil(this.ngUnsubscribe),
            filter((event) => event instanceof NavigationEnd)
        ).subscribe((event: NavigationEnd) => {
            this.getTuning();
        });

        this.getID();
        if (this.tuning.id.toString() === 'new') {
            this.titleService.setTitle('Tuning Generator - Intruder DB');
            this.canEdit = this.authService.isLoggedIn();
            this.canReset = this.canEdit;
        } else {
            this.getTuning();
        }
    }

    ngOnDestroy() {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    getID() {
        this.route.params.pipe(
            takeUntil(this.ngUnsubscribe)
        ).subscribe(params => this.tuning.id = params.id);
    }

    getTuning() {
        this.dataService.get('/tunings/' + this.tuning.id, null, this.authService.isLoggedIn())
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(tuningSettings => {
                this.tuning = tuningSettings;
                this.titleService.setTitle(tuningSettings.name + ' - Tunings - Intruder DB');
                this.resetForm(tuningSettings.settings);
                this.canDelete = this.authService.isLoggedIn() && this.tuning.author.id === this.authService.getUser().id;
                this.canEdit = this.canDelete;
                this.canReset = this.canEdit;
                setTimeout(() => {
                    $('#description').trigger('autoresize');
                    Materialize.updateTextFields();
                }, 100);
            }, err => {
                let errorResponse = err.error;

                if (errorResponse && errorResponse.code === 404) {
                    this.notFound = errorResponse;
                } else {
                    if (errorResponse === null) {
                        errorResponse = {
                            message: 'Unable to retrieve tuning settings'
                        };
                    }

                    this.error = errorResponse;
                }

                this.titleService.setTitle(errorResponse.code + ' - Tunings - Intruder DB');
            });
    }

    onCopied(copied: boolean) {
        this.copied = true;
        setTimeout(() => this.copied = false, 2500);
    }

    onGenerate(form: any) {
        this.generatedTuning = this.tuningService.generate(form);

        if (this.generatedTuning && Object.keys(this.generatedTuning).length > 0) {
            this.generatedTuning.tuningMessage = 'Tuning created by '
                + `${this.tuning.author.id ? this.tuning.author.name : 'anonymous' } at https://www.intruder-db.info/tunings`;
        }

        this.stringifiedGeneratedTuning = JSON.stringify(this.generatedTuning);
    }

    resetForm(settings: object) {
        this.tuningForm.reset(this.tuningService.extend(true, this.defaultTuningValues, settings));
    }

    selectTuningGroup(tuningGroup: string) {
        this.tuningGroup = tuningGroup;
    }

    save() {
        this.onGenerate(this.tuningForm.value);

        if (!this.generatedTuning) {
            return Materialize.toast('Nothing to save, no values modified', 2500, 'red darken-2');
        }

        delete this.generatedTuning.tuningMessage;

        const tuningToSave = {
            id: this.tuning.id.toString() === 'new' ? 0 : this.tuning.id,
            name: this.tuning.name,
            description: this.tuning.description,
            settings: this.generatedTuning,
            share: this.tuning.share
        };

        this.processing = true;
        if (this.tuning.id.toString() !== 'new') {
            this.dataService.put('/tunings/' + this.tuning.id, tuningToSave)
                .pipe(
                    takeUntil(this.ngUnsubscribe)
                ).subscribe(tuningSettings => {
                    this.processing = false;
                    this.tuning = tuningSettings;
                    this.titleService.setTitle(tuningSettings.name + ' - Tunings - Intruder DB');
                    this.resetForm(tuningSettings.settings);
                    Materialize.toast('Tuning settings updated', 2500, 'green darken-2');
                }, err => {
                    let errorResponse = err.error;

                    if (errorResponse === null) {
                        errorResponse = {
                            message: 'Unable to update tuning settings'
                        };
                    }

                    Materialize.toast(errorResponse.message, 2500, 'red darken-2');
                    this.processing = false;
                });
        } else {
            this.dataService.post('/tunings', tuningToSave, true)
                .pipe(
                    takeUntil(this.ngUnsubscribe)
                ).subscribe(tuningSettings => {
                    this.processing = false;
                    Materialize.toast('Tuning settings saved', 2500, 'green darken-2');
                    this.router.navigate(['/tunings', tuningSettings.id]);
                }, err => {
                    let errorResponse = err.error;

                    if (errorResponse === null) {
                        errorResponse = {
                            message: 'Unable to save tuning settings'
                        };
                    }

                    Materialize.toast(errorResponse.message, 2500, 'red darken-2');
                    this.processing = false;
                });
        }
    }

    deleteWithTimeout() {
        setTimeout(() => this.delete(), 200);
    }

    private delete() {
        this.processing = true;
        this.dataService.delete('/tunings/' + this.tuning.id)
            .pipe(
                takeUntil(this.ngUnsubscribe)
            ).subscribe(confirmation => {
                this.processing = false;
                Materialize.toast('Tuning settings deleted', 2500, 'green darken-2');
                this.router.navigate(['/tunings']);
            }, err => {
                let errorResponse = err.error;

                if (errorResponse === null) {
                    errorResponse = {
                        message: 'Unable to delete tuning settings'
                    };
                }

                Materialize.toast(errorResponse.message, 2500, 'red darken-2');
                this.processing = false;
            });
    }
}
