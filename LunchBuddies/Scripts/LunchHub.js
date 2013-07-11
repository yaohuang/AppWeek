/// <reference path="jquery.signalR-2.0.0-beta1.js" />

$(new function () {
    $.connection.lunchHub.client.newsFeed = function (data) {
        addNews(data);
    }
    $.connection.lunchHub.client.invite = function (data) {
        addInvite(data);
    }
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

            $.connection.lunchHub.server.toNewsFeed("Hello!");
            $.connection.lunchHub.server.fakeInvite(1);
            $.connection.lunchHub.server.fakeInvite(2);
            $.connection.lunchHub.server.fakeInvite(3);
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

function addNews(message) {
    $("#NewsFeedMessages").append("[" + new Date().toTimeString() + "] " + message + "<hr>");
}

function addInvite(request) {
    var log = $("#InviteMessages");
    log.append("From: " + request.From + "<br>");
    log.append("Subject: " + request.Subject + "<br>");
    log.append("Meeting Place: " + request.MeetingPlace + "<br>");
    log.append("Invites: " + "<br>");
    $.each(request.Invitees, function callback(index, item) {
        log.append(item + "<br>");
    });
    log.append("<input type='button' id='accept" + request.Id + "' value='Accept' onClick='acceptInvite(" + request.Id + ")' />");
    log.append("<input type='button' id='reject" + request.Id + "' value='Reject' onClick='rejectInvite(" + request.Id + ")' />");
    log.append("<hr>");
}

function acceptInvite(id) {
    $("#accept" + id).hide();
    $("#reject" + id).hide();
}

function rejectInvite(id) {
    $("#accept" + id).hide();
    $("#reject" + id).hide();
}