app.controller('photoController',
    [
        '$scope',
        '$http',
        '$filter',
        function ($scope, $http, $filter) {
            $scope.searchName = '';

            $http.get('album').then(
                function(result) {
                    $scope.albums = result.data;
                },
                function(error) {
                    console.log(error);
                });

            $scope.selectAlbum = function (album) {
                if ($scope.selectedAlbum) {
                    $scope.selectedAlbum.selected = false;
                }
                $scope.selectedAlbum = album;
                $scope.selectedAlbum.selected = true;

                $scope.clearSelection();
            }

            $scope.checkedImages = function () {
                if (!$scope.selectedAlbum || !$scope.selectedAlbum.photos || $scope.selectedAlbum.photos.length <= 0) {
                    return [];
                }

                var items = [];

                for (var i = 0; i < $scope.selectedAlbum.photos.length; i++) {
                    if ($scope.selectedAlbum.photos[i].checked) {
                        items.push($scope.selectedAlbum.photos[i]);
                    }
                }

                return items;
            }

            $scope.clearSelection = function() {
                if (!$scope.selectedAlbum || !$scope.selectedAlbum.photos || $scope.selectedAlbum.photos.length <= 0) {
                    return;
                }

                for (var i = 0; i < $scope.selectedAlbum.photos.length; i++) {
                    if ($scope.selectedAlbum.photos[i].checked) {
                        $scope.selectedAlbum.photos[i].checked = false;
                    }
                }
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