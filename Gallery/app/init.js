
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