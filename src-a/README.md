# i2p-dns

## Descritpion
This is a Domain Name Service (DNS) that acts on the diva blockchain.

## Getting started
### Starting the service
Make sure [diva-dockerized](https://github.com/diva-exchange/diva-dockerized) is running before starting i2p-dns

**Expand** the compressed archive (using PowerShell):
```
Expand-Archive -Path ./dist/i2p-dns.tar.zip -DestinationPath ./dist
```

To **Load** the docker image:
```
docker load --input ./dist/i2p-dns.tar
```

To **Start** the docker container:
```
docker run --name i2p-dns --network network.join.testnet.diva.i2p --env URL_API_CHAIN=http://172.19.72.21:17468/ --publish 5126:80 i2p-dns:src-a
```

To **Stop** the docker container:
```
docker stop i2p-dns
```

### Using the api
Read the **api documentation** on http://localhost:5126/swagger/index.html

## Development
Make sure [diva-dockerized](https://github.com/diva-exchange/diva-dockerized) is running before starting i2p-dns.

The docker image will be built from the current code and used to run the container.

To **Start** the docker container:
```
docker compose up -d
```

To **Stop** the docker container:
```
docker compose stop
```

## Provide the docker image
To **Build** the docker image:
```
docker build --file ./src/DivaDnsWebApi/Dockerfile --tag i2p-dns:src-a ./src/DivaDnsWebApi
```

To **Save** the docker image to an archive:
```
docker save --output ./dist/i2p-dns.tar i2p-dns:src-a
```

**Compress** the archive (using PowerShell):
```
Compress-Archive -Path ./dist/i2p-dns.tar -DestinationPath ./dist/i2p-dns.tar.zip
```
