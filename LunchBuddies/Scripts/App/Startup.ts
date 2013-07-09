/// <reference path="..\typings\angularjs\angular.d.ts" />
/// <reference path="Controllers.ts" />

module LunchBuddies {
    var lunchBuddies: ng.IModule = angular.module("LunchBuddies", []);

    lunchBuddies.config(['$routeProvider', '$provide', function ($routeProvider: ng.IRouteProvider, $provide: ng.auto.IProvideService) {
        $routeProvider.when('/', { templateUrl: '/Templates/Index.html', controller: LunchBuddies.Controllers.Index });
    }]);
}