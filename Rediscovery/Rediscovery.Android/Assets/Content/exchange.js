var modelData = {};
var callbackFunction = null;
function internalChangeModel(model) {
    modelData = model;
    if (callbackFunction) {
        callbackFunction(modelData);
    }
}

function changeModelListener(callback) {
    callbackFunction = callback;
}

function featureSend(data) {
    jsBridge.invokeAction(data);
}