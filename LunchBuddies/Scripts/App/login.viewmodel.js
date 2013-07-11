function LoginViewModel(app, dataModel) {
    var self = this;

    // implementation details
    var validationTriggered = ko.observable(false);

    // data-bind foreach
    self.externalLoginProviders = ko.observableArray();

    // data-bind value
    self.userName = ko.observable("").extend({
        required: {
            enabled: validationTriggered,
            message: "The User name field is required."
        }
    });
    self.password = ko.observable("").extend({
        required: {
            enabled: validationTriggered,
            message: "The Password field is required."
        }
    });
    self.rememberMe = ko.observable(false);
    self.errors = ko.observableArray();

    // data-bind visible
    self.loggingInVisible = ko.observable(false);
    self.unknownErrorVisible = ko.observable(false);
    self.externalLoginVisible = ko.computed(function () {
        return self.externalLoginProviders().length > 0;
    });

    // data-bind enable
    self.loggingIn = ko.observable(false);

    // data-bind click
    self.loginClick = function () {
        self.unknownErrorVisible(false);
        self.errors.removeAll();
        validationTriggered(true);
        if (self.userName.hasError() || self.password.hasError())
            return;
        self.loggingIn(true);

        dataModel.login({
            userName: self.userName(),
            password: self.password(),
            rememberMe: self.rememberMe()
        }).done(function (data) {
            self.loggingIn(false);
            if (data.errors)
                self.errors(data.errors);
            else if (data.userName && data.accessToken)
                app.navigateToLoggedIn(data.userName, data.accessToken, self.rememberMe());
            else
                self.unknownErrorVisible(true);
        }).fail(function () {
            self.loggingIn(false);
            self.unknownErrorVisible(true);
        });
    };

    self.registerClick = function () {
        app.navigateToRegister();
    };

    // initialization: inlineData is defined inline in Index.cshtml
    var externalLoginProviders = inlineData.externalLoginProviders;
    for (var i = 0; i < externalLoginProviders.length; i++) {
        self.externalLoginProviders.push(new ExternalLoginProviderModel(externalLoginProviders[i]));
    }
}

function ExternalLoginProviderModel(data) {
    var self = this;

    // data-bind text
    self.name = ko.observable(data.name);

    // data-bind click
    self.loginClick = function () {
        window.location = data.url;
    };
}
