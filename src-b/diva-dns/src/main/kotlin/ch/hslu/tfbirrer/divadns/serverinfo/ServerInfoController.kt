package ch.hslu.tfbirrer.divadns.serverinfo

import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.beans.factory.annotation.Value
import org.springframework.boot.info.BuildProperties
import org.springframework.core.env.Environment
import org.springframework.http.HttpStatus
import org.springframework.http.ResponseEntity
import org.springframework.stereotype.Controller
import org.springframework.web.bind.annotation.GetMapping

@Controller
class ServerInfoController @Autowired constructor(private val buildProperties: BuildProperties) {
    private val log = LoggerFactory.getLogger(this.javaClass)

    @Autowired
    private lateinit var environment: Environment

    @GetMapping(value = ["/info"], produces = ["text/html"])
    fun serverInfo(
        @Value("\${divaexchange.baseUrl}") baseUrl: String,
    ): ResponseEntity<String> {
        log.debug("in /info")

        val stringBuilder =
                    "DIVAEXCHANGE_BASEURL: $baseUrl" + "<br/><br/>" +
                    "Artifactname: " + buildProperties.name + "<br/>" +
                    "Version: " + buildProperties.version + "<br/>" +
                    "Date/Time: " + buildProperties.time + "<br/>" +
                    "Artifact ID: " + buildProperties.artifact + "<br/>" +
                    "Group ID: " + buildProperties.group + "<br/>" +
                    "Profiles: " + environment.activeProfiles.contentToString() + "<br/><br/>"
        return ResponseEntity.ok().body(stringBuilder)
    }

}
