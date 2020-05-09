function featureSend(data) {
    jsBridge.invokeAction(JSON.stringify(data));
}
function domReady() {
    jsBridge.invokeDOMReady();
}
(function () {
    domReady();
})();