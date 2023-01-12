import dotenv from 'dotenv';

dotenv.config();

const SERVER_HOSTNAME = process.env.SERVER_HOSTNAME || 'localhost';
const SERVER_PORT = process.env.SERVER_PORT || 1337;

const DIVA_API_HOSTNAME = '172.19.72.21';
const DIVA_API_PORT = 17468;
const DIVA_API_PATH_GET = '/state/search/IIPDNS:';
const DIVA_API_PATH_PUT = '/transaction/';

const SERVER = {
    hostname: SERVER_HOSTNAME,
    port: SERVER_PORT,
};

const DIVA_API = {
    hostname: DIVA_API_HOSTNAME,
    port: DIVA_API_PORT,
    getPath: DIVA_API_PATH_GET,
    putPath: DIVA_API_PATH_PUT
};

const config = {
    server: SERVER,
    divaApi: DIVA_API
};

export default config;