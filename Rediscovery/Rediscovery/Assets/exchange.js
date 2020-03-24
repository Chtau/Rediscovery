var modelData = undefined;
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

function currentModel() {
    return modelData;
}

function featureSend(data) {
    jsBridge.invokeAction(data);
}