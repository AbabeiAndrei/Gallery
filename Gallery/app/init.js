
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