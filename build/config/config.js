"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const dotenv_1 = __importDefault(require("dotenv"));
dotenv_1.default.config();
const SERVER_HOSTNAME = process.env.SERVER_HOSTNAME || 'localhost';
const SERVER_PORT = process.env.SERVER_PORT || 1337;
const DIVA_API_HOSTNAME = '127.19.72.21';
const DIVA_API_PORT = 17468;
const SERVER = {
    hostname: SERVER_HOSTNAME,
    port: SERVER_PORT,
    divaApi: `http://${DIVA_API_HOSTNAME}:${DIVA_API_PORT}/state/search/IIPDNS:`,
};
const config = {
    server: SERVER
};
exports.default = config;
