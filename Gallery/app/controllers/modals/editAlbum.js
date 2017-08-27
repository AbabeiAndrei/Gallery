app.controller('editAlbum',
    [
        '$scope',
        '$http',
        '$uibModalInstance',
        function ($scope, $http, $uibModalInstance) {
             
            $scope.close = function () {
                $uibModalInstance.close(null);
            }

            $scope.dismiss = function () {
                $uibModalInstance.dismiss('cancel');
            };

            if (!$scope.$resolve.settings) {
                $scope.close();
            }

            $scope.album = angular.copy($scope.$resolve.settings || {});
            $scope.albumName = $scope.album.name;

            $scope.generateAlbum = function() {
                return {
                    name: $scope.album.name,
                    privacy: $scope.album.privacy
                };
            };

            $scope.save = function() {
                $scope.inSaving = true;
                $scope.errorSave = '';

                $http.put('album/' + $scope.album.id, $scope.generateAlbum())
                     .then(function() {
                            $scope.inSaving = false;
                            $scope.close();
                        },
                        function (error) {
                            $scope.inSaving = false;
                            $scope.errorSave = error.data;
                        });
            };
        }
    ]);