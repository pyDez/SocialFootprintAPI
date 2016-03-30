(function () {

    associateController = function ($scope, $location, $timeout, authService) {

        $scope.savedSuccessfully = false;
        $scope.message = "";

        $scope.registerData = {
            userName: authService.externalAuthData.userName,
            provider: authService.externalAuthData.provider,
            externalAccessToken: authService.externalAuthData.externalAccessToken
        };

        $scope.registerExternal = function () {

            authService.registerExternal($scope.registerData).then(function (response) {

                $scope.savedSuccessfully = true;
                $scope.message = "User has been registered successfully, you will be redicted to orders page in 2 seconds.";
                authService.fillAuthData();
                startTimer();

            },
              function (response) {
                  var errors = [];
                  for (var key in response.modelState) {
                      errors.push(response.modelState[key]);
                  }
                  $scope.message = "Failed to register user due to:" + errors.join(' ');
              });
        };

        var startTimer = function () {
            var timer = $timeout(function () {
                $timeout.cancel(timer);
                $location.path('/feed');
            }, 2000);
        }



    };


    var module = angular.module("socialFootprintApp");
    module.controller("associateController", ['$scope', '$location', '$timeout', 'authService', associateController]);
}());