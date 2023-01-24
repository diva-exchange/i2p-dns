# DivaDNS Server 

This server application is  prototype of a simple DNS based on the Diva-Exchange blockchain.  
Its main goal is to show how a simple DNS can be implemented with a blockchain in a trustless network.

The server is written in Kotlin, using the SpringBoot framework.


## The application (features)

Run the image using the `docker-compose` file:

- command is `DIVAEXCHANGE_BASEURL=<divaBaseUrl> docker compose up -d`  (default is `http://172.19.72.21:17468`)
  where <divaBaseUrl> is the URL of a Diva blockchain node, e.g. `DIVAEXCHANGE_BASEURL=http://172.19.69.21:17468 docker compose up -d`.
- to stop the container, use `DIVAEXCHANGE_BASEURL=<divaBaseUrl> docker compose down`.
- If the blockchain does not use default settings, you can also configure `HOST_BASE_IP`, `BASE_IP` and `PORT` 
  as described in https://github.com/diva-exchange/diva-dockerized#environment-variables .


### Domain lookup

`GET /<domain.i2p>`

- where <domain.i2p> is a domain name in format \"[a-z0-9-_]{3-64}\.i2p$\"
- returns: `<domain.i2p>=<b32name>`
- Example: `GET /diva.i2p`, returns `diva.i2p=auoqibfnyujhcht4v3nzahpqztwlyomesfywltuls5bqqi3nd3ka`


### Register a new domain

`PUT /<domain.ip2>/<b32address>`

- where <b32address> is the address in format \"[a-z0-9]{52}$\" and <domain.i2p> is as in GET above.
- Example: `PUT /diva.i2p/auoqibfnyujhcht4v3nzahpqztwlyomesfywltuls5bqqi3nd3ka`


### Info

`GET /info`

- will return information about the current deployed server like version etc. (as HTML)
- Example: `http://localhost:8080/info` in a web-browser (or `GET /info`)


## Implementation details

See [FOR_DEVELOPERS.md](FOR_DEVELOPERS.md)
