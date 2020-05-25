class JSONComponent extends HTMLElement {

  model = null;

  static get observedAttributes() {
    return ['setmodel', 'getmodel'];
  }

  constructor() {
    super();
  }

  connectedCallback() {
    this.setContent(null);
  }

  attributeChangedCallback(name, oldValue, newValue) {
    var newValueObj = JSON.parse(newValue);
    //console.log(`attributeChangedCallback in HTMLElement name:${name} newValue:${JSON.stringify(newValueObj)}`);
    if (name == 'setmodel') {
      this.model = newValueObj;
      this.setContent(newValueObj);
    } else if (name == 'getmodel') {
      this.dispatchEvent(new CustomEvent('modelchanged', { 
        detail: this.model
      }))
    }
  }

  setContent(jsonObj) {
    if (jsonObj) {
      this.innerHTML = `
      <style>
        .content-wrapper {
          height: 100%;
          width: 100%;
        }
        pre {
          height: 100%;
          width: 100%;
          margin: 0;
          padding: 5px;
        }
      </style>
      <div class="content-wrapper">
      <pre>${JSON.stringify(jsonObj, null, 4)}</pre>
      </div>`
    } else {
      this.innerHTML = `<h1>No content</h1>`;
    }
  }
}