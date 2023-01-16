"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
const express_1 = __importDefault(require("express"));
const controller_1 = __importDefault(require("../controllers/controller"));
const NAMESPACE = "ROUTES";
const router = express_1.default.Router();
router.get("/:dns([a-z0-9-_]{3,64}.i2p)", controller_1.default.getDnsFromChain);
router.put("/:dns([A-Za-z0-9-_]{3,64}.i2p)/:b32([a-z0-9]{52})", controller_1.default.putDns);
module.exports = router;
