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
import { DeviceService } from './device/device.service';
import { DeviceOverviewComponent } from './device/overview/device-overview.component';
import { DeviceDetailComponent } from './device/detail/device-detail.component';
import { DeviceRegisteredComponent } from './device/registered/device-registered.component';
import { InfoComponent } from './info/info.component';
import { SettingComponent } from './setting/setting.component';
import { IpcService } from './ipc.service';

const routes: Route[] = [
  { path: 'counter', component: CounterComponent },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'logger', component: LoggerComponent },
  { path: 'info', component: InfoComponent },
  { path: 'settings', component: SettingComponent },
  { 
    path: 'devices', 
    component: DeviceListComponent,
    children: [
      {
        path: 'registered/:id', 
        component: DeviceRegisteredComponent,
      },
      {
        path: ':id', 
        component: DeviceDetailComponent,
      },
      {
        path: '', 
        component: DeviceOverviewComponent,
      }
    ]
  },
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
    FeatureDetailComponent,
    DeviceOverviewComponent,
    DeviceDetailComponent,
    DeviceRegisteredComponent,
    InfoComponent,
    SettingComponent
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
    FeatureService,
    DeviceService,
    IpcService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
