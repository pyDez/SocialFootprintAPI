(function () {

    var profileService = function ($http, ngAuthSettings) {

        

            var serviceBase = ngAuthSettings.apiServiceBaseUri;

            var feedFactory = {};

            var _getProfile = function (appUserId) {

                return $http.get(serviceBase + 'api/user/'+ appUserId).then(function (results) {
                    return results;
                });
            };
            var _getMyProfile = function () {

                return $http.get(serviceBase + 'api/CurrentUser').then(function (results) {
                    return results;
                });
            };
            

            feedFactory.getProfile = _getProfile;
            feedFactory.getMyProfile = _getMyProfile;
            return feedFactory;
    };

    var module = angular.module("socialFootprintApp");
    module.factory('profileService', ['$http', 'ngAuthSettings', profileService]);
}());