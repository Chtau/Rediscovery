import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

import * as dummyFeature from '../../assets/dummy/feature.json';

@Injectable()
export class FeatureService {

  models: IDeviceFeatureDefinition[] = [];
  
  constructor() {
    if (environment.isElectron === false) {
      this.models = <IDeviceFeatureDefinition[]>dummyFeature.default;
    }
  }

  public getFeatures(): IDeviceFeatureDefinition[] {
    return this.models;
  }

  public getFeatureDetail(id: string): IDeviceFeatureDefinition {
    return this.models.find(x => {
      if (x.id == id) {
        return x;
      }
    });
  }
}
