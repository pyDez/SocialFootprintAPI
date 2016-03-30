(function (app) {

    var profileController = function ($scope, profileService, $routeParams) {

        var _getMyProfile = function () {
            profileService.getMyProfile().then(function (results) {

                $scope.profile = results.data;
                var tempPost = results.data.Posts.slice(0); //clone des posts
                $scope.profile.Posts = [];
                for (i = 0; i < tempPost.length; i++) {
                    $scope.profile.Posts.push({
                        Post: tempPost[i],
                        User: results.data
                    });
                }
            }, function (error) {
                //alert(error.data.message);
            });
        };


        if (!$routeParams.appuserid) {
            _getMyProfile();
        }
        else {
            profileService.getProfile($routeParams.appuserid).then(function (results) {

                $scope.profile = results.data[0];

            }, function (error) {
                _getMyProfile();
            });
        }
    };

    app.controller("profileController", ["$scope", 'profileService', '$routeParams', profileController]);
}(angular.module("socialFootprintApp")));

