package ch.hslu.tfbirrer.divadns.arecord

import ch.hslu.tfbirrer.divadns.utils.AlreadyRegisteredException
import ch.hslu.tfbirrer.divadns.utils.DivaClientException
import ch.hslu.tfbirrer.divadns.utils.isValidContent
import ch.hslu.tfbirrer.divadns.utils.isValidDomainName
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.http.HttpStatus
import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.PutMapping
import org.springframework.web.bind.annotation.RestController


@RestController
class ARecordController(
    @Autowired val aRecordService: ARecordService
) {
    private val log = LoggerFactory.getLogger(this.javaClass)

    /**
     * Get the registered IP address for the given hostname.
     * @param domainName       path, conforming to this pattern: [a-z0-9-_]{3,64}\.i2p$
     * @return response with status 200 and body with the registered IP as text if found,
     *   response with status 200 and empty body if hostname not found,
     *   response with status 400 and body with error message if hostname is blank or not according to pattern
     *   response with status 503 and body with error message if the underlying service had an error
     */
    @GetMapping(
        value = ["/{domain}"],
        produces = ["text/plain"]
    )
    fun getARecord(
        @PathVariable(value="domain") domainName: String
    ): ResponseEntity<String> {
        log.info("GET called for /$domainName")

        if (!isValidDomainName(domainName)) {
            return ResponseEntity("domainName does not match the specification", HttpStatus.BAD_REQUEST)
        }
        return try {
            val result = this.aRecordService.getARecord(domainName).orEmpty()
            ResponseEntity.ok().body(result)
        }
        catch (dce: DivaClientException) {
            // error occurred while calling the DivaChain
            ResponseEntity.status(HttpStatus.SERVICE_UNAVAILABLE).body("error when calling DivaExchange: \n${dce.localizedMessage}")
        }
        catch (iae: IllegalArgumentException) {
            // if hostname is blank
            ResponseEntity.badRequest().body("hostname must not be blank")
        }
    }


    @PutMapping(
        value = ["/{domain}/{address}"],
        consumes = ["application/json"],
        produces = ["text/plain"]
    )
    fun putARecord(
        @PathVariable(value="domain") domainName: String,
        @PathVariable(value="address") address: String
    ): ResponseEntity<String> {
        log.info("PUT called for /$domainName/$address")

        if (!isValidDomainName(domainName)) {
            return ResponseEntity("domainName does not match the specification", HttpStatus.BAD_REQUEST)
        }
        if (!isValidContent(address)) {
            return ResponseEntity("address does not match the specification", HttpStatus.BAD_REQUEST)
        }

        return try {
            val result = this.aRecordService.addARecord(domainName, address)
            ResponseEntity.ok().body(result)
        } catch (iae: IllegalArgumentException) {
            // if hostname or IP is blank
            ResponseEntity.badRequest().body("domain name and IP must not be blank")
        } catch (are: AlreadyRegisteredException) {
            // hostname has already been registered
            ResponseEntity.status(HttpStatus.FORBIDDEN).body("domain name already registered")
        } catch (dce: DivaClientException) {
            // error occurred while calling the DivaChain
            ResponseEntity.status(HttpStatus.FORBIDDEN).body("error when calling DivaExchange: \n${dce}")
        }
    }
}
