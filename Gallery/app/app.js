
var app = angular.module('galleryApp', ['ui.bootstrap']);

var modalControllerTemplates = {
    'carouselModal': '/app/partials/modalCarousel.html',
    'addAlbum': '/app/partials/addAlbum.html',
    'editAlbum': '/app/partials/editAlbum.html',
    'optionsPhotos': '/app/partials/optionsPhotos.html',
    'addPhotos': '/app/partials/addPhotos.html'
};

app.provider('routeConfigurator', function () {

    var self = this;

    this.$get = [function () {
        return self;
    }];

    self.modalMapping = function (key) {
        return modalControllerTemplates[key];
    };
});
app.directive('fileReader', function ($q) {
    var slice = Array.prototype.slice;

    return {
        restrict: 'A',
        require: '?ngModel',
        link: function (scope, element, attrs, ngModel) {
            if (!ngModel)
                return;

            ngModel.$render = function () { };

            element.bind('change', function (e) {
                var element = e.target;

                $q.all(slice.call(element.files, 0).map(readFile))
                    .then(function (values) {
                        if (element.multiple) ngModel.$setViewValue(values);
                        else ngModel.$setViewValue(values.length ? values[0] : null);
                    });

                function readFile(file) {
                    var deferred = $q.defer();

                    var reader = new FileReader();
                    reader.onload = function (e) {
                        deferred.resolve({ content: e.target.result, name: file.name });
                    };
                    reader.onerror = function (e) {
                        deferred.reject(e);
                    };
                    reader.readAsDataURL(file);

                    return deferred.promise;
                }

            }); //change

        } //link
    }; //return
});
app.directive('genericModal', ['$rootScope', '$uibModal', 'routeConfigurator', function ($rootScope, $uibModal, routeConfigurator) {
    return {
        restrict: 'A',
        replace: false,
        link: function (scope, element, attributes) {
            $(element).click(function () {
                var instanceOptions = {
                    animation: true
                };

                if (attributes.genericModalTemplateUrl) {
                    instanceOptions.templateUrl = attributes.genericModalTemplateUrl;
                } else {
                    instanceOptions.templateUrl = routeConfigurator.modalMapping(attributes.genericModal);
                }

                instanceOptions.resolve = {
                    settings: function () {
                        if (attributes.genericModalData !== 'undefined' && attributes.genericModalData !== null) {
                            return scope.$eval(attributes.genericModalData);
                        }
                        return 'invalid';
                    }
                };

                if (attributes.size !== undefined) {
                    instanceOptions.size = attributes.size;
                }
                instanceOptions.controller = attributes.genericModal;

                var modalInstance = $uibModal.open(instanceOptions);
                modalInstance.result.then(function () {
                    if (attributes.genericModalSuccess !== undefined && attributes.genericModalSuccess !== null)
                        scope.$eval(attributes.genericModalSuccess);
                }, function () {
                    if (attributes.genericModalCancel !== undefined && attributes.genericModalCancel !== null)
                        scope.$eval(attributes.genericModalCancel);
                });
            });
        }
    };
}]);
app.controller('discoveryController',
    [
        '$scope',
        '$http',
        function ($scope, $http) {
            $http.get('album/discovery')
                 .then(function(result) {
                    $scope.items = result.data;
                }, function() {
                              
                });
        }
    ]);
app.controller('galleryController',
    [
        '$scope',
        '$location',
        function ($scope, $location) {
            $scope.isOnPhoto = $location.absUrl().includes('Photos');
        }
    ]);

app.controller('loginController',
[
    '$scope',
    '$http',
    function($scope, $http) {

        $scope.errorUi = '';
        $scope.email = '';
        $scope.password = '';
        $scope.rememberMe = false;
        $scope.performLogin = function() {
            if ($scope.email.trim().length <= 0) {
                $scope.errorUi = 'email-ul nu este completat';
                return;
            }
            if ($scope.password.trim().length <= 0) {
                $scope.errorUi = 'parola nu este completata';
                return;
            }

            $http.post('Account/Login',
                {
                    email: $scope.email,
                    password: $scope.password,
                    rememberMe: $scope.rememberMe
                })
                .then(function () {
                    window.location = '/Gallery/Photos';
                },
                    function (response) {
                        if (response.status === 400)    //badRequest
                            $scope.errorUi = 'something wrong happen, try again later';
                        else if (response.status === 404) //conflict
                            $scope.errorUi = 'email or password wrong';
                        else
                            $scope.errorUi = 'something wrong happen (' + response.status + ')';
                    });;
        };
    }
]);
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
app.controller('registerController',
[
    '$scope',
    '$http',
    function($scope, $http) {
        $scope.errorUi = '';
        $scope.email = '';
        $scope.fullName = '';
        $scope.password = '';
        $scope.password2 = '';
        $scope.performRegister = function () {
            $scope.errorUi = false;
            if ($scope.fullName.trim().length <= 0) {
                $scope.errorUi = 'numele nu este completat';
                return;
            }
            if ($scope.email.trim().length <= 0) {
                $scope.errorUi = 'email-ul nu este completat';
                return;
            }
            if ($scope.password.trim().length <= 0) {
                $scope.errorUi = 'parola nu este completata';
                return;
            }
            if ($scope.password !== $scope.password2) {
                $scope.errorUi = 'parolele nu corespund';
                return;
            }

            $http.post('Account/Register',
                    {
                        email: $scope.email,
                        password: $scope.password,
                        fullName: $scope.fullName
                    })
                .then(function() {
                        window.location = '/';
                    },
                    function (response) {
                        if (response.status === 400)    //badRequest
                            $scope.errorUi = 'something wrong happen, try again later';
                        else if (response.status === 409) //conflict
                            $scope.errorUi = 'email already exists';
                        else 
                            $scope.errorUi = 'something wrong happen (' + response.status + ')';
                    });
        };
    }
]);
app.controller('addAlbum',
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

            $scope.dismiss = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.album = {
                name: '',
                privacy: 'public',
                photos: []
            };

            $scope.selectPhoto = function(photo) {
                if ($scope.selectedPhoto)
                    $scope.selectedPhoto.selected = false;

                $scope.selectedPhoto = photo;
                $scope.selectedPhoto.selected = true;
            };

            $scope.removePhoto = function(photo) {
                for (var i = 0; i < $scope.album.photos.length; i++) {
                    if ($scope.album.photos[i].internalId === photo.internalId) {
                        $scope.album.photos.splice(i, 1);
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

                $http.post('file/fromBase64',
                        { fileContent: $scope.fileUpload.content, extension: extension })
                    .then(function (result) {
                        var file = result.data;
                        file.name = fileName;
                        file.privacy = 'album';
                        file.date = getToday();
                        $scope.album.photos.push(file);
                        if ($scope.album.photos.length === 1) {
                            $scope.selectPhoto($scope.album.photos[0]);
                        }
                        $scope.inUpload = false;
                    }, function (error) {
                        $scope.inUpload = false;
                        $scope.errorUpload = error.data;
                    });

                $scope.fileUpload = '';
            });

            $scope.createPhotos = function() {
                var photos = [];

                for (var i = 0; i < $scope.album.photos.length; i++) {
                    photos.push({
                        name: $scope.album.photos[i].name,
                        privacy: $scope.album.photos[i].privacy,
                        fileId: $scope.album.photos[i].id
                    });
                }

                return photos;
            };

            $scope.createAlbum = function() {
                return {
                    name: $scope.album.name,
                    privacy: $scope.album.privacy,
                    photos: $scope.createPhotos()
                };
            };

            $scope.create = function() {
                $scope.errorSave = '';
                $scope.inSaving = true;

                $http.post('album',
                        $scope.createAlbum())
                    .then(function() {
                            $scope.inSaving = false;
                            $scope.close();
                        },
                        function(error) {
                            $scope.errorSave = error.data;
                            $scope.inSaving = false;
                        });
            };

            $timeout(function() {
                var dropZone = document.getElementById('drop-zone');
                
                dropZone.ondrop = function(e) {
                    e.preventDefault();
                    this.className = 'upload-container';

                    console.log(e.dataTransfer);
                    var files = e.target.files || e.dataTransfer.files;
                    var file = files[0];
                    var reader = new FileReader();

                    $scope.inUpload = true;

                    $scope.errorUpload = '';

                    reader.onload = function(e) {
                        var text = e.target.result;

                        $http.post('file/fromResult',
                                    {fileContent: text, extension: 'png'})
                            .then(function (result) {
                                var file = result.data;
                                $scope.album.photos.push(file);
                                $scope.inUpload = false;
                            }, function (error) {
                                $scope.inUpload = false;
                                $scope.errorUpload = error.data;
                            });
                    }
                    reader.readAsText(file);
                };

                dropZone.ondragover = function() {
                    this.className = 'upload-container active';
                    return false;
                };

                dropZone.ondragleave = function() {
                    this.className = 'upload-container';
                    return false;
                };
            });
        }
    ]);

function getToday() {
    return new Date();
}
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