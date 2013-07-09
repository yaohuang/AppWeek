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
var LunchBuddies;
(function (LunchBuddies) {
    var lunchBuddies = angular.module("LunchBuddies", []);

    lunchBuddies.config([
        '$routeProvider',
        '$provide',
        function ($routeProvider, $provide) {
            $routeProvider.when('/', { templateUrl: '/Templates/Index.html', controller: LunchBuddies.Controllers.Index });
        }
    ]);
})(LunchBuddies || (LunchBuddies = {}));
