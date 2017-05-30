/// <binding BeforeBuild='less, js' Clean='js-clean, less-clean' ProjectOpened='watch-less, watch-js' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
    less = require("gulp-less"),
    uglify = require("gulp-uglify"),
    del = require("del"),
    concat = require("gulp-concat"),
    config = {
        "js": [
            "Scripts/**/*.js",
            "!Scripts/**/*.min.js"
        ],
        "less": ["Styles/less/style.less"]
    };

gulp.task("js-clean", function() {
    return del(["Scripts/scripts.js", "Styles/less/style.less"]);
});

gulp.task("less-clean", function () {
    return del(["Scripts/scripts.js", "Styles/less/style.less"]);
});

gulp.task("js", ["js-clean"], function () {
    return gulp.src(config.js)
    .pipe(uglify())
    //.pipe(concat("scripts.min.js"))
    .pipe(gulp.dest("Scripts/scripts.min.js"));
});

gulp.task('less', ["less-clean"], function () {
    return gulp.src(config.less)
    .pipe(less())
    //.pipe(concat("styles.min.less"))
    .pipe(gulp.dest("/Styles/css/style.min.css"));

    //gulp.src("Styles/less/style*.less")
    //.pipe(less())
    //.pipe(gulp.dest("Styles/css/"));
});

gulp.task('watch-less', function () {
    return gulp.watch(config.less, ["less"]);
    //.on('change', function (event) {
    //    console.log("File " + event.path + " was " + event.type + ", running task...");
    //});
});

gulp.task('watch-js', function () {
    return gulp.watch(config.js, ["scripts"]);
});