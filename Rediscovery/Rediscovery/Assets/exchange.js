function featureSend(data) {
    jsBridge.invokeAction(JSON.stringify(data));
}
function domReady() {
    jsBridge.invokeDOMReady();
}
function logger(data) {
    jsBridge.invokeLogger(JSON.stringify(data));
}
(function () {
    domReady();
})();