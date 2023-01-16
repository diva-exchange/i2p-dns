import axios from 'axios';
import { Request, Response, NextFunction } from 'express';
import logging from '../config/logging';
import config from '../config/config';
import Command from '../model/command';

const NAMESPACE = 'CONTROLLER';

const getDnsFromChain = (req: Request, res: Response, next: NextFunction) => {
    const dns: string = req.params.dns.replace(".i2p", ":i2p_");
    const url: string = `http://${config.divaApi.hostname}:${config.divaApi.port}${config.divaApi.getPath}${dns}`;
    
    logging.info(NAMESPACE, url);

    axios.get(url)
        .then(response => {
            res.status(200).send(response)
        })
        .catch(err => {
            res.status(404).send(err)
        });
};

const putDns = (req: Request, res: Response, next: NextFunction) => {
    logging.info(NAMESPACE, "PUT DNS CALL");

    const url: string = `http://${config.divaApi.hostname}:${config.divaApi.port}${config.divaApi.putPath}`;

    let command = new Command(req.params.dns, req.params.b32);
    const data: any = [command];

    const httpHeaders: any = {
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        }
    };
    
    axios.put(url, data, httpHeaders)
        .then(response => {
            res.status(200).send(response.data)
        })
        .catch(err => res.status(403).send(err.message));
};

const postToChain = (req: Request, res: Response, next: NextFunction) => {

};

export default {getDnsFromChain, postToChain, putDns}