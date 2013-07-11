function RegisterViewModel(app, dataModel) {
    var self = this;

    // data-bind value
    self.email = ko.observable();
    self.error = ko.observable();

    // data-bind visible
    self.registeringVisible = ko.observable(false);
    self.unknownErrorVisible = ko.observable(false);
    self.done = ko.observable(false);

    // data-bind enable
    self.registering = ko.observable(false);

    // data-bind click
    self.registerClick = function () {
        self.unknownErrorVisible(false);
        self.error("");
        self.registering(true);
        dataModel.register(self.email()).done(function (data) {
            self.registering(false);
            if (data.error)
                self.error(data.error);
            else
                self.done(true);
        }).fail(function () {
            self.registering(false);
            self.unknownErrorVisible(true);
        });
    };

    self.loginClick = function () {
        app.navigateToLogin();
    };
}
