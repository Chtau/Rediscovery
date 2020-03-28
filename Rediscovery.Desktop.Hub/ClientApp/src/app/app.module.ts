import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule, Route } from '@angular/router';

import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { ClarityModule } from "@clr/angular";

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { DeviceListComponent } from './device/device-list.component';
import { FeatureListComponent } from './feature/feature-list.component';
import { FeatureListOverviewComponent } from './feature/list/feature-list-overview.component';
import { LoggerComponent } from './logger/logger.component';
import { LoggerService } from './logger/logger.service';
import { FeatureService } from './feature/feature.service';
import { FeatureDetailComponent } from './feature/detail/feature-detail.component';

const routes: Route[] = [
  { path: 'counter', component: CounterComponent },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'logger', component: LoggerComponent },
  { path: 'devices', component: DeviceListComponent },
  { 
    path: 'features',
    component: FeatureListComponent,
    children: [
      {
        path: ':id', 
        component: FeatureDetailComponent,
      },
      {
        path: '', 
        component: FeatureListOverviewComponent,
      }
    ]
  },
  { path: '**', component: HomeComponent },
];

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    DeviceListComponent,
    FeatureListComponent,
    FeatureListOverviewComponent,
    LoggerComponent,
    FeatureDetailComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(routes),
    BrowserAnimationsModule,
    ClarityModule
  ],
  providers: [
    LoggerService,
    FeatureService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
