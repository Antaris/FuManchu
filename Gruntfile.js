module.exports = function (grunt) {

  var rootContext = {
    basePath: "/FuManchu/",
    bowerPath: "/FuManchu/bower_components/",
    docsPath: "/FuManchu/docs/"
  };

  grunt.initConfig({

    pkg: grunt.file.readJSON('package.json'),

    handlebarslayouts: {
      "docs": {
        files: {
          "docs/*.html": "src/pages/*.hbs"
        },
        options: {
          context: rootContext,
          partials: "src/partials/*.hbs"
        }
      },
      "index": {
        files: {
          "index.html": "src/pages/index.hbs"
        },
        options: {
          context: rootContext,
          partials: "src/partials/*.hbs"
        }
      }
    },

    watch: {
      docs: {
        files: "src/**/*.hbs",
        tasks: [ "handlebarslayouts" ]
      }
    }
  });

  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-handlebars-layouts');

  grunt.registerTask('default' [ 'handlebarslayouts' ]);

};