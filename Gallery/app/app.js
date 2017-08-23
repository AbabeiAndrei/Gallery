
var app = angular.module('galleryApp', []);

app.controller('loginController',
[
    '$scope',
    '$http',
    function($scope, $http) {

        $scope.errorUi = '';
        $scope.email = '';
        $scope.password = '';
        $scope.rememberMe = false;
        $scope.performLogin = function() {
            if ($scope.email.trim().length <= 0) {
                $scope.errorUi = 'numele nu este completat';
                return;
            }
            if ($scope.password.trim().length <= 0) {
                $scope.errorUi = 'parola nu este completata';
                return;
            }

            $http.post('Account/Login',
                {
                    email: $scope.email,
                    password: $scope.password,
                    rememberMe: $scope.rememberMe
                });
        };
    }
]);
app.controller('registerController',
[
    '$scope',
    '$http',
    function($scope, $http) {
        $scope.errorUi = '';
        $scope.email = '';
        $scope.fullName = '';
        $scope.password = '';
        $scope.password2 = '';
        $scope.performLogin = function () {
            if ($scope.email.trim().length <= 0) {
                $scope.errorUi = 'email-ul nu este completat';
                return;
            }
            if ($scope.password.trim().length <= 0) {
                $scope.errorUi = 'parola nu este completata';
                return;
            }

            $http.post('Account/Register',
                {
                    email: $scope.email,
                    password: $scope.password,
                    fullName: $scope.fullName
                });
        };
    }
]);