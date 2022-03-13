// https://webpack.js.org/plugins/html-webpack-plugin/
// ESLINT INSTALLATION
//     npm install eslint --save-dev
//     npm init @eslint/config
//     Die Datei .eslintrc.json nach der Erstellung in src verschirben
// BABEL INSTALLATION
//     npm install --save-dev @babel/core @babel/preset-env
//     npm install --save-dev babel-loader
//     Dann babel.config.json mit folgendem Inhalt anlegen:
//         { "presets": [ "@babel/preset-env" ] }
// COPYWEBPACKPLUGIN
//     Zum Kopieren des css Files in den dist Ordner
//     npm install copy-webpack-plugin --save-dev
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyPlugin = require('copy-webpack-plugin');
const path = require('path');

module.exports = {
  mode: 'production',
  devServer: {
    static: {
      directory: path.join(__dirname, 'public'),
    },
    compress: true,
    port: 9000,
    https: true
  },
  output: {
    filename: '[name].bundle.js',
    libraryTarget: "var",
    library: "QrDecoder",
    clean: true
  },
  module: {
    rules: [
      {
        test: /\.(js)$/,
        exclude: /node_modules/,
        use: ['babel-loader']
      }   
    ]
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        {
          context: "public",
          from: '*.css',
          to: ''
        }
      ]
    }),
    new HtmlWebpackPlugin({
      template: 'public/index.html',
      inject: 'head',
      scriptLoading: 'blocking'
    })],
}
