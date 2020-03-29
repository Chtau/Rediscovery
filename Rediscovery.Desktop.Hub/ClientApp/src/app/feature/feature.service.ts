import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable()
export class FeatureService {

  models: IDeviceFeatureDefinition[] = [];
  
  constructor() {
    if (environment.isElectron === false) {
      this.models = [
        {
          displayName: "Test 1",
          id: "0",
          version: "v0.0"
        },
        {
          displayName: "Test 2",
          id: "1",
          version: "v0.0"
        }
      ];
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
