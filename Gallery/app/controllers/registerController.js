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
        $scope.performRegister = function () {
            $scope.errorUi = false;
            if ($scope.fullName.trim().length <= 0) {
                $scope.errorUi = 'numele nu este completat';
                return;
            }
            if ($scope.email.trim().length <= 0) {
                $scope.errorUi = 'email-ul nu este completat';
                return;
            }
            if ($scope.password.trim().length <= 0) {
                $scope.errorUi = 'parola nu este completata';
                return;
            }
            if ($scope.password !== $scope.password2) {
                $scope.errorUi = 'parolele nu corespund';
                return;
            }

            $http.post('Account/Register',
                    {
                        email: $scope.email,
                        password: $scope.password,
                        fullName: $scope.fullName
                    })
                .then(function() {
                        window.location = '/';
                    },
                    function (response) {
                        if (response.status === 400)    //badRequest
                            $scope.errorUi = 'something wrong happen, try again later';
                        else if (response.status === 409) //conflict
                            $scope.errorUi = 'email already exists';
                        else 
                            $scope.errorUi = 'something wrong happen (' + response.status + ')';
                    });
        };
    }
]);