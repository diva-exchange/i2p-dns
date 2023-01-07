import { Request, Response, NextFunction } from "express";
import logging from "../config/logging";

const NAMESPACE = 'i2p DNS Controller';

const getDns = (req:Request, res:Response, next:NextFunction) => {
    logging.info(NAMESPACE, `Get DNS called`);

    return res.send(req.params);
};

const putDns = (req:Request, res:Response, next:NextFunction) => {
    logging.info(NAMESPACE, `Put DNS called. ${req}`);

    return res.status(200).json({
        message: req.body
    });
};

export default { getDns, putDns }