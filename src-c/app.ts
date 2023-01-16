import express, {
  Request,
  Response,
  NextFunction,
  ErrorRequestHandler,
} from "express";
import logging from "./config/logging";
import route from "./routes/routes";
import bodyParser from "body-parser";
import config from "./config/config";

const NAMESPACE = "APP";
const app = express();
//const router = express.Router();

/** Logging the request */
app.use((req: Request, res: Response, next: NextFunction) => {
  logging.info(
    NAMESPACE,
    `METHOD - [${req.method}], URL - ${req.url}, IP - [${req.socket.remoteAddress}]`
  );

  res.on("finish", () => {
    logging.info(
      NAMESPACE,
      `METHOD - [${req.method}], URL - ${req.url}, IP - [${req.socket.remoteAddress}], STATUS - [${res.statusCode}]`
    );
  });

  next();
});

app.use(express.urlencoded({ extended: false }));
app.use(bodyParser.json());

/** Rules of our API */
app.use((req: Request, res: Response, next: NextFunction) => {
  res.header("Access-Control-Allow-Origin", "*");
  res.header(
    "Access-Control-Headers",
    "Origin, X-Requested-With, Content-Type, Accept, Authorization"
  );

  if (req.method == "OPTIONS") {
    res.header("Access-Control-Allowed-Methods", "GET PATCH DELETE POST PUT");
    return res.status(200).json({});
  }

  next();
});

app.use("/", route);

app.use(
  (
    err: ErrorRequestHandler,
    req: Request,
    res: Response,
    next: NextFunction
  ) => {
    return res.status(502).json({
      message: err,
    });
  }
);

app.listen(config.server.port, () => {
  logging.info(NAMESPACE, "Server is listening", config);
});
