import { Request, Response, NextFunction } from "express";
import logging from "../config/logging";
import util from "../util/util";

const NAMESPACE = 'i2pDNS Controller';

const getDns = (req:Request, res:Response, next:NextFunction) => {
    logging.info(NAMESPACE, `Get DNS called ${req.params.dns.substring(0)}`);

    let regexCheck = util.check('^[a-z0-9-_]{3,64}\.i2p$', req.params.dns);

    // Todo: Architecture middleware?

    if (regexCheck) {
        return res.send({
            status: 200,
            message: `Domain ${req.params.dns} added.`
        });
    } else {
        return res.send({
            status: 400,
            message: 'Malformed request'
        });
    }
};

const putDns = (req:Request, res:Response, next:NextFunction) => {
    logging.info(NAMESPACE, `Put DNS called. ${req}`);

    // Todo: Regex & params check
    // Todo: b32 string create
    // Todo: Put on Chain

    return res.status(200).json({
        message: req.params
    });
};

export default { getDns, putDns }