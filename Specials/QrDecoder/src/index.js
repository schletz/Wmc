import base45 from 'base45-web'
import pako from 'pako'
import { Html5QrcodeScanner } from "html5-qrcode"
import { Decoder } from 'cbor-web'
import { Buffer } from 'Buffer'


// Sucht nach dem Key "ver" in den Maps. Diese können verschachtelt sein.
// Siehe https://dgc.a-sit.at/ehn/
function decodePayload(map) {
    for (const value of map.values()) {
        if (value instanceof Map) return decodePayload(value);
        if (value.ver) { return value; }
    }    
    return null;
}

function decode(base45String) {
    const match = /HC1\:(?<data>[ \$\%\*\+\-\.\/\:0-9A-Z]+)/.exec(base45String);
    if (!match) { throw "Invalid healthcare code or invalid base45 string."; }
    const decodedData = Buffer.from(base45.decode(match.groups.data));
    const unzipped = pako.inflate(decodedData);
    const rawContent = Decoder.decodeFirstSync(unzipped);
    const rawPayload = Decoder.decodeFirstSync(rawContent.value[2]);
    const payload = decodePayload(rawPayload);
    if (!payload) { throw "QrCode contains no data."; }
    return payload;
}

export {
    decode,
    Html5QrcodeScanner
};
