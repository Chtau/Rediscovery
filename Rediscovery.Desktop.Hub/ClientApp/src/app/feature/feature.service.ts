import { Injectable, EventEmitter, NgZone } from "@angular/core";
import { environment } from "src/environments/environment";

import * as dummyFeature from '../../assets/dummy/feature.json';
//import * as dummyFeatureUI from '../../assets/dummy/featuresetting.html';
import { IpcService } from "../ipc.service";
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import {map} from 'rxjs/operators';

@Injectable()
export class FeatureService {

  featuresChanged = new EventEmitter<IDeviceFeature[]>();

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

  public getFeatureDetailUI(id: string): Observable<string> {
    return this.http.get('../../assets/dummy/featuresetting.html', {responseType: 'text'}).pipe(map(data => {
      //console.log('data', data);
      return data;
    }));
    //return '../../assets/dummy/featuresetting.html';//<string>dummyFeatureUI.default;
  }
}
