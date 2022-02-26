const path = require('path');
const { defineConfig } = require('@vue/cli-service')

module.exports = defineConfig({
  publicPath: './',
  transpileDependencies: true,
  outputDir: path.resolve("../api/public")
})
