import axios from 'axios';
import { Request, Response, NextFunction } from 'express';
import logging from '../config/logging';

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
    logging.info(NAMESPACE, "GetDnsFromChain");
    
    axios.get("https://catfact.ninja/fact")
        .then(response => res.status(200).send(response.data))
        .catch(err => next(err));
};

const postToChain = (req: Request, res: Response, next: NextFunction) => {

};

export default {getDns, getDns2, getDnsFromChain, postToChain}