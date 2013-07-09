/// <reference path="..\typings\angularjs\angular.d.ts" />
module LunchBuddies {
    export module Controllers {

        export function MasterController() {
        }

        export interface IndexViewModel extends ng.IScope {
            message: string;
        }

        export var Index: any = function ($scope: IndexViewModel) {
            $scope.message = "Hello angularjs";
        }
        Index.$inject = ['$scope'];
    }
}