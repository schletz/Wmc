import base45 from 'base45-web'
import pako from 'pako'
import { Html5QrcodeScanner } from "html5-qrcode"
import { Decoder } from 'cbor-web'
import { Buffer } from 'Buffer'
import Encodr from "encodr"

function decode(base45String) {
    const match = /HC1\:(?<data>[ \$\%\*\+\-\.\/\:0-9A-Z]+)/.exec(base45String);
    if (!match) { throw "Invalid healthcare code or invalid base45 string."; }
    const decodedData = Buffer.from(base45.decode(match.groups.data));
    const unzipped = pako.inflate(decodedData);
    const result = Decoder.decodeFirstSync(unzipped);
    const [headers1, headers2, cbor_data, signature] = result.value;
    // https://dgc.a-sit.at/ehn
    return cbor_data;
}

export {
    decode,
    Html5QrcodeScanner
};
