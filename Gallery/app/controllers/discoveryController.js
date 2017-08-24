app.controller('discoveryController',
    [
        '$scope',
        '$http',
        function ($scope, $http) {
            $http.get('album/discovery')
                 .then(function(result) {
                    $scope.items = result.data;
                }, function() {
                              
                });
        }
    ]);