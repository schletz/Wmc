import base45 from 'base45-web'
import pako from 'pako'
import { Html5QrcodeScanner } from "html5-qrcode"
import { Decoder, Encoder } from 'cbor-web'
import { Buffer } from 'Buffer'
import QrCode from 'qrcode'

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
    const cborEncoded = pako.inflate(decodedData);
    const rawContent = Decoder.decodeFirstSync(cborEncoded);
    const rawPayload = Decoder.decodeFirstSync(rawContent.value[2]);
    const payload = decodePayload(rawPayload);
    if (!payload) { throw "QrCode contains no data."; }
    return { cborEncoded: cborEncoded, payload: payload }
}

/**
 * Erzeugt einen neuen QR Code, indem ein neuer Name eingefügt wird.
 * Die Signaturprüfung wird allerdings fehlschlagen.
 */
function generateQrData(cborEncoded, lastname, firstname, birth) {
    const rawContent = Decoder.decodeFirstSync(cborEncoded);
    const payload = Decoder.decodeFirstSync(rawContent.value[2]);
    if (!payload.get(-260) || !payload.get(-260).get(1)) {throw "Invalid payload in original qr code."}
    payload.get(-260).get(1).nam.fn = lastname;
    payload.get(-260).get(1).nam.fnt = lastname.toUpperCase();
    payload.get(-260).get(1).nam.gn = firstname;
    payload.get(-260).get(1).nam.gnt = firstname.toUpperCase();    
    payload.get(-260).get(1).dob = birth;
    rawContent.value[2] = Encoder.encode(payload);
    cborEncoded = Encoder.encode(rawContent);
    cborEncoded = pako.deflate(cborEncoded);
    return "HC1:" + base45.encode(cborEncoded);
}

export {
    decode,
    generateQrData,
    Html5QrcodeScanner,
    QrCode
};
