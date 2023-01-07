import express, { NextFunction, Router } from "express";
import controller from '../controllers/I2pDnsController';

const router = express.Router();

router.get('/:dns', controller.getDns);
router.put('/', controller.putDns);

export = router;