'use strict';
const webpack = require('webpack');
module.exports = {
    entry: {
        Home: './wwwroot/src/Home/app.js',
        Shared: './wwwroot/src/shared.js'
    },
    output: {
        path: __dirname + "/wwwroot/dist/",
        filename: "[name].min.js"
    },
    module: {
        loaders: [
            {
                test: /\.js$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader?cacheDirectory'
                }
            },
            {
                test: /\.css$/,
                loader: "style-loader!css-loader"
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
        ]
    },
    plugins: [
        new webpack.optimize.UglifyJsPlugin({
            output: {
                comments: false //No comments
            }
        })
    ]
}