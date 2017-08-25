
var app = angular.module('galleryApp', ['ui.bootstrap']);

var modalControllerTemplates = {
    'carouselModal': '/app/partials/modalCarousel.html'
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
                    window.location = '/';
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