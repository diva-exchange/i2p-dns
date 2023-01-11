import axios from 'axios';
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
    logging.info(NAMESPACE, `GetDnsFromChain ${config.server.divaApi}`);     

    //axios.get(config.server.divaApi)
    //axios.get("https://catfact.ninja/fact")   
    axios.get("http://127.19.72.21:17468/state/search/IIPDNS:google.i2p")
        .then(response => res.status(200).send(response.data))
        .catch(err => next(err));
};

const putDns = (req: Request, res: Response, next: NextFunction) => {
    logging.info(NAMESPACE, "Put Dns", req.params);

    res.status(200).send(req.params);
};

const postToChain = (req: Request, res: Response, next: NextFunction) => {

};

export default {getDns, getDns2, getDnsFromChain, postToChain, putDns}