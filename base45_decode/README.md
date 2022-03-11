## Decode Funktion

Schreibe die Decode Funktion in *src/index.js*, sodass sie das fertige Ergebnis zurückliefert.
Es muss dann nur mehr diese Funktion exportiert werden.

```javascript
import base45 from 'base45-web'
import pako from 'pako'
import { Decoder } from 'cbor-web'
import { Buffer } from 'Buffer'

function decode(base45String) {
    // Remove `HC1:` from the string
    const greenpassBody = base45String.trim().substr(4);
    const decodedData = base45.decode(greenpassBody);
    const decodedBuffer = Buffer.from(decodedData);
    const unzipped = pako.inflate(decodedBuffer);
    const results = Decoder.decodeAllSync(unzipped);
    let headers1, headers2, cbor_data, signature;
    [headers1, headers2, cbor_data, signature] = results[0].value;
    return cbor_data;
}

export {
    decode
};
```

## package.json

```json
{
  "dependencies": {
    "base45": "^2.0.1",
    "base45-web": "^1.0.2",
    "buffer": "^6.0.3",
    "cbor-web": "^8.1.0",
    "fflate": "^0.7.3",
    "html-webpack-plugin": "^5.5.0",
    "i": "^0.3.7",
    "nofilter": "^3.1.0",
    "pako": "^2.0.4",
    "process": "^0.11.10",
    "stream-browserify": "^3.0.0",
    "webpack": "^5.65.0"
  },
  "scripts": {
    "build": "webpack --config webpack.config.js",
    "serve": "webpack serve --config webpack.config.js --mode=development"
  },
  "devDependencies": {
    "webpack-cli": "^4.9.1",
    "webpack-dev-server": "^4.7.1"
  }
}
```

## webpack.config.js

```js
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
```

## decode im Browser verwenden

Im Browser kann nun diese Funktion genutzt werden (*public/index.html*):

```html
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>QR Decoder Test</title>
    <style>
        html,
        body {
            width: 100%;
            height: 100%;
            padding: 0;
            margin: 0;
            font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
        }

        input,
        button {
            padding: 5px;
        }

        pre {
            white-space: pre-wrap;
            background-color: #EEE;
            padding: 1rem;
            border: 1px solid #999;
        }
    </style>
</head>

<body>
    <div style="max-width: 768px; margin: 1rem auto; border:1px solid #888; padding:1rem">
        <div style="display: flex; gap:0.4rem; align-items:flex-start;">
            <textarea style="flex-grow: 1" rows="10" id="qrContent">HC1:NCFK80R80T9WTWGVLKG59C:PG4NGHHC85X*4FBB$*4FN0IDC.WEWY0ADCX5K*70TB8D97TK0F90IECSHG3KCZPCNF6OF63W5CA7*96/SA8:6DL61G73564KCAWE6T9G%6G%67W5/JCE$D8+9TB84KC*PCUV3REDY CVKEZED234X34:X8PR8379*KENLFM-DF195JCWJCT3ETB8WJC0FD$F5AIA%G7X+AQB9746IG77TA:F6FL6RX8:F6OCA/HA+H9YX89A6UCAX+A 1BN*9TH8+EDC8FHZ95/D QEALEN44:+C%69AECAWE1Q5HC8 QE*KE0EC9WEQDD+Q6TW6FA7C464KC*KETF6A46.96646C56..DX%DLPCG/D0ED$B91OA:S9E+9ZJC5/D0O96$CSQEZED73DI3D8WERB8LTAHZA JC2/DEC8AWE6%E$PC5$CUZC$$5Y$5JPCT3E6JD646KF6B463W5IG6F988*SA$GYQA5BGGX1%YGZ81YP4$/SQ K228.:NK4L%IC2WUH%P81NW020T6PWL*4FO Q--KI5D/ 9B.QY6MNJ4CKATRJ..DDSTK7VI2</textarea>
            <button type="button" onclick="decodeQrContent()">Decode!</button>
        </div>
        <div>
            <h4>Output</h4>
            <pre id="result"></pre>
        </div>
    </div>
    <script>
        function decodeQrContent() {
            const decoded = QrDecoder.decode(document.getElementById("qrContent").value);
            document.getElementById("result").innerText = decoded
        }

    </script>

</body>

</html>
```