import base45 from 'base45-web'
import pako from 'pako'
import { Html5QrcodeScanner } from "html5-qrcode"
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
    decode,
    Html5QrcodeScanner
};
