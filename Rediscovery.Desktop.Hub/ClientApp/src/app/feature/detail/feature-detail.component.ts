import { Component, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { FeatureService } from '../feature.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { IOService } from 'src/app/io.service';

declare var getModel: any;
declare var setModel: any;

@Component({
  selector: 'app-feature-detail',
  templateUrl: './feature-detail.component.html',
  styleUrls: ['./feature-detail.component.css']
})
export class FeatureDetailComponent implements AfterViewInit {
  
  @ViewChild('profileContentWrapper') profileContentWrapper: ElementRef;
  @ViewChild('settingContentWrapper') settingContentWrapper: ElementRef;

  model: IDeviceFeature = null;
  modelProfiles: IEntityContent<string, IDeviceFeatureProfil[]> = null;
  modelSettings: IEntityContent<string, IDeviceFeatureSetting> = null;
  selectedProfileId: string = null;
  selectedProfileName: string = null;

  get SelectedProfileId() : string {
    return this.selectedProfileId;
  }
  set SelectedProfileId(value: string) {
    this.selectedProfileId = value;
    var curModel = this.modelProfiles.content.find(item => {
      if (item.id == this.selectedProfileId) {
        return true;
      }
    });
    this.selectedProfileName = curModel.displayName;
    this.onSetProfileContentModel(curModel);
  }

  constructor(private featureService: FeatureService,
    private ioService: IOService,
    private route: ActivatedRoute,
    private router: Router) {
    this.onLoadModel(this.route.snapshot.paramMap.get('id'));
    router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.onLoadModel(this.route.snapshot.paramMap.get('id'));
      }
    });
  }

  ngAfterViewInit(): void {
    
  }

  private onLoadModel(id: string): void {
    this.model = this.featureService.getFeatureDetail(id);
    this.featureService.featuresProfilesReceived.subscribe((result: IEntityContent<string, IDeviceFeatureProfil[]>) => {
      if (this.model && this.model.id == result.id) {
        this.modelProfiles = result;
        this.onSetProfilesDropModel();
      }
    });
    this.featureService.featuresSettingsReceived.subscribe((result: IEntityContent<string, IDeviceFeatureSetting>) => {
      if (this.model && this.model.id == result.id) {
        this.modelSettings = result;
        this.onSetSettingsContentModel();
      }
    });
    this.featureService.featuresProfileUIReceived.subscribe((result: IEntityContent<string, string>) => {
      this.setupWebElementUI(result, this.profileContentWrapper, (value) => {
        this.featureService.saveFeatureProfile((this.model.id, value));
      });
      this.onSetProfilesDropModel();
    });
    this.featureService.featuresSettingUIReceived.subscribe((result: IEntityContent<string, string>) => {
      this.setupWebElementUI(result, this.settingContentWrapper, (value) => {
        this.featureService.saveFeatureSetting((this.model.id, value));
      });
      this.onSetSettingsContentModel();
    });
    this.featureService.requestFeatureDetailUI(id);
  }

  private setupWebElementUI(result: IEntityContent<string, string>, hostElement: ElementRef, changedCallback: (value) => void):void {
    if (result && result.content) {
      if (result.id == this.model.id && hostElement) {
        var js = result.content as string;
        var componentName = "my-" + this.getWebElementType(js).toLowerCase();
        var ele = customElements.get(componentName);
        if (!ele) {
          js += `\r\ncustomElements.define('${componentName}', ${this.getWebElementType(js)});`;
          this.loadScript(js);
        }
        this.addWebElement(componentName, hostElement, changedCallback);
      }
    }
  }

  private addWebElement(componentName: string, hostElement: ElementRef, changedCallback: (value) => void): void {
    const control: any = document.createElement(componentName) as any;
    control.addEventListener('closed', () => document.body.removeChild(control));
    // clean up previously added web elements
    while (hostElement.nativeElement.firstChild) {
      hostElement.nativeElement.removeChild(hostElement.nativeElement.lastChild);
    }
    hostElement.nativeElement.appendChild(control);
    control.addEventListener('modelchanged', e => changedCallback(e.detail));
  }

  private getWebElementType(js: string): string {
    var patt = /class (.*) extends/i;
    var match = js.match(patt);
    if (match.length == 2) {
      return match[1];
    }
    return null;
  }

  private loadScript(scriptContent) {
    let node = document.createElement('script');
    node.textContent = scriptContent;
    node.type = 'text/javascript';
    node.async = true;
    node.async = false;
    node.charset = 'utf-8';
    document.getElementsByTagName('head')[0].appendChild(node);
  }

  private onCreateAlphanumericId(id: string) : string {
    var map = [
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
    ];
    var idArray = id.split('');
    var alphaId: string = "";
    idArray.forEach(item => {
      var num = parseInt(item);
      if (!isNaN(num)) {
        alphaId += map[num];
      }
    });
    return alphaId;
  }

  openFolder(): void {
    this.ioService.openDirectory(this.model.pluginDirectory);
  }

  onSaveProfile(): void {
    if (this.profileContentWrapper && this.profileContentWrapper.nativeElement && this.profileContentWrapper.nativeElement.children.length > 0) {
      this.profileContentWrapper.nativeElement.children[0].setAttribute('getModel', null);
    }
  }

  onSaveSetting(): void {
    if (this.settingContentWrapper && this.settingContentWrapper.nativeElement && this.settingContentWrapper.nativeElement.children.length > 0) {
      this.settingContentWrapper.nativeElement.children[0].setAttribute('getModel', null);
    }
  }

  private onSetSettingsContentModel(): void {
    if (this.settingContentWrapper && this.settingContentWrapper.nativeElement && this.modelSettings) {
      this.settingContentWrapper.nativeElement.children[0].setAttribute('setModel', JSON.stringify(this.modelSettings));
    }
  }

  private onSetProfilesDropModel(): void {
    if (this.modelProfiles.content.length > 0)
    this.SelectedProfileId = this.modelProfiles.content[0].id;
  }

  private onSetProfileContentModel(profile: IDeviceFeatureProfil): void {
    if (this.profileContentWrapper && this.profileContentWrapper.nativeElement) {
      this.profileContentWrapper.nativeElement.children[0].setAttribute('setModel', JSON.stringify(profile));
    }
  }
}
