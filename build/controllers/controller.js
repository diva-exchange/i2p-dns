"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const axios_1 = __importDefault(require("axios"));
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
    logging_1.default.info(NAMESPACE, `GetDnsFromChain ${config_1.default.server.divaApi}`);
    //axios.get(config.server.divaApi)
    //axios.get("https://catfact.ninja/fact")   
    axios_1.default.get("http://127.19.72.21:17468/state/search/IIPDNS:google.i2p")
        .then(response => res.status(200).send(response.data))
        .catch(err => next(err));
};
const putDns = (req, res, next) => {
    logging_1.default.info(NAMESPACE, "Put Dns", req.params);
    res.status(200).send(req.params);
};
const postToChain = (req, res, next) => {
};
exports.default = { getDns, getDns2, getDnsFromChain, postToChain, putDns };
