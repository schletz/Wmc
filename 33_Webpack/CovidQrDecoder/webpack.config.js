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
const HtmlWebpackPlugin = require('html-webpack-plugin');
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
    library: "QrDecoder"
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
    new HtmlWebpackPlugin({
      template: 'public/index.html',
      inject: 'head',
      scriptLoading: 'blocking'
    })],
}
