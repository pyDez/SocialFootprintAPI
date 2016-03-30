(function () {

    var feedService = function ($http, $q, ngAuthSettings) {

        

            var serviceBase = ngAuthSettings.apiServiceBaseUri;

            var feedFactory = {};

            var _getFeed = function () {

                return $http.get(serviceBase + 'api/currentuser/NewsFeed').then(function (results) {
                    return results;
                });
            };
            var _getUpFeed = function () {

                return $http.get(serviceBase + 'api/currentuser/UpPostsFeed').then(function (results) {
                    return results;
                });
            };
            var _getDownFeed = function () {

                return $http.get(serviceBase + 'api/currentuser/DownPostsFeed').then(function (results) {
                    return results;
                });
            };
            var _getNext = function (nextPageUrl) {

                return $http.get(nextPageUrl).then(function (results) {
                    return results;
                });
            };
            var _vote = function (vote) {

                var deferred = $q.defer();

                $http.post(ngAuthSettings.apiServiceBaseUri + 'api/currentuser/vote', vote).success(function (response) {
                    deferred.resolve(response);

                });
                return deferred.promise;
            };


            feedFactory.vote = _vote;
            feedFactory.getFeed = _getFeed;
            feedFactory.getDownFeed = _getDownFeed;
            feedFactory.getUpFeed = _getUpFeed;
            feedFactory.getNext = _getNext;

            return feedFactory;
    };

    var module = angular.module("socialFootprintApp");
    module.factory('feedService', ['$http', '$q', 'ngAuthSettings', feedService]);
}());