function ConfirmRegistrationViewModel(app, dataModel) {
    var self = this;

    // data-bind value
    self.token = ko.observable();
    self.userName = ko.observable();

    self.email = ko.observable();
    self.office = ko.observable();
    self.telephone = ko.observable();
    self.department = ko.observable();
    self.title = ko.observable();
    self.pictureUrl = ko.observable();

    self.password = ko.observable();
    self.confirmPassword = ko.observable();
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
        dataModel.createUser({
            token: self.token(),
            password: self.password()
        }).done(function (data) {
            self.registering(false);
            if (data.errors)
                self.errors(data.errors);
            else if (data.userName && data.access_token)
                app.navigateToLoggedIn(data.userName, data.access_token, false);
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
