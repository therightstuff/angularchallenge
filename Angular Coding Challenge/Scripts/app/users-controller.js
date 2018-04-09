angular.module('AngularCodingChallenge', [])
    .controller('UsersController', function ($scope, $http) {
        $scope.username = "admin";
        $scope.password = "secure password admin";
        $scope.authenticated = false;
        $scope.jwtToken = null;
        $scope.status = "not authenticated";
        $scope.working = false;

        $scope.users = [];
        $scope.indices = [];
        $scope.tickerQuery = '';
        $scope.foundIndex = {};

        $scope.authenticate = function () {
            $scope.working = true;
            $scope.authenticated = false;

            // validate $scope.username / .password

            $http.post('/api/login', { 'username': this.username, 'password': this.password }).then(function (res, status, headers, config) {
                $scope.authenticated = true;
                $scope.jwtToken = res.data;
                $scope.status = 'authenticated';
                document.title = 'Angular Coding Challenge - ' + $scope.username;
                $scope.working = false;
            }).catch(function (res, status, headers, config) {
                $scope.status = "error authenticating: " + res.data.Message;
                $scope.working = false;
            });
        };

        $scope.getHeaders = function () {
            return {
                'Authorization': this.jwtToken
            };
        }

        $scope.getUserList = function () {
            $scope.working = true;
            $http.get('/api/users', { headers: $scope.getHeaders() }).then(function (res, status, headers, config) {
                $scope.users = res.data;
                $scope.working = false;
            }).catch(function (res, status, headers, config) {
                console.log(res);
                $scope.status = "error retrieving user list (see console for details)";
                $scope.working = false;
            });
        };

        $scope.getIndexList = function () {
            $scope.working = true;
            $http.get('/api/indices', { headers: $scope.getHeaders() }).then(function (res, status, headers, config) {
                $scope.indices = res.data;
                $scope.working = false;
            }).catch(function (res, status, headers, config) {
                console.log(res);
                $scope.status = "error retrieving index list  (see console for details)";
                $scope.working = false;
            });
        };

        $scope.queryIndex = function () {
            $scope.working = true;
            $http.get('/api/indices/' + this.tickerQuery, { headers: $scope.getHeaders() }).then(function (res, status, headers, config) {
                $scope.foundIndex = res.data;
                $scope.working = false;
            }).catch(function (res, status, headers, config) {
                console.log(res);
                $scope.foundIndex = {};
                $scope.status = "error retrieving index (see console for details)";
                $scope.working = false;
            });
        };

    });