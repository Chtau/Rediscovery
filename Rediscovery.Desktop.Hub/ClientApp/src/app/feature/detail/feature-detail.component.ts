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

  model: IDeviceFeature = null;
  settingsUrl: string = null;

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
    /*this.featureService.getFeatureDetailUI(id).subscribe(result => {
      this.settingsUrl = result;
    });*/
    this.featureService.featuresProfileUIReceived.subscribe((result: IEntityContent) => {
      //console.log('received Profile UI:' + JSON.stringify(result));
      if (result.id == this.model.id && this.profileContentWrapper) {
        var componentName = 'profile-component-' + this.onCreateAlphanumericId(result.id);
        var ele = customElements.get(componentName);
        if (!ele) {
          // get component Name
          //class MyComponent extends
          var js = result.content as string;
          var patt = /class (.*) extends/i;
          var match = js.match(patt);
          var componentType = null;
          if (match.length == 2) {
            componentType = match[1];
          }

          // append define custom element and load js
          js += `\r\ncustomElements.define('${componentName}', ${componentType});`;
          this.loadScript(js);

          // Create element
          const popupEl: any = document.createElement(componentName) as any;

          // Listen to the close event
          popupEl.addEventListener('closed', () => document.body.removeChild(popupEl));

          while (this.profileContentWrapper.nativeElement.firstChild) {
            this.profileContentWrapper.nativeElement.removeChild(this.profileContentWrapper.nativeElement.lastChild);
          }
          this.profileContentWrapper.nativeElement.appendChild(popupEl);
        }
      }
    });
    this.featureService.requestFeatureDetailUI(id);
  }

  loaded: boolean = false;
  private loadScript(scriptContent) {
    if (this.loaded == false) {
      this.loaded = true;
      console.log('preparing to load...')
      let node = document.createElement('script');
      node.textContent = scriptContent;
      //node.src = url;
      node.type = 'text/javascript';
      node.async = true;
      node.charset = 'utf-8';
      document.getElementsByTagName('head')[0].appendChild(node);
    }
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
