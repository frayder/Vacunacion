const { series, src, dest, parallel, watch } = require("gulp");

const autoprefixer = require("gulp-autoprefixer");
const concat = require("gulp-concat");
const CleanCSS = require("gulp-clean-css");
const npmdist = require("gulp-npm-dist");
const rename = require("gulp-rename");
const rtlcss = require("gulp-rtlcss");
const sourcemaps = require("gulp-sourcemaps");
const sass = require("gulp-sass")(require("sass"));
const uglify = require("gulp-uglify");


const paths = {
    baseDistAssets: "wwwroot/",  // build assets directory
    baseSrcAssets: "wwwroot/",   // source assets directory
};

const vendor = function () {
    const out = paths.baseDistAssets + "vendor/";
    return src(npmdist(), { base: "./node_modules" })
        .pipe(rename(function (path) {
            path.dirname = path.dirname.replace(/\/dist/, '').replace(/\\dist/, '');
        }))
        .pipe(dest(out));
};

const plugin = function () {
    const outCSS = paths.baseDistAssets + "css/";

    // vendor.min.css
    src([
        // Form Advance Plugin
        paths.baseDistAssets + "vendor/flatpickr/flatpickr.min.css",
        paths.baseDistAssets + "vendor/select2/css/select2.min.css",
        paths.baseDistAssets + "vendor/choices.js/public/assets/styles/choices.min.css",

    ])
        .pipe(concat("vendor.css"))
        .pipe(CleanCSS())
        .pipe(rename({ suffix: ".min" }))
        .pipe(dest(outCSS));

    const outjs = paths.baseDistAssets + "js/";

    // vendor.min.js
    return src([
        paths.baseDistAssets + "vendor/jquery/jquery.min.js",
        paths.baseDistAssets + "vendor/bootstrap/js/bootstrap.bundle.min.js",
        paths.baseDistAssets + "vendor/iconify-icon/iconify-icon.min.js",
        paths.baseDistAssets + "vendor/simplebar/simplebar.min.js",

        // Form Advance Plugin
        paths.baseDistAssets + "vendor/flatpickr/flatpickr.min.js",  // Flatpickr
        paths.baseDistAssets + "vendor/select2/js/select2.min.js",   // select2
        paths.baseDistAssets + "vendor/inputmask/inputmask.min.js",  // inputmask
        paths.baseDistAssets + "vendor/choices.js/public/assets/scripts/choices.min.js", // choices js
    ])
        .pipe(concat("vendor.js"))
        .pipe(uglify())
        .pipe(rename({ suffix: ".min" }))
        .pipe(dest(outjs));
};

const scss = function () {
    const out = paths.baseDistAssets + "css/";

    src([paths.baseSrcAssets + "scss/**/*.scss", "!" + paths.baseSrcAssets + "scss/icons.scss", "!" + paths.baseSrcAssets + "scss/icons/*.scss"])
        .pipe(sourcemaps.init())
        .pipe(sass.sync().on('error', sass.logError)) // scss to css
        .pipe(
            autoprefixer({
                overrideBrowserslist: ["last 2 versions"],
            })
        )
        .pipe(dest(out))
        .pipe(CleanCSS())
        .pipe(rename({ suffix: ".min" }))
        .pipe(sourcemaps.write("./")) // source maps
        .pipe(dest(out));

    // generate rtl
    return src([paths.baseSrcAssets + "scss/**/*.scss", "!" + paths.baseSrcAssets + "scss/icons.scss", "!" + paths.baseSrcAssets + "scss/icons/*.scss"])
        .pipe(sourcemaps.init())
        .pipe(sass.sync().on('error', sass.logError)) // scss to css
        .pipe(
            autoprefixer({
                overrideBrowserslist: ["last 2 versions"],
            })
        )
        .pipe(rtlcss())
        .pipe(rename({ suffix: "-rtl" }))
        .pipe(dest(out))
        .pipe(CleanCSS())
        .pipe(rename({ suffix: ".min" }))
        .pipe(sourcemaps.write("./")) // source maps
        .pipe(dest(out));
};

const icons = function () {
    const out = paths.baseDistAssets + "css/";
    return src([paths.baseSrcAssets + "scss/icons.scss", paths.baseSrcAssets + "scss/icons/*.scss"])
        .pipe(sourcemaps.init())
        .pipe(sass.sync()) // scss to css
        .pipe(
            autoprefixer({
                overrideBrowserslist: ["last 2 versions"],
            })
        )
        .pipe(dest(out))
        .pipe(CleanCSS())
        .pipe(rename({ suffix: ".min" }))
        .pipe(sourcemaps.write("./")) // source maps
        .pipe(dest(out));
};


function watchFiles() {
    watch(paths.baseSrcAssets + "scss/icons.scss", series(icons));
    watch([paths.baseSrcAssets + "scss/**/*.scss", "!" + paths.baseSrcAssets + "scss/icons.scss", "!" + paths.baseSrcAssets + "scss/icons/*.scss"], series(scss));
}

// Production Tasks
exports.default = series(
    vendor,
    parallel(plugin, scss, icons),
    parallel(watchFiles)
);

// Build Tasks
exports.build = series(
    vendor,
    parallel(plugin, scss, icons)
);