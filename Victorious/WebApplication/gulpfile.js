/// <binding AfterBuild='less' ProjectOpened='watch-less' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
    //sass = require("gulp-sass"),
    less = require("gulp-less"),
    fs = require("fs"),
    rimraf = require("rimraf");

gulp.task('less', function () {
    gulp.src("Styles/less/style*.less")
    .pipe(less())
    .pipe(gulp.dest("Styles/css/"));
});

gulp.task('watch-less', function () {
    return gulp.watch("Styles/less/*", ['less'])
    .on('change', function (event) {
        console.log("File " + event.path + " was " + event.type + ", running task...");
    });
})