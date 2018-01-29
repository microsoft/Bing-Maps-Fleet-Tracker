import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/shared.module';

import { CoreRoutingModule, routedComponents } from './core-routing.module';

import { NavComponent } from './nav/nav.component';
import { SettingsComponent } from './settings/settings.component';
import { throwIfAlreadyLoaded } from './module-import-guard';

import { DataService } from './data.service';
import { TripService } from './trip.service';
import { SettingsService } from './settings.service';
import { EnvironmentSettingsService } from './environment-settings.service';
import { SpinnerService } from './spinner.service';
import { AuthorizedHttpService } from './authorized-http.service';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpModule,
        RouterModule,
        CoreRoutingModule,
        SharedModule
    ],
    declarations: [
        NavComponent,
        routedComponents
    ],
    exports: [
        FormsModule,
        NavComponent,
        SettingsComponent
    ],
    providers: [
        EnvironmentSettingsService,
        AuthorizedHttpService,
        SettingsService,
        SpinnerService,
        DataService,
        TripService
    ]
})
export class CoreModule {
    constructor( @Optional() @SkipSelf() parentModule: CoreModule) {
        throwIfAlreadyLoaded(parentModule, 'CoreModule');
    }
}
