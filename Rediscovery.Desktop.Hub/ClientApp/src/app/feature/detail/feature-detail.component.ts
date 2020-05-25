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
      this.setupWebElementUI(result, this.profileContentWrapper, 'component-');
    });
    this.featureService.featuresSettingUIReceived.subscribe((result: IEntityContent<string, string>) => {
      this.setupWebElementUI(result, this.settingContentWrapper, 'component-');
      this.onSetSettingsContentModel();
    });
    this.featureService.requestFeatureDetailUI(id);
  }

  private setupWebElementUI(result: IEntityContent<string, string>, hostElement: ElementRef, componentBaseName: string):void {
    if (result && result.content) {
      if (result.id == this.model.id && hostElement) {
        var js = result.content as string;
        var componentName = "my-" + this.getWebElementType(js).toLowerCase();//componentBaseName + this.getWebElementType(js);//componentBaseName + this.getWebElementType(js) + '-' + this.onCreateAlphanumericId(result.id);
        var ele = customElements.get(componentName);
        if (!ele) {
          console.log('add custom element:' + componentName);
          // append define custom element and load js
          js += `\r\ncustomElements.define('${componentName}', ${this.getWebElementType(js)});`;
          //customElements.define('myjsoncomponent', JSONComponent);
          //js += `\r\ncustomElements.define('my-jsoncomponent', JSONComponent);`;
          this.loadScript(js);
        }
        this.addWebElement(componentName, hostElement);
      }
    }
  }

  private addWebElement(componentName: string, hostElement: ElementRef): void {
    // Create element
    const control: any = document.createElement(componentName) as any;
    // Listen to the close event
    control.addEventListener('closed', () => document.body.removeChild(control));
    // clean up previously added web elements
    while (hostElement.nativeElement.firstChild) {
      hostElement.nativeElement.removeChild(hostElement.nativeElement.lastChild);
    }
    hostElement.nativeElement.appendChild(control);
    control.addEventListener('modelchanged', e => console.log(e.detail));
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
    //node.async = true;
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

  onSave(): void {
    // TODO: test only
    this.profileContentWrapper.nativeElement.children[0].setAttribute('setModel', JSON.stringify({
      name: "Profile Test",
      id: "1"
    }));
    this.profileContentWrapper.nativeElement.children[0].setAttribute('getModel', null);
  }

  private onSetSettingsContentModel(): void {
    if (this.settingContentWrapper && this.settingContentWrapper.nativeElement && this.modelSettings) {
      this.settingContentWrapper.nativeElement.children[0].setAttribute('setModel', JSON.stringify(this.modelSettings));
    }
  }

  private onSetProfilesDropModel(): void {
    // TODO: set select box for profile
    // TODO: set display name
  }

  private onSetProfileContentModel(profile: IDeviceFeatureProfil): void {
    if (this.profileContentWrapper && this.profileContentWrapper.nativeElement) {
      this.profileContentWrapper.nativeElement.children[0].setAttribute('setModel', JSON.stringify(profile));
    }
  }
}
