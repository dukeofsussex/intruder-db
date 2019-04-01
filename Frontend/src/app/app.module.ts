import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { Angulartics2Module } from 'angulartics2';
import { Angulartics2Piwik } from 'angulartics2/piwik';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AuthService } from './auth.service';
import { AuthGuardService } from './auth-guard.service';
import { ClaimGuardService } from './claim-guard.service';
import { DataService } from './data.service';

import { AgentModule } from './agent/agent.module';
import { AuthModule } from './auth/auth.module';
import { ChangelogModule } from './changelog/changelog.module';
import { DashboardModule } from './dashboard/dashboard.module';
import { GuideModule } from './guide/guide.module';
import { LandingPageModule } from './landing-page/landing-page.module';
import { LegalModule } from './legal/legal.module';
import { MapModule } from './map/map.module';
import { ErrorModule } from './error/error.module';
import { PageNotFoundRoutingModule } from './error/page-not-found-routing.module';
import { ServerModule } from './server/server.module';
import { SharedModule } from './shared/shared.module';
import { StatsModule } from './stats/stats.module';
import { TuningModule } from './tuning/tuning.module';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        Angulartics2Module.forRoot([Angulartics2Piwik]),
        AppRoutingModule,
        AgentModule,
        AuthModule,
        ChangelogModule,
        DashboardModule,
        GuideModule,
        LandingPageModule,
        LegalModule,
        MapModule,
        ServerModule,
        SharedModule,
        StatsModule,
        TuningModule,
        ErrorModule,
        PageNotFoundRoutingModule
    ],
    providers: [AuthService, AuthGuardService, ClaimGuardService, DataService],
    bootstrap: [AppComponent]
})
export class AppModule { }
