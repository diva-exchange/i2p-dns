"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const axios_1 = __importDefault(require("axios"));
const logging_1 = __importDefault(require("../config/logging"));
const config_1 = __importDefault(require("../config/config"));
const command_1 = __importDefault(require("../model/command"));
const NAMESPACE = "CONTROLLER";
const getDnsFromChain = (req, res, next) => {
    const dns = "IIPDNS:" + req.params.dns.replace(".i2p", ":i2p_");
    const url = `http://${config_1.default.divaApi.hostname}:${config_1.default.divaApi.port}${config_1.default.divaApi.getPath}${dns}`;
    logging_1.default.info(NAMESPACE, url);
    axios_1.default
        .get(url)
        .then((response) => {
        res.status(200).send(response.data);
    })
        .catch((err) => {
        logging_1.default.info(NAMESPACE, "ERROR: ", err);
        res.status(404).send(err);
    });
};
const putDns = (req, res, next) => {
    logging_1.default.info(NAMESPACE, "PUT DNS CALL");
    const url = `http://${config_1.default.divaApi.hostname}:${config_1.default.divaApi.port}${config_1.default.divaApi.putPath}`;
    let command = new command_1.default(req.params.dns, req.params.b32);
    const data = [command];
    const httpHeaders = {
        headers: {
            "Content-Type": "application/json; charset=utf-8",
        },
    };
    axios_1.default
        .put(url, data, httpHeaders)
        .then((response) => {
        res.status(200).send(response.data);
    })
        .catch((err) => res.status(403).send(err.message));
};
const postToChain = (req, res, next) => { };
exports.default = { getDnsFromChain, postToChain, putDns };
