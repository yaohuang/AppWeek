function UserInfoViewModel(app, name, dataModel) {
    var self = this;

    // data-bind text
    self.name = ko.observable(name);

    // data-bind click
    self.logoff = function () {
        dataModel.logoff()
            .done(function (data) {
                if (data.success) {
                    app.navigateToLoggedOff();
                }
                else {
                    app.error("Logout failed.");
                }
            }).fail(function () {
                app.error("An unknown logout error occurred.");
            });
    };
}
