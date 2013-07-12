function AppViewModel(dataModel) {
    var self = this;

    self.Views = {
        Loading: "loading",
        Todo: "todo",
        Login: "login",
        Register: "register",
        ConfirmRegistration: "confirmregistration"
    };

    self.chosenViewId = ko.observable(self.Views.Loading);

    // data-bind with
    self.user = ko.observable(null);

    self.todo = new TodoViewModel(self, dataModel);

    self.login = new LoginViewModel(self, dataModel);

    self.register = new RegisterViewModel(self, dataModel);

    self.confirmRegistration = new ConfirmRegistrationViewModel(self, dataModel);

    // Client-side routes    
    Sammy(function () {
        this.get('#:view', function () {
            self.chosenViewId(this.params.view);
        });
        this.get('#confirmregistration/:token', function () {
            self.chosenViewId(self.Views.ConfirmRegistration);
            //self.confirmRegistration.token(this.params.token);
            dataModel.confirmRegistration(this.params.token).done(function (data) {
                self.confirmRegistration.userName(data.userName);
                self.confirmRegistration.email(data.email);
                self.confirmRegistration.office(data.office);
                self.confirmRegistration.telephone(data.telephone);
                self.confirmRegistration.department(data.department);
                self.confirmRegistration.title(data.title);
                self.confirmRegistration.pictureUrl(data.pictureUrl);
            }).fail(function () {
                alert("something went wrong.");
            });
        });
    }).run();

    // child coordination
    self.errors = ko.observableArray();

    self.archiveSessionStorageToLocalStorage = function () {
        var backup = {};
        for (var i = 0; i < sessionStorage.length; i++)
            backup[sessionStorage.key(i)] = sessionStorage[sessionStorage.key(i)];
        localStorage["sessionStorageBackup"] = JSON.stringify(backup);
        sessionStorage.clear();
    };

    self.restoreSessionStorageFromLocalStorage = function () {
        var backupText = localStorage["sessionStorageBackup"];

        if (backupText) {
            var backup = JSON.parse(backupText);

            for (var key in backup) {
                sessionStorage[key] = backup[key];
            }

            localStorage.removeItem("sessionStorageBackup");
        }
    };

    self.navigateToLoggedIn = function (userName, accessToken, persistent) {
        self.errors.removeAll();

        if (accessToken)
            dataModel.setAccessToken(accessToken, persistent)

        self.user(new UserInfoViewModel(self, userName, dataModel));
        self.navigateToTodo();
    };
    self.navigateToTodo = function () {
        self.errors.removeAll();
        self.chosenViewId(self.Views.Todo);
    };
    self.navigateToLoggedOff = function () {
        self.errors.removeAll();
        dataModel.clearAccessToken();
        self.navigateToLogin();
    };

    self.navigateToTodo = function () {
        self.errors.removeAll();
        self.chosenViewId(self.Views.Todo);
    };
    self.navigateToRegister = function () {
        self.errors.removeAll();
        self.chosenViewId(self.Views.Register);
    };

    self.navigateToLogin = function () {
        self.errors.removeAll();
        self.user(null);
        self.chosenViewId(self.Views.Login);
    };


    //self.loading = ko.computed(function () {
    //    return view() === Views.Loading;
    //});
    //self.todo = ko.computed(function () {
    //    if (view() !== Views.Todo)
    //        return null;


    function cleanUpLocation() {
        window.location.hash = "";

        if (typeof (history.pushState) !== "undefined") {
            history.pushState("", document.title, location.pathname);
        }
    }

    function parseQueryString(queryString) {
        var data = {};

        if (queryString == null)
            return data;

        var pairs = queryString.split("&");
        for (var i = 0; i < pairs.length; i++) {
            var pair = pairs[i];
            var separatorIndex = pair.indexOf("=");

            var escapedKey;
            var escapedValue;

            if (separatorIndex == -1) {
                escapedKey = pair;
                escapedValue = null;
            }
            else {
                escapedKey = pair.substr(0, separatorIndex);
                escapedValue = pair.substr(separatorIndex + 1);
            }

            var key = decodeURIComponent(escapedKey);
            var value = decodeURIComponent(escapedValue);

            data[key] = value;
        }

        return data;
    }

    function verifyStateMatch(fragment) {
        if (typeof (fragment.access_token) !== "undefined") {
            var state = sessionStorage["state"];
            sessionStorage.removeItem("state");

            if (state == null || fragment.state != state)
                fragment.error = "invalid_state";
        }
    }

    var fragment;
    if (window.location.hash.indexOf("#") == 0)
        fragment = parseQueryString(window.location.hash.substr(1));
    else
        fragment = {};

    self.restoreSessionStorageFromLocalStorage();

    verifyStateMatch(fragment);

    if (sessionStorage["associatingExternalLogin"]) {
        sessionStorage.removeItem("associatingExternalLogin");

        var externalAccessToken;
        var externalError;
        if (typeof (fragment.error) !== "undefined") {
            externalAccessToken = null;
            externalError = fragment.error;
            cleanUpLocation();
        }
        else if (typeof (fragment.access_token) !== "undefined") {
            externalAccessToken = fragment.access_token;
            externalError = null;
            cleanUpLocation();
        }
        else {
            externalAccessToken = null;
            externalError = null;
            cleanUpLocation();
        }

        dataModel.getUserInfo()
            .done(function (data) {
                if (data.userName) {
                    self.navigateToLoggedIn(data.userName);
                    self.navigateToManage(externalAccessToken, externalError);
                }
                else {
                    self.navigateToLogin();
                }
            })
            .fail(function () {
                self.navigateToLogin();
            });
    }
    else if (typeof (fragment.error) !== "undefined") {
        cleanUpLocation();
        self.navigateToLogin();
        self.errors.push("External login failed.");
    }
    else if (typeof (fragment.access_token) !== "undefined") {
        cleanUpLocation();
        dataModel.externalLoginComplete(fragment.access_token)
            .done(function (data) {
                if (data.userName && data.access_token)
                    self.navigateToLoggedIn(data.userName, data.access_token, false);
                else if (typeof (data.userName) !== "undefined" && typeof (data.loginProvider) !== "undefined")
                    self.navigateToRegisterExternal(data.userName, data.loginProvider, fragment.access_token);
                else
                    self.navigateToLogin();
            })
            .failJSON(function (data) {
                self.navigateToLogin();
                var errors = errorsToArray(data);
                if (errors)
                    self.errors(errors);
            });
    }
    else
        dataModel.getUserInfo()
            .done(function (data) {
                if (data.userName)
                    self.navigateToLoggedIn(data.userName);
                else
                    self.navigateToLogin();
            })
            .fail(function () {
                self.navigateToLogin();
            });
}

// activate knockout
ko.applyBindings(new AppViewModel(new AppDataModel()));
