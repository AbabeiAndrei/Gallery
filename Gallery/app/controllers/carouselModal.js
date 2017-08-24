app.controller('carouselModal',
    [
        '$scope',
        '$http',
        '$uibModalInstance',
        function ($scope, $http, $uibModalInstance) {

            $scope.close = function () {
                $uibModalInstance.close(null);
            }

            if (!$scope.$resolve.settings) {
                $scope.close();
            }

            $scope.album = $scope.$resolve.settings || {};

            var id = $scope.album.id;
            $scope.activeItem = 0;

            $http.get('album/' + $scope.album.albumId)
                .then(function (result) {
                    $scope.album = result.data;
                    $scope.album.items = $scope.album.photos || [];

                    for(var i = 0 ; i < $scope.album.items.length ; i++){
                        if ($scope.album.items[i].id === id) {
                            $scope.activeItem = i;
                            break;
                        }
                    }
                }, function () {

                });
        }
]);