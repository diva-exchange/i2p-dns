# Documentation

## Goal
Erstellung eines einfachen «DNS»-Prototypen in einem vertrauenslosen, verteilten Systems innerhalb eines «Overlay»-Netzwerkes (I2P). Es kann nur eine Art "A-Record" gelesen und geschrieben werden (keine Implementation von DNS Record Typen). Ein "A-Record" im Kontext von I2P ist ein Schlüssel/Wert-Paar, also ein "Domain-Name" als Schlüssel und eine "B32-Adresse" als Wert. Alle Domain Namen im I2P Kontext enden auf ".i2p", also beispielsweise "overlaynetzwerk.i2p". In dieser einfachen Implementation werden nur sehr einfache Namen unterstützt (siehe Regex unten).

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
Zu Beginn der Arbeit hatten wir einige Probleme mit der Installation der Diva-Chain. Da wir beide wenig Linux Vorkentnisse haben, mussten wir viel Zeit investieren um uns grundlegende Terminal-Befehle beizubringen.
Dank Rücksprachen mit Konrad, konnten wir die Probleme lösen und eine laufffähige node/TypeScript Applikation entwicklen.
