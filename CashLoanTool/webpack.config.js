﻿'use strict';
const webpack = require('webpack');
module.exports = {
    entry: {
        Shared: './wwwroot/src/shared.js',
        Home: './wwwroot/src/Home/app.js',
        Adm: './wwwroot/src/Adm/app.js'
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