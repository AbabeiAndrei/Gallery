app.controller('optionsPhotos',
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

            var data = $scope.$resolve.settings;

            $scope.photoIds = data.photoIds;
            $scope.selectedAlbum = data.selectedAlbum;
            $scope.albums = data.albums;
            $scope.selectedPrivacy = data.selectedPrivacy;

            $scope.genearteRequest = function() {
                return {
                    photoIds: $scope.photoIds,
                    albumId: $scope.selectedAlbum.id,
                    privacy: $scope.selectedPrivacy
                }
            };

            $scope.selectAlbum = function(album) {
                $scope.selectedAlbum = album;
            };

            $scope.save = function () {
                $scope.inSaving = true;
                $scope.errorSave = '';

                $http.put('photo/all', $scope.genearteRequest())
                    .then(function () {
                            $scope.inSaving = false;
                            $scope.close();
                        },
                        function (error) {
                            $scope.inSaving = false;
                            $scope.errorSave = error.data;
                        });
            };

            $scope.delete = function () {
                var result = confirm('Esti sigur ca vrei sa stergi toate fotografiile selectate? Actiunea este ireversibila');
                if (!result)
                    return;

                $scope.inSaving = true;
                $scope.errorSave = '';

                $http({
                    method: 'DELETE',
                    url: 'photo/all',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    data: $scope.genearteRequest()
                }).then(function() {
                        $scope.inSaving = false;
                        $scope.close();
                    },
                    function(error) {
                        $scope.inSaving = false;
                        $scope.errorSave = error.data;
                    });
            }
        }
    ]);