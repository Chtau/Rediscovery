function featureSend(data) {
    jsBridge.invokeAction(JSON.stringify(data));
}