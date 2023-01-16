"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class Command {
    constructor(dns, b32) {
        this.seq = 1;
        this.command = "data";
        this.ns = "";
        this.d = "";
        this.ns = "IIPDNS:" + dns.replace(".i2p", ":i2p_");
        this.d = b32;
    }
}
exports.default = Command;
