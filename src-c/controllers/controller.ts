import http from 'http';
import { Request, Response, NextFunction } from 'express';
import logging from '../config/logging';
import config from '../config/config';

const NAMESPACE = 'CONTROLLER';

const getDns = (req: Request, res: Response, next: NextFunction) => {
    logging.info(NAMESPACE, `Get DNS called ${req.params.dns.substring(0)}`);
    next();
};

const getDns2 = (req: Request, res: Response, next: NextFunction) => {
    logging.info(NAMESPACE, `Get DNS 2 called ${req.params.dns.substring(0)}`);
   /* res.send({
        status: 200,
        message: req.params.dns
    });    */
    next();
};

const getDnsFromChain = (req: Request, res: Response, next: NextFunction) => {
    const dns: string = req.params.dns.replace(".i2p", ":i2p_");

    var request = http.request({
        host: config.divaApi.hostname,
        port: config.divaApi.port,
        path: `${config.divaApi.path}${dns}`,
        method: 'GET'
    }, function(response) {
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

const putDns = (req: Request, res: Response, next: NextFunction) => {
    logging.info(NAMESPACE, "Put Dns", req.params);

    res.status(200).send(req.params);   
};

const postToChain = (req: Request, res: Response, next: NextFunction) => {

};

export default {getDns, getDns2, getDnsFromChain, postToChain, putDns}