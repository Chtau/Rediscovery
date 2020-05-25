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
    this.featureService.featuresProfileUIReceived.subscribe((result: IEntityContent) => {
      this.setupWebElementUI(result, this.profileContentWrapper, 'profile-component-');
    });
    this.featureService.featuresSettingUIReceived.subscribe((result: IEntityContent) => {
      this.setupWebElementUI(result, this.settingContentWrapper, 'setting-component-');
    });
    this.featureService.requestFeatureDetailUI(id);
  }

  private setupWebElementUI(result: IEntityContent, hostElement: ElementRef, componentBaseName: string):void {
    if (result && result.content) {
      if (result.id == this.model.id && hostElement) {
        var componentName = componentBaseName + this.onCreateAlphanumericId(result.id);
        var ele = customElements.get(componentName);
        if (!ele) {
          var js = result.content as string;
          // append define custom element and load js
          js += `\r\ncustomElements.define('${componentName}', ${this.getWebElementType(js)});`;
          this.loadScript(js);
        }
        this.addWebElement(componentName, hostElement);
      }
    }
  }

  private addWebElement(componentName: string, hostElement: ElementRef): void {
    // Create element
    const controlEl: any = document.createElement(componentName) as any;

    // Listen to the close event
    controlEl.addEventListener('closed', () => document.body.removeChild(controlEl));

    while (hostElement.nativeElement.firstChild) {
      hostElement.nativeElement.removeChild(hostElement.nativeElement.lastChild);
    }
    hostElement.nativeElement.appendChild(controlEl);
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
    // TODO: we should integrate Web Components as Profile and Setting edit view 
    // TODO: https://github.com/mdn/web-components-examples/blob/master/editable-list/main.js
    // TODO: https://developer.mozilla.org/en-US/docs/Web/Web_Components
    // TODO: https://angular.io/guide/elements
    // TODO: test only
    console.log('Try to set Model in Profile configuration JS');
    setModel({
      name: "Profile Test",
      id: "1"
    });
  }
}
