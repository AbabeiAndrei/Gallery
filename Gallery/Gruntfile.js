/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
        clean: ['app/lib/*', 'temp'],
        concat: {
            all: {
                src: [
                    'app/init.js',
                    'app/services/*.js',
                    'app/filters/*.js',
                    'app/directives/*.js',
                    'app/controllers/*.js'
                ],
                dest: 'app/app.js'
            }
        },
        watch: {
            files: [
                'app/services/*.js',
                'app/filters/*.js',
                'app/directives/*.js',
                'app/controllers/*.js'
            ],
            tasks: ['all']
        }
    });

    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.registerTask('all', ['clean', 'concat'/*, 'jshint', 'uglify'*/]);
};