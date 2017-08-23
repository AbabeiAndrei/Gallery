app.controller('galleryController',
    [
        '$scope',
        '$location',
        function ($scope, $location) {
            $scope.isOnPhoto = $location.absUrl().includes('Photos');
        }
    ]);