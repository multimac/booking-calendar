var gulp = require("gulp"),
    sourcemaps = require("gulp-sourcemaps"),
    ts = require("gulp-typescript");

var del = require("del");

gulp.task("default", ["prod"]);
gulp.task("prod", ["typescript"]);

gulp.task("clean", function () {
    return del([
        "node_modules", "typings",
            
        "static/**", "!static",
        "!static/system-config.js"
    ]);
});


var tsProject = ts.createProject("tsconfig.json", {
    typescript: require("typescript"), rootDir: "."
});
gulp.task("typescript", function () {
    var typescript = tsProject.src()
        .pipe(sourcemaps.init())
        .pipe(ts(tsProject));

    return typescript.js
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./static/"));
});