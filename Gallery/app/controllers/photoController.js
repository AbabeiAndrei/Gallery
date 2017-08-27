app.controller('photoController',
    [
        '$scope',
        '$http',
        function ($scope, $http) {
            $scope.searchName = '';
            $scope.albums = [];

            $scope.refresh = function() {
                $http.get('album').then(
                    function(result) {
                        $scope.albums = result.data;

                        if (!$scope.selectAlbum)
                            return;

                        for(var i = 0 ; i < $scope.albums.length ; i++)
                            if ($scope.albums[i].id === $scope.selectAlbum.id)
                                return;

                        $scope.selectAlbum(null);
                    },
                    function(error) {
                        console.log(error);
                    });
            }

            $scope.selectAlbum = function (album) {
                if ($scope.selectedAlbum) {
                    $scope.selectedAlbum.selected = false;
                }
                $scope.selectedAlbum = album;

                if ($scope.selectedAlbum) {
                    $scope.selectedAlbum.selected = true;
                }

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
            };

            $scope.download = function () {
                var url = 'album/download/' + $scope.selectAlbum.id;

                var selectedImages = $scope.checkedImages();

                if (selectedImages.length <= 0) {
                    selectedImages = $scope.selectedAlbum.photos;
                }

                if (selectedImages.length > 0) {
                    $http({
                        method: 'POST',
                        url: 'photo/download',
                        cache: false,
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        data: { photoIds: selectedImages.map(function(item) { return item.id; }) }
                    }).then(function (result) {
                            window.open(result.data);
                        },
                        function (error) {
                            console.log(error.data);
                        });
                    return;
                }

                $http.get(url)
                    .then(function () { }, function (error) { console.log(error.data); });
            };

            $scope.addPhotos = function() {
                alert('addPhotos');
            };

            $scope.addAlbum = function() {
                alert('addAlbum');
            };

            $scope.showMenu = function (album) {
                album.showMenu = !album.showMenu;
            };

            $scope.deleteAlbum = function(album) {
                var result = confirm('Esti sigur ca vrei sa stergi albumul ' + album.name + '? Actiunea este ireversibila!');

                if (!result)
                    return;

                $http.delete('album/' + album.id)
                     .then(function() { $scope.refresh(); },
                           function(error) {
                                console.log(error.data);
                           });
            };

            $scope.generateOptionsPhoto = function() {
                return {
                    photoIds: $scope.checkedImages().map(function(item) { return item.id }),
                    selectedAlbum: angular.copy($scope.selectedAlbum),
                    albums: $scope.albums,
                    selectedPrivacy: 'album'
                };
            };

            $scope.generateUploadPhoto = function() {
                return {
                    selectedAlbum: angular.copy($scope.selectedAlbum),
                    albums: $scope.albums
                };
            };

            $scope.refresh();
        }
    ]);