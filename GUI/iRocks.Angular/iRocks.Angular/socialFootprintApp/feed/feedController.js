(function (app) {

    var feedController = function ($scope, feedService, $routeParams) {
        $scope.feeds = [];
        votedPosts = [];

        if ($routeParams.feedtype == "top")
        {
            feedService.getUpFeed().then(function (results) {

                $scope.feeds = results.data.Results;
                $scope.nextPageUrl = results.data.NextPageUrl;

            }, function (error) {
                //alert(error.data.message);
            });
        }
        else if ($routeParams.feedtype == "flop") {
            feedService.getDownFeed().then(function (results) {

                $scope.feeds = results.data.Results;
                $scope.nextPageUrl = results.data.NextPageUrl;

            }, function (error) {
                //alert(error.data.message);
            });
        }
        else {
            feedService.getFeed().then(function (results) {

                $scope.feeds = results.data.Results;
                $scope.nextPageUrl = results.data.NextPageUrl;

            }, function (error) {
                //alert(error.data.message);
            });
        }

        var _vote = function (event, ui, AWin, index) {
            var aVote = {
                DownPostId: "",
                UpPostId: ""
            };

            if (AWin) {
                aVote.UpPostId = $scope.voteData.PostAId
                aVote.DownPostId = $scope.voteData.PostBId
            }
            else
            {
                aVote.UpPostId = $scope.voteData.PostBId
                aVote.DownPostId = $scope.voteData.PostAId
            }
            votedPosts.push(index);
            feedService.vote(aVote).then(function (response) {

            }, function (error) {
                //alert(error.data.message);
            });
        };

        var _setDropZone = function (event, ui, postA, postB, index) {
            $scope.voteData = {
                PostAId: postA,
                PostBId: postB,
                Index: index
            };
        };
        var _clearDropZone = function () {
            $scope.voteData.Index = -1;
        };

        var _isVoted = function (index) {

            if ($.inArray(index, votedPosts) != -1) {
                return true;
            }
            return false;
            

        };
        var tempNextPageUrl = "";
        _loadMore = function () {
            if (tempNextPageUrl != $scope.nextPageUrl) {
                tempNextPageUrl = $scope.nextPageUrl;
                feedService.getNext($scope.nextPageUrl).then(function (results) {

                    $scope.feeds = $scope.feeds.concat(results.data.Results);
                    $scope.nextPageUrl = results.data.NextPageUrl;

                }, function (error) {
                    //alert(error.data.message);
                });
            }
        };




        $scope.voteData = {
            PostAId: "",
            PostBId: "",
            Index: -1
        };

        $scope.vote = _vote;
        $scope.setDropZone = _setDropZone;
        $scope.isVoted = _isVoted;
        $scope.isProfile = false;
        $scope.clearDropZone = _clearDropZone;
        $scope.loadMore = _loadMore;
        
    };

    app.controller("feedController", ["$scope", 'feedService', '$routeParams', feedController]);

   
}(angular.module("socialFootprintApp")));

