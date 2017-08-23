
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
                $scope.errorUi = 'email-ul nu este completat';
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
                })
                .then(function () {
                    window.location = '/';
                },
                    function (response) {
                        if (response.status === 400)    //badRequest
                            $scope.errorUi = 'something wrong happen, try again later';
                        else if (response.status === 404) //conflict
                            $scope.errorUi = 'email or password wrong';
                        else
                            $scope.errorUi = 'something wrong happen (' + response.status + ')';
                    });;
        };
    }
]);