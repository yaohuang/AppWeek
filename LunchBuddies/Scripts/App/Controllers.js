var LunchBuddies;
(function (LunchBuddies) {
    (function (Controllers) {
        function MasterController() {
        }
        Controllers.MasterController = MasterController;

        Controllers.Index = function ($scope) {
            $scope.message = "Hello angularjs";
        };
        Controllers.Index.$inject = ['$scope'];
    })(LunchBuddies.Controllers || (LunchBuddies.Controllers = {}));
    var Controllers = LunchBuddies.Controllers;
})(LunchBuddies || (LunchBuddies = {}));
