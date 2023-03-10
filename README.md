# i2p-dns

## 0. Seth the running directory
```pwsh
Set-Location .\src-d\diva-dns\
```

## 1. Extract the image
```pwsh
Expand-Archive -Path .\image\diva-dns-image.zip -DestinationPath .\image\
```

## 2. Install the image
```pwsh
docker load -i .\image\diva-dns-image.tar
```

## 3. Build and run the container
Variant 1: Attach the terminal for direct input
```pwsh
docker compose up -d && docker attach i2p.diva.dns
```
Variant 2: Build and start without attaching:
```pwsh
docker compose up -d
```

## 4. Shut down the container
If you attached the terminal, then shut down the service by typing `exit`. Then:
```pwsh
docker compose down
```

# Use the Service
The Terminal provides three commands:
## Terminal: Get
With the Get command you can check the "B32" IP of a domain. 
```pwsh
get [domain-name].i2p
```

## Terminal: Put
With the put command you can register a domain name with a "B32" IP.
```pwsh
put [domain-name].i2p [b32-string]
```

## Terminal: Exit
With the Exit command you can shut down the Diva server and close the Terminal.
```pwsh
exit
```

## REST API
Server is accessible under: `http://127.19.72.227:19445/`.
```
GET /^([A-Za-z_-]{4,15}:){1,3}\.i2p$

PUT /^([A-Za-z_-]{4,15}:){1,3}\.i2p$/[a-z0-9]{53}$
```
