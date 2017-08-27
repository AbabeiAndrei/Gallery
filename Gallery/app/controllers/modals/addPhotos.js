app.controller('addPhotos',
    [
        '$scope',
        '$http',
        '$uibModalInstance',
        '$timeout',
        function ($scope, $http, $uibModalInstance, $timeout) {

            $scope.fileUpload = '';
            $scope.inSaving = false;
            $scope.close = function () {
                $uibModalInstance.close(null);
            }

            $scope.dismiss = function() {
                $uibModalInstance.dismiss('cancel');
            };

            var data = $scope.$resolve.settings;

            $scope.selectedAlbum = data.selectedAlbum;
            $scope.albums = data.albums;
            $scope.photos = [];

            $scope.selectPhoto = function (photo) {
                if ($scope.selectedPhoto)
                    $scope.selectedPhoto.selected = false;

                $scope.selectedPhoto = photo;
                $scope.selectedPhoto.selected = true;
            };

            $scope.removePhoto = function (photo) {
                for (var i = 0; i < $scope.photos.length; i++) {
                    if ($scope.photos[i].internalId === photo.internalId) {
                        $scope.photos.splice(i, 1);
                        $scope.selectedPhoto = '';
                        break;
                    }
                }
            };

            $scope.$watch('fileUpload', function () {
                if (!$scope.fileUpload || ($scope.fileUpload.content || '').trim().length <= 0)
                    return;

                $scope.inUpload = true;

                var re = /(?:\.([^.]+))?$/;

                var fileName = re.exec($scope.fileUpload.name);

                var accepedExtensions = ['png', 'jpg', 'jpeg', 'bmp'];

                if (fileName.length !== 2) {
                    $scope.errorUpload = 'incorect image extension';
                    return;
                }
                var extension = fileName[1];
                if (accepedExtensions.indexOf(extension) < 0) {
                    $scope.errorUpload = 'extension not allowed';
                    return;
                }

                fileName = $scope.fileUpload.name.replace(fileName[0], '');

                $http.post('file/fromBase64', { fileContent: $scope.fileUpload.content, extension: extension })
                    .then(function (result) {
                        var file = result.data;
                        file.name = fileName;
                        file.privacy = 'album';
                        file.date = getToday();
                        $scope.photos.push(file);
                        if ($scope.photos.length === 1) {
                            $scope.selectPhoto($scope.photos[0]);
                        }
                        $scope.inUpload = false;
                    },
                          function (error) {
                            $scope.inUpload = false;
                            $scope.errorUpload = error.data;
                        });

                $scope.fileUpload = '';
            });

            $scope.createPhotos = function () {
                var photos = [];

                for (var i = 0; i < $scope.photos.length; i++) {
                    photos.push({
                        name: $scope.photos[i].name,
                        privacy: $scope.photos[i].privacy,
                        fileId: $scope.photos[i].id
                    });
                }

                return {
                    photos: photos,
                    albumId: $scope.selectedAlbum.id
                };
            };

            $scope.upload = function () {
                $scope.errorSave = '';
                $scope.inSaving = true;

                $http.post('photo/all', $scope.createPhotos())
                    .then(function () {
                            $scope.inSaving = false;
                            $scope.close();
                        },
                        function (error) {
                            $scope.errorSave = error.data;
                            $scope.inSaving = false;
                        });
            };

            $timeout(function () {
                var dropZone = document.getElementById('drop-zone');

                dropZone.ondrop = function (e) {
                    e.preventDefault();
                    this.className = 'upload-container';

                    console.log(e.dataTransfer);
                    var files = e.target.files || e.dataTransfer.files;
                    var file = files[0];
                    var reader = new FileReader();

                    $scope.inUpload = true;

                    $scope.errorUpload = '';

                    reader.onload = function (e) {
                        var text = e.target.result;

                        $http.post('file/fromResult',
                                { fileContent: text, extension: 'png' })
                            .then(function (result) {
                                var file = result.data;
                                $scope.photos.push(file);
                                $scope.inUpload = false;
                            }, function (error) {
                                $scope.inUpload = false;
                                $scope.errorUpload = error.data;
                            });
                    }
                    reader.readAsText(file);
                };

                dropZone.ondragover = function () {
                    this.className = 'upload-container active';
                    return false;
                };

                dropZone.ondragleave = function () {
                    this.className = 'upload-container';
                    return false;
                };
            });
        }
    ]);

function getToday() {
    return new Date();
}