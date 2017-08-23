app.controller('discoveryController',
    [
        '$scope',
        '$http',
        function ($scope, $http) {
            $http.get('api/album/discovery')
                 .then(function(result) {
                    
                });
        }
    ]);