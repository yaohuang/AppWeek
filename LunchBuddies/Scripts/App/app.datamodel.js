function AppDataModel() {
    var self = this;

    self.clearAccessToken = function () {
        localStorage.removeItem("accessToken");
        sessionStorage.removeItem("accessToken");
    };
    self.setAccessToken = function (accessToken, persistent) {
        if (persistent)
            localStorage["accessToken"] = accessToken;
        else
            sessionStorage["accessToken"] = accessToken;
    };

    // ajax helper
    function ajaxRequest(type, url, data, dataType) {
        var options = {
            dataType: dataType || "json",
            contentType: "application/json",
            cache: false,
            type: type,
            data: data ? data.toJson() : null
        };
        var accessToken = sessionStorage["accessToken"] || localStorage["accessToken"];
        if (accessToken) {
            options.headers = {
                "Authorization": "Bearer " + accessToken
            }
        }
        return $.ajax(url, options);
    }

    // routes
    function todoListUrl(id) { return "/api/TodoList/" + (id || ""); }
    function todoItemUrl(id) { return "/api/Todo/" + (id || ""); }
    function externalLoginCallbackUrl(state) { return "/api/AjaxAccount/ExternalLoginCallback?state=" + (state || ""); }
    var loginUrl = "/api/AjaxAccount/Login";
    var logoffUrl = "/api/AjaxAccount/LogOff";
    var registerUrl = "/api/AjaxAccount/Register";
    var registerExternalUrl = "/api/AjaxAccount/RegisterExternal";
    var userInfoUrl = "/api/AjaxAccount/UserInfo";

    // data access
    self.getTodoLists = function () {
        return ajaxRequest("GET", todoListUrl());
    };
    self.saveNewTodoItem = function (todoItem) {
        return ajaxRequest("POST", todoItemUrl(), todoItem);
    };
    self.saveNewTodoList = function (todoList) {
        return ajaxRequest("POST", todoListUrl(), todoList);
    };
    self.deleteTodoItem = function (todoItem) {
        return ajaxRequest("DELETE", todoItemUrl(todoItem.todoItemId));
    };
    self.deleteTodoList = function (todoList) {
        return ajaxRequest("DELETE", todoListUrl(todoList.todoListId));
    };
    self.saveChangedTodoItem = function (todoItem) {
        return ajaxRequest("PUT", todoItemUrl(todoItem.todoItemId), todoItem, "text");
    };
    self.saveChangedTodoList = function (todoList) {
        return ajaxRequest("PUT", todoListUrl(todoList.todoListId), todoList, "text");
    };
    self.getUserInfo = function () {
        return ajaxRequest("GET", userInfoUrl);
    };
    self.login = function (user) {
        return $.ajax(loginUrl, {
            type: "POST",
            data: user
        });
    };
    self.externalLoginCallback = function (state) {
        return $.ajax(externalLoginCallbackUrl(state));
    };
    self.logoff = function () {
        return ajaxRequest("POST", logoffUrl);
    };
    self.register = function (user) {
        return $.ajax(registerUrl, {
            type: "POST",
            data: user
        });
    };
    self.registerExternal = function (user) {
        return $.ajax(registerExternalUrl, {
            type: "POST",
            data: user
        });
    };
}
