# Info for Developers about DivaDNS Server

Basic usage see [README.MD](README.md)

## Implementation details

IN the current version of Diva Exchange (v0.34.x), the namespace of the blockchain must comply with the following regex:
`"^([A-Za-z_-]{4,15}:){1,4}[A-Za-z0-9_-]{1,64}$"`  
Specifically: digits may only occur in the last part of the namespace, and all parts before the last must be at least
4 characters long. Therefore, using the namespace as specified "I2PDNS:[domain-name.i2p]" does not work, the digit is
not allowed, nor is the dot for the prefix.

As a **workaround**, I use the following format:

- Use prefix "IIPDNS" instead of "I2PDNS".
- Use the last part of the namespace for the domain name without the suffix ".i2p". This way, the domain-name can also
  contain digits and can be up to 64 characters long.


## Run the application

### For development, in IntelliJ IDEA:

* create a Maven run configuration with command line `clean spring-boot:run`
* Alternatively, create a springboot run configuration for `ch.hslu.tfbirrer.divadns.DivaDnsApplication`
  and set it to reload classes and resources on update (like a project build).

In both cases, set the run configuration to run the maven goal `spring-boot:build-info` before starting the main task.


### In a Docker container

1. Build the app with: `mvn package`

2. Now there should be a jar file under `/target/*.jar`. This could already be run with `java -jar <path to jar file>\*.jar`
   (of course this also needs the environment variables as described below)

3. To build the docker image, check that there is only one .jar file under `/target/*.jar`, of the correct version.
   In a shell in project root dir, run: `docker build -t diva-dns:<current version> .` (do not forget the `.`).        
   E.g. `docker build -f Dockerfile_smallimage -t diva-dns:0.5.0-SNAPSHOT .`  
   The Dockerfile creates a minimal JRE and then copies that and the existing *.jar file into the image, along with the
   necessary commands to run this jar.

4. Now run the Docker container locally.
    - command is `DIVAEXCHANGE_BASEURL=<divaBaseUrl> docker compose up -d`     
      where <divaBaseUrl> is the URL of a Diva blockchain node, e.g. `DIVAEXCHANGE_BASEURL=http://172.19.69.21:17468 docker compose up -d`.
    - to stop the container, use `DIVAEXCHANGE_BASEURL=<divaBaseUrl> docker compose down`.
    - If the Diva blockchain does not run with standard settings, you can also configure `HOST_BASE_IP`, `BASE_IP` and 
      `PORT` as described in https://github.com/diva-exchange/diva-dockerized#environment-variables .
