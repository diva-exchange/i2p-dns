# Documentation

## Goal
Erstellung eines einfachen «DNS»-Prototypen in einem vertrauenslosen, verteilten Systems innerhalb eines «Overlay»-Netzwerkes (I2P). Es kann nur eine Art "A-Record" gelesen und geschrieben werden (keine Implementation von DNS Record Typen). Ein "A-Record" im Kontext von I2P ist ein Schlüssel/Wert-Paar, also ein "Domain-Name" als Schlüssel und eine "B32-Adresse" als Wert. Alle Domain Namen im I2P Kontext enden auf ".i2p", also beispielsweise "overlaynetzwerk.i2p". In dieser einfachen Implementation werden nur sehr einfache Namen unterstützt (siehe Regex unten).

## API-Description

Default IP: 172.19.72.45
Default Port: 1337

### Example requests for Diva Chain

#### GET-Request
```
GET /state/search/
curl -i -H 'Accept: application/json' http://<DIVA.DOCKERIZED-IP>:17468/state/search/IIPDNS:<PUT-YOUR-DNS-HERE>:i2p_
```

### PUT-Request
```
PUT /transaction/
curl -X PUT -H "Content-Type: application/json; charset=utf-8" -d '[{"seq":1, "command":"data", "ns":"<PUT-YOUR-DNS-HERE>", "d":"<B32>"}]' http://<DIVA.DOCKERIZED-IP>:17468/transaction/
```

### Example requests from our app

#### GET-Request
```
GET /<PUT-YOUR-DNS-HERE>
curl -i -H 'Accept: application/json' http://172.19.72.45:1337/<PUT-YOUR-DNS-HERE>
```

### PUT-Request
```
PUT /<PUT-YOUR-DNS-HERE>/<B32>
curl -X PUT -H "Content-Type: application/json; charset=utf-8" http://172.19.72.45:1337/<PUT-YOUR-DNS-HERE>/<B32>
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
Zu Beginn der Arbeit hatten wir einige Probleme mit der Installation der Diva-Chain. Da wir beide wenig Linux Vorkenntnisse haben, mussten wir viel Zeit investieren um uns grundlegende Terminal-Befehle beizubringen.
Dank Rücksprachen mit Konrad, konnten wir die Probleme lösen und eine lauffähige node/TypeScript Applikation entwicklen.
