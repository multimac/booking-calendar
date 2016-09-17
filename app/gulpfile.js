var gulp = require("gulp"),
    sass = require("gulp-sass"),
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


gulp.task("html", function () {
    return gulp.src("./html/**/*.html", { base: "./html/" })
        .pipe(gulp.dest("./static/lib/"));
});

gulp.task("styles", function () {
    return gulp.src("./styles/**/*.scss", { base: "./styles/" })
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./static/styles/"));
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

gulp.task("watch", ["html", "styles", "typescript"], function () {
    gulp.watch("html/**/*.html", ["html"]);
    gulp.watch("styles/**/*.scss", ["styles"]);
    gulp.watch("lib/**/*.ts", ["typescript"]);
})