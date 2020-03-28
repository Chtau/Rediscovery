import { Injectable } from "@angular/core";

@Injectable()
export class FeatureService {

  
  constructor() {

  }

  public getFeatures(): IDeviceFeatureDefinition[] {
    return [
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
