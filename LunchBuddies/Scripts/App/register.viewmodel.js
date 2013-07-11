function RegisterViewModel(app, dataModel) {
    var self = this;

    // data-bind value
    self.userName = ko.observable("");
    self.password = ko.observable("");
    self.confirmPassword = ko.observable("");
    self.errors = ko.observableArray();

    // data-bind visible
    self.registeringVisible = ko.observable(false);
    self.unknownErrorVisible = ko.observable(false);

    // data-bind enable
    self.registering = ko.observable(false);

    // data-bind click
    self.registerClick = function () {
        self.unknownErrorVisible(false);
        self.errors.removeAll();
        self.registering(true);
        dataModel.register({
            userName: self.userName(),
            password: self.password(),
            confirmPassword: self.confirmPassword()
        }).done(function (data) {
            self.registering(false);
            if (data.errors)
                self.errors(data.errors);
            else if (data.userName && data.accessToken)
                app.navigateToLoggedIn(data.userName, data.accessToken, false);
            else
                self.unknownErrorVisible(true);
        }).fail(function () {
            self.registering(false);
            self.unknownErrorVisible(true);
        });
    };

    self.loginClick = function () {
        app.navigateToLogin();
    };
}
