function AppViewModel(dataModel) {
    var self = this;

    var Views = {
        Loading: 0,
        Todo: 1,
        Login: 2,
        Register: 3,
        RegisterExternal: 4
    };

    var view = ko.observable(Views.Loading);

    // child coordination
    self.error = ko.observable(null);

    self.navigateToLoggedIn = function (userName, accessToken, persistent) {
        self.error(null);

        if (accessToken)
            dataModel.setAccessToken(accessToken, persistent)

        self.user(new UserInfoViewModel(self, userName, dataModel));
        self.navigateToTodo();
    };
    self.navigateToLoggedOff = function () {
        self.error(null);
        dataModel.clearAccessToken();
        self.navigateToLogin();
    };
    self.navigateToTodo = function () {
        self.error(null);
        view(Views.Todo);
    };
    self.navigateToRegister = function () {
        self.error(null);
        view(Views.Register);
    };
    self.navigateToRegisterExternal = function (userName, loginProvider, state) {
        self.error(null);
        view(Views.RegisterExternal);
        self.registerExternal().userName(userName);
        self.registerExternal().loginProvider(loginProvider);
        self.registerExternal().state(state);
    };
    self.navigateToLogin = function () {
        self.error(null);
        self.user(null);
        view(Views.Login);
    };

    // data-bind with
    self.user = ko.observable(null);

    self.loading = ko.computed(function () {
        return view() === Views.Loading;
    });
    self.todo = ko.computed(function () {
        if (view() !== Views.Todo)
            return null;

        return new TodoViewModel(self, dataModel);
    });
    self.login = ko.computed(function () {
        if (view() !== Views.Login)
            return null;

        return new LoginViewModel(self, dataModel);
    });
    self.register = ko.computed(function () {
        if (view() !== Views.Register)
            return null;

        return new RegisterViewModel(self, dataModel);
    });
    self.registerExternal = ko.computed(function () {
        if (view() !== Views.RegisterExternal)
            return null;

        return new RegisterExternalViewModel(self, dataModel);
    });

    function cleanUpLocation() {
        if (typeof (history.pushState) !== "undefined") {
            history.pushState("", document.title, location.pathname);
        }
        else {
            window.location = location.pathname;
        }
    }

    if (inlineData.logOff) {
        cleanUpLocation();
        self.navigateToLoggedOff();
    }
    else if (inlineData.externalLoginProvider && !inlineData.externalLoginState) {
        self.navigateToLogin();
        self.error("External login failed.");
    }
    else if (inlineData.externalLoginProvider)
        dataModel.externalLoginCallback(inlineData.externalLoginState)
            .done(function (data) {
                if (data.userName && data.accessToken)
                    self.navigateToLoggedIn(data.userName, data.accessToken, false);
                else if (typeof (data.userName) !== "undefined" && inlineData.externalLoginProvider && inlineData.externalLoginState)
                    self.navigateToRegisterExternal(data.userName, inlineData.externalLoginProvider, inlineData.externalLoginState);
                else
                    self.navigateToLogin();
            })
            .fail(function () {
                self.navigateToLogin();
            });
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
