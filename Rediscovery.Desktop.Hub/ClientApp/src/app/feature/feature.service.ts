import { Injectable, EventEmitter, NgZone } from "@angular/core";
import { environment } from "src/environments/environment";

import * as dummyFeature from '../../assets/dummy/feature.json';
import { IpcService } from "../ipc.service";

@Injectable()
export class FeatureService {

  featuresChanged = new EventEmitter<IDeviceFeature[]>();

  models: IDeviceFeature[] = [];
  
  constructor(private ipc: IpcService,private zone: NgZone) {
    
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
}
