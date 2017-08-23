app.controller('galleryController',
    [
        '$scope',
        '$location',
        function ($scope, $location) {
            $scope.isOnPhoto = $location.url().contains('photos');
        }
    ]);