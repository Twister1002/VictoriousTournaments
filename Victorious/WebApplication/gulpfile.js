/// <binding BeforeBuild='less, js' Clean='less-clean, js-clean' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
    less = require("gulp-less"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    concat = require("gulp-concat"),
    del = require("del"),
    config = {
        "js": [
            "Scripts/**/*.js",
            "lib/jquery-timepicker/jquery.ui.timepicker.js",
            "!Scripts/**/*.min.js"
        ],
        "less": [
            "Styles/less/style.less"
        ],
        "css": [
            "lib/jquery-ui/themes/base/jquery-ui.min.css",
            "lib/jquery-timepicker/jquery.ui.timepicker.css",
            "Styles/css/main.min.css"
        ]
    };

gulp.task("js-clean", function() {
    return del(["Scripts/scripts.min.js"]);
});

gulp.task("less-clean", function () {
    return del(["Styles/css/styles.min.less"]);
});

gulp.task("js", ["js-clean"], function () {
    return gulp.src(config.js)
    .pipe(uglify())
    .pipe(concat("scripts.min.js"))
    .pipe(gulp.dest("Scripts/"));
});

gulp.task("less", ["less-clean"], function () {
    return gulp.src(config.less)
        .pipe(concat("main.min.css"))
        .pipe(less())
        .pipe(gulp.dest("Styles/css/"));
});

gulp.task("bundle:css", ["less"], function () {
    return gulp.src(config.css)
    .pipe(cssmin())
    .pipe(concat("style.min.css"))
    .pipe(gulp.dest("Styles/css/"))
});

gulp.task('watch-less', function () {
    return gulp.watch("Styles/less/**/*.less", ["bundle:css"]);
});

gulp.task('watch-js', function () {
    return gulp.watch(config.js, ["js"]);
});