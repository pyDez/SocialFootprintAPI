(function () {
    var app = angular.module("socialFootprintApp", ["LocalStorageModule", "ngRoute", "ngDragDrop", "infinite-scroll"]);
    app.config(function ($routeProvider) {
        $routeProvider
        .when("/login", {
            templateUrl: "/socialFootprintApp/auth/login/login.html",
            controller: "loginController"
        })
        .when("/signup", {
            templateUrl: "socialFootprintApp/auth/signup/signup.html",
            controller: "signupController"
        })
        .when("/associate", {
            templateUrl: "socialFootprintApp/auth/externalLogin/associate.html",
            controller: "associateController"
        })
        .when("/feed/:feedtype?", {
            templateUrl: "socialFootprintApp/feed/feed.html",
            controller: "feedController"
        })
        .when("/profile/:appuserid?", {
            templateUrl: "socialFootprintApp/profile/profile.html",
            controller: "profileController"
         })
        .otherwise({ redirectTo: "/login" });
    });

    app.config(function ($httpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    });

    var serviceBase = 'http://localhost/Service/';
    app.constant('ngAuthSettings', {
        apiServiceBaseUri: serviceBase,
        clientId: 'ngAuthApp'
    });

}());