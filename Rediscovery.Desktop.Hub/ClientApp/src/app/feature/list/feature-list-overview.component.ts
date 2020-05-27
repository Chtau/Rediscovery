import { Component } from '@angular/core';
import { FeatureService } from '../feature.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-feature-list-overview',
  templateUrl: './feature-list-overview.component.html',
  styleUrls: ['./feature-list-overview.component.css']
})
export class FeatureListOverviewComponent {
  
  public featureModels: IDeviceFeature[] = [];

  constructor(private featureService: FeatureService, private route: Router) {
    this.featureModels = featureService.getFeatures();

    this.featureService.featuresChanged.subscribe(result => {
      this.featureModels = result;
    });
  }

  onViewFeature(featureModel: IDeviceFeature): void {
    this.route.navigate(['/features/',featureModel.id])
  }
}
