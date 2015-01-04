module.exports = function (grunt) {

  grunt.initConfig({

    pkg: grunt.file.readJSON('package.json'),

    handlebarslayouts: {
      "docs": {
        files: {
          "docs/*.html": "src/pages/*.hbs"
        },
        options: {
          partials: "src/partials/*.hbs"
        }
      },
      "index": {
        files: {
          "index.html": "src/pages/index.hbs"
        },
        options: {
          partials: "src/partials/*.hbs"
        }
      }
    }
  });

  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-handlebars-layouts');

  grunt.registerTask('default' [ 'handlebarslayouts' ]);

};