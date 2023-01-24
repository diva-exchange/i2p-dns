package ch.hslu.tfbirrer.divadns.divaclient

import org.slf4j.LoggerFactory
import org.springframework.stereotype.Service

@Service
class TokenService(
) {

    private val log = LoggerFactory.getLogger(this.javaClass)


    /**
     * Returns the currently valid token for the DivaExchange (current implementation is read from a file).
     * @return token, or null if it could not be obtained.
     */
    fun getToken(): String? {
        // TODO read from file (currently, it seems not to be necessary, the transaction works without the token in the header)
        log.debug("getToken called")
        return "dummyToken"
    }

}
