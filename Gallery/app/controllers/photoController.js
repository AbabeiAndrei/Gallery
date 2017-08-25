app.controller('photoController',
    [
        '$scope',
        '$location',
        function ($scope, $location) {
            $scope.searchName = '';
            $scope.albums = [
                {
                    name: 'album 1',
                    photos:[]
                },
                {
                    name: 'album 2',
                    photos: []
                },
                {
                    name: 'album 3',
                    photos: []
                }
            ];

            $scope.selectAlbum = function (album) {
                if ($scope.selectedAlbum) {
                    $scope.selectedAlbum.selected = false;
                }
                $scope.selectedAlbum = album;
                $scope.selectedAlbum.selected = true;
            }

            $scope.checkedImages = function() {
                return [];
            }

            $scope.showOptions = function() {
                alert('showOptions');
            }

            $scope.download = function () {
                alert('download');
            }

            $scope.addPhotos = function () {
                alert('addPhotos');
            }

            $scope.addAlbum = function () {
                alert('addAlbum');
            }
        }
    ]);