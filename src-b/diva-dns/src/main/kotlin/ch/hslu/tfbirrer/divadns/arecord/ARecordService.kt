package ch.hslu.tfbirrer.divadns.arecord

import ch.hslu.tfbirrer.divadns.divaclient.DataCommand
import ch.hslu.tfbirrer.divadns.divaclient.DivaExchangeClient
import ch.hslu.tfbirrer.divadns.utils.AlreadyRegisteredException
import ch.hslu.tfbirrer.divadns.utils.DivaClientException
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.stereotype.Service

const val DIVADNS_DATAPREFIX = "IIPDNS:"

/**
 * This service contains the logic to convert domain names and addresses into a format that can be stored on the
 * Diva Exchange blockchain. Domain names look like "diva.i2p", always with a ".i2p" suffix.
 *
 * Currently (in version 0.34.x) the namespace of the blockchain must comply with the following regex:
 * "^([A-Za-z_-]{4,15}:){1,4}[A-Za-z0-9_-]{1,64}$".
 * Specifically: digits may only occur in the last part of the namespace, and all parts before the last must be at
 * least 4 characters long. Therefore, using the namespace as specified "I2PDNS:[domain-name.i2p]" does not work,
 * the digit is not allowed.
 * So I use the following format as a workaround:
 * - Use prefix "IIPDNS" instead of "I2PDNS".
 * - Use the last part of the namespace for the domain name, omitting the suffix ".i2p". This way, the domain-name can
 *   also contain digits and be up to 64 characters long (and we have no problem with the fact that "i2p" is too short).
 */
@Service
class ARecordService(
    @Autowired private val divaClient: DivaExchangeClient,
//    @Autowired private val tokenService: TokenService,
) {

    private val log = LoggerFactory.getLogger(this.javaClass)


    /**
     * Returns the valid entry in the DivaDNS that was entered first for the given domain name, or null.
     * @param domainName domain name (in plain text, must not to be blank)
     * @return address as String, null if domain name not yet registered.
     * @throws DivaClientException if error occurs while calling the DivaChain
     * @throws IllegalArgumentException if domain name is blank
     */
    fun getARecord(domainName: String): String? {
        log.debug("getARecord called for domainName '{}'", domainName)
        return getOldestARecord(domainName)
    }

    /**
     * Check if domain name already registered in the queue. Is used internally before registering a new value,
     * but can also be used externally.
     * @param domainName domainName (must not be blank)
     * @return true if domainName is already registered
     * @see getARecord
     */
    fun hasARecord(domainName: String): Boolean {
        log.debug("hasARecord called for domain name '{}'", domainName)
        return getOldestARecord(domainName) != null
    }

    /**
     * Returns the oldest entry in the chain that was entered for the given domain name.
     * TODO LATER: when decision blocks are available, this function needs to be changed to getting a specific value instead of a search
     * @param domainName domain name
     * @return stored value, or null if not found
     * @throws DivaClientException if error occurs while calling the DivaChain
     * @throws IllegalArgumentException if domainName is blank
     */
    private fun getOldestARecord(domainName: String): String? {
        if (domainName.isBlank())
            throw IllegalArgumentException("domain name must not be blank")

        val searchKey = domainnameToKey(domainName)
        val storedForHostnameMap = this.divaClient.searchStates(searchKey)

        if (storedForHostnameMap.isEmpty())
            return null

        // Due to the implementation, the searchKey has been found somewhere in the ns field (could be not the start,
        // or the key could be longer). So filter found entries: keep those where the key matches from the start and up
        // to a colon. Then return the first entry (we don't care about age of entries and assume that there are no
        // "fake entries" - implementation later will be different anyway).
        return storedForHostnameMap
            .filterKeys { key -> key.startsWith("$searchKey:") }
            .firstNotNullOfOrNull { it }?.value
    }

    /**
     * Add a block with the domain name and the b32-string according to the following JSON-representation:
     * <code>
     * {
     *   seq: number;       // Prototype, set to 1
     *   command: string;   // "decision"     // TODO LATER not yet possible, for now we use "data"
     *   ns: string;        // "I2PDNS:[domain-name]"
     *   h: number;         // current Blockchain Height + 25       // TODO LATER not used in data blocks
     *   d: string;         // Format: "domain-name=b32-string"
     * }
     * </code>
     *
     * @param domainName domain name
     * @param b32String  destination for domainName
     * @return a non-null string containing the ident of the new block and the two saved values
     * @throws AlreadyRegisteredException if an entry for domain name already exists
     * @throws IllegalArgumentException if domain name or ip is blank
     * @throws DivaClientException  if an error occurred while calling the blockchain
     */
    fun addARecord(domainName: String, b32String: String): String? {
        log.debug("addARecord called...")
        if (domainName.isBlank() || b32String.isBlank())
            throw IllegalArgumentException("domainName or b32String must not be blank")
        if (hasARecord(domainName))
            throw AlreadyRegisteredException("domain $domainName has already been registered")

        val ident = putARecord(domainName, b32String)   // this can throw DivaClientException
        // ident should not be null, but technically it could
        return "OK - $ident: $domainName = $b32String"
    }

    /** how domain names are stored in the chain data */
    fun domainnameToKey(domainName: String): String {
        return "$DIVADNS_DATAPREFIX${domainName.removeSuffix(".i2p")}"
    }


    /**
     * @param domainName domain name (not encoded), must be checked not to be blank
     * @param b32String     the path value for the domainName, must be checked not to be blank
     * @return  the ident of the new block, null if failed
     * @throws DivaClientException  if an error occurred while calling the blockchain
     */
    private fun putARecord(domainName: String, b32String: String): String? {
        val dataValue = "$domainName=$b32String"
        val dataCommand = listOf(
            DataCommand(
                ns = domainnameToKey(domainName),
                // h = height  // TODO LATER height not yet used
                d = dataValue
            )
        )
        val result = this.divaClient.putData(dataCommand)
        return result?.ident
    }

}
