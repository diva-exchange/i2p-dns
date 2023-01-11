import express, { Request, Response, NextFunction } from 'express';
import logging from './config/logging';
import route from './routes/routes';
import bodyParser from 'body-parser';

const NAMESPACE = 'APP';
const app = express();
//const router = express.Router();

/** Logging the request */
app.use((req: Request, res: Response, next: NextFunction) => {
    logging.info(NAMESPACE, `METHOD - [${req.method}], URL - ${req.url}, IP - [${req.socket.remoteAddress}]`);
    
    res.on('finish', () => {
        logging.info(NAMESPACE, `METHOD - [${req.method}], URL - ${req.url}, IP - [${req.socket.remoteAddress}], STATUS - [${res.statusCode}]`);
    });

    next();
});

app.use(express.urlencoded({ extended: false }));
app.use(bodyParser.json());

/** Rules of our API */
app.use((req: Request, res: Response, next: NextFunction) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Headers', 'Origin, X-Requested-With, Content-Type, Accept, Authorization');

    if (req.method == 'OPTIONS') {
        res.header('Access-Control-Allowed-Methods', 'GET PATCH DELETE POST PUT');
        return res.status(200).json({});
    }

    next();
});

app.use('/', route);

app.use((req: Request, res: Response) => {
    const error = new Error('not found');
    
    return res.status(404).json({
        message: error.message
    });
});

app.listen(1337, () => {
    logging.info(NAMESPACE, "Server is listening");
});