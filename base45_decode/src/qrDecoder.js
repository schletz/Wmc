import base45 from 'base45-web'
import pako from 'pako'
import cbor from 'cbor-web'

class QrDecoder {
    constructor() {
    }
    base45Decode(str) {
        return base45.decode(str);
    }

}

export default QrDecoder;