# Documentation

## Summary

## API-Description

### GET-Request

#### Example

```
GET /state/search/
curl -i -H 'Accept: application/json' `http://localhost:1337/state/search/IIPDNS:<PUT-YOUR-DNS-HERE>:i2p_`
```

### PUT-Request

#### Example

```
PUT /transaction/
curl -X PUT -H "Content-Type: application/json" -d '{"seq":1, "command":"data", "ns":"IIPDNS:<YOUR-DNS>:i2p_", "d":"<B32 STRING>"}' http://172.19.72.21:17468/transaction/
```

## Build / Development

### Installing dependencies
Type the following command into the command-line. Please be sure you are in the root directory!
`npm install`

### Starting the app
Type the following command into the command-line. Please be sure you are in the root directory!
`nodemon app.js` 

## Start using docker compose
Type the following command into the command-line. Please be sure you are in the app directory!
`docker compose up -d`

## Stop using docker compose
Type the following command into the command-line. Please be sure you are in the app directory!
`docker compose stop`

## Conclusion
