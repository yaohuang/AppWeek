/// <reference path="jquery.signalR-2.0.0-beta1.js" />

$(new function () {
    $.connection.lunchHub.client.echo = function (data) {
        log(data);
    }

    $.connection.hub.logging = true;
    $.connection.hub.connectionSlow(function () {
        log("[connectionSlow]");
    });
    $.connection.hub.disconnected(function () {
        log("[disconnected]");
    });
    $.connection.hub.error(function (error) {
        log("[error]" + error);
    });
    $.connection.hub.received(function (payload) {
        log("[received]" + window.JSON.stringify(payload));
    });
    $.connection.hub.reconnected(function () {
        log("[reconnected]");
    });
    $.connection.hub.reconnecting(function () {
        log("[reconnecting]");
    });
    $.connection.hub.starting(function () {
        log("[starting]");
    });
    $.connection.hub.stateChanged(function (change) {
        log("[stateChanged] " + displayState(change.oldState) + " => " + displayState(change.newState));
    });

    $.connection.hub.start().
        done(function () {
            log("Connected");
            log("transport.name=" + $.connection.hub.transport.name);

            $.connection.lunchHub.server.echo("Hello!");
        }).
        fail(function (error) {
            log("Failed to connect: " + error);
        });
});

function displayState(state) {
    return ["connecting", "connected", "reconnecting", state, "disconnected"][state];
}

function log(message) {
    $("#Messages").append("<li>[" + new Date().toTimeString() + "] " + message + "</li>");
}
