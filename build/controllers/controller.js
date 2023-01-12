"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const http_1 = __importDefault(require("http"));
const logging_1 = __importDefault(require("../config/logging"));
const config_1 = __importDefault(require("../config/config"));
const NAMESPACE = 'CONTROLLER';
const getDns = (req, res, next) => {
    logging_1.default.info(NAMESPACE, `Get DNS called ${req.params.dns.substring(0)}`);
    next();
};
const getDns2 = (req, res, next) => {
    logging_1.default.info(NAMESPACE, `Get DNS 2 called ${req.params.dns.substring(0)}`);
    /* res.send({
         status: 200,
         message: req.params.dns
     });    */
    next();
};
const getDnsFromChain = (req, res, next) => {
    const dns = req.params.dns.replace(".i2p", ":i2p_");
    var request = http_1.default.request({
        host: config_1.default.divaApi.hostname,
        port: config_1.default.divaApi.port,
        path: `${config_1.default.divaApi.path}${dns}`,
        method: 'GET'
    }, function (response) {
        var data = '';
        response.setEncoding('utf8');
        response.on('data', (chunk) => {
            data += chunk;
        });
        response.on('end', () => {
            //res.end('check result:' + data);
            res.status(200).send(data);
        });
        response.on('error', (err) => {
            res.status(404).send(err);
        });
    });
    request.end();
};
const putDns = (req, res, next) => {
    logging_1.default.info(NAMESPACE, "Put Dns", req.params);
    res.status(200).send(req.params);
};
const postToChain = (req, res, next) => {
};
exports.default = { getDns, getDns2, getDnsFromChain, postToChain, putDns };
