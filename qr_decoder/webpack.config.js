// https://webpack.js.org/plugins/html-webpack-plugin/
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
  resolve: {
    fallback: { stream: require.resolve("stream-browserify"),}
  },
  
  plugins: [
    new HtmlWebpackPlugin({
      template: 'public/index.html',
      inject: 'head',
      scriptLoading: 'blocking'
    })],
}
