import { Injectable, EventEmitter, NgZone } from "@angular/core";
import { environment } from "src/environments/environment";

import * as dummyFeature from '../../assets/dummy/feature.json';
import * as dummyFeatureProfiles from '../../assets/dummy/featureprofiles.json';
import * as dummyFeatureSettings from '../../assets/dummy/featuresettings.json';
import { IpcService } from "../ipc.service";
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import {map} from 'rxjs/operators';

@Injectable()
export class FeatureService {

  featuresChanged = new EventEmitter<IDeviceFeature[]>();
  featuresProfileUIReceived = new EventEmitter<IEntityContent<string, string>>();
  featuresSettingUIReceived = new EventEmitter<IEntityContent<string, string>>();
  featuresProfilesReceived = new EventEmitter<IEntityContent<string, IDeviceFeatureProfil[]>>();
  featuresSettingsReceived = new EventEmitter<IEntityContent<string, IDeviceFeatureSetting>>();

  models: IDeviceFeature[] = [];
  
  constructor(private ipc: IpcService,private zone: NgZone, private http: HttpClient) {
    
  }

  public initIPC(): void {
    console.log("init Features IPC");
    if (environment.isElectron === false) {
      this.models = <IDeviceFeature[]>dummyFeature.default;
    } else {
      this.ipc.on('features-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.models = arg;
          this.featuresChanged.emit(this.models);
        });
      });
      this.ipc.on('features-profile-ui-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.featuresProfileUIReceived.emit(arg);
        });
      });
      this.ipc.on('features-setting-ui-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.featuresSettingUIReceived.emit(arg);
        });
      });
      this.ipc.on('features-settings-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.featuresSettingsReceived.emit(arg);
        });
      });
      this.ipc.on('features-profiles-ipc', (event, arg) => {
        // switch to angular zone for change detected events ...
        this.zone.run(() => {
          this.featuresProfilesReceived.emit(arg);
        });
      });
    }
  }

  public getFeatures(): IDeviceFeature[] {
    return this.models;
  }

  public getFeatureDetail(id: string): IDeviceFeature {
    return this.models.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }

  public requestFeatureDetail(id: string): void {
    if (environment.isElectron === true) {
      this.ipc.send('request-features-detail-ipc', id);
    } else {
      
    }
  }

  public requestFeatureDetailUI(id: string): void {
    if (environment.isElectron === true) {
      this.ipc.send('request-features-detail-ui-ipc', id);
    } else {
      this.getFeatureDetailEmulate(id).subscribe(result => {

      });
    }
  }

  public getFeatureDetailEmulate(id: string): Observable<string> {
    this.featuresProfilesReceived.emit({id: id, content: <IDeviceFeatureProfil[]>dummyFeatureProfiles.default});
    this.featuresSettingsReceived.emit({id: id, content: <IDeviceFeatureSetting>dummyFeatureSettings.default});
    return this.http.get('../../assets/dummy/jsonelement.js', {responseType: 'text'}).pipe(map(data => {
      //console.log('data', data);
      this.featuresProfileUIReceived.emit({ id: id, content: data});
      this.featuresSettingUIReceived.emit({ id: id, content: data});
      return data;
    }));
  }

  public saveFeatureProfile(entity: IEntityContent<string, IDeviceFeatureProfil>): void {
    if (environment.isElectron === true) {
      this.ipc.send('request-features-save-profile-ipc', entity);
    } else {
      console.log('Save feature profile: ' + JSON.stringify(entity));
    }
  }

  public deleteFeatureProfile(entity: IEntityContent<string, string>): void {
    if (environment.isElectron === true) {
      this.ipc.send('request-features-delete-profile-ipc', entity);
    } else {
      console.log('Delete feature profile: ' + JSON.stringify(entity));
    }
  }

  public saveFeatureSetting(entity: IEntityContent<string, IDeviceFeatureSetting>): void {
    if (environment.isElectron === true) {
      this.ipc.send('request-features-save-setting-ipc', entity);
    } else {
      console.log('Save feature profile: ' + JSON.stringify(entity));
    }
  }
}
