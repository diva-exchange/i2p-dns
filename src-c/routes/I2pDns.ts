import express, { NextFunction, Router } from "express";
import controller from '../controllers/I2pDns.controller';

const router = express.Router();

router.get('/:dns', controller.getDns);
router.put('/:dns/:b32', controller.putDns);

export = router;