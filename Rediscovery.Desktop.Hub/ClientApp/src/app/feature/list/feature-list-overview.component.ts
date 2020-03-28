import { Component } from '@angular/core';

@Component({
  selector: 'app-feature-list-overview',
  templateUrl: './feature-list-overview.component.html',
  styleUrls: ['./feature-list-overview.component.css']
})
export class FeatureListOverviewComponent {
  
  public featureModels: IDeviceFeatureDefinition[] = [];

}
