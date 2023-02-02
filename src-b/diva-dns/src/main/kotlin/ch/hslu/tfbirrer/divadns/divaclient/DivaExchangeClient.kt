package ch.hslu.tfbirrer.divadns.divaclient

import ch.hslu.tfbirrer.divadns.utils.DivaClientException
import com.fasterxml.jackson.databind.ObjectMapper
import com.fasterxml.jackson.module.kotlin.registerKotlinModule
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Value
import org.springframework.http.HttpHeaders
import org.springframework.http.MediaType
import org.springframework.stereotype.Service
import org.springframework.web.reactive.function.BodyInserters
import org.springframework.web.reactive.function.client.*
import java.util.*


@Service
class DivaExchangeClient (
    @Value("\${divaexchange.baseUrl}") private val baseUrl: String,
    )
{
    private val log = LoggerFactory.getLogger(this.javaClass)
    private val client = WebClient.create(baseUrl)
    private val mapper = ObjectMapper().apply { registerKotlinModule() }


    /**
     * Get the state for an exact key.
     * @param exactKey      key to use
     * @return  the found state (key/value pair), or null if not found or the exactKey is blank
     * @throws DivaClientException  if an error occurred while calling the blockchain
     */
    fun getState(exactKey: String): State? {
        if (exactKey.isBlank())
            return null
        // get rid of potentially dangerous characters
        val encodedKey = exactKey.replace('"', '_').replace('$', '_')
        log.debug("get diva-chain state for key '{}' (from client {})", encodedKey, baseUrl)
        try {
            return client
                .get()
                .uri("/state/$encodedKey")
                .accept(MediaType.APPLICATION_JSON)
                .retrieve()
                .bodyToMono<State>()
                .block()
        } catch (e: WebClientResponseException) {
            val msg = "state not found for key '$exactKey'"
            log.error(msg, e)
            throw DivaClientException(msg, e)
        }
    }


    /**
     * Get the list of states (key/value pairs) found for a key or part of a key.
     * @param keyPart  string to look for (key or part of one)
     * @return  a map with found states (key/value pairs), or an empty map if not found
     * @throws DivaClientException  if an error occurred while calling the blockchain
     */
    fun searchStates(keyPart: String): Map<String, String> {
        // get rid of potentially dangerous characters
        val encodedKeyPart = keyPart.replace('"', '_').replace('$', '_')
        log.debug("search diva-chain states for string '{}' (from client {})", encodedKeyPart, baseUrl)
        try {
            val stateFlux = client
                .get()
                .uri("/state/search/$encodedKeyPart")
                .retrieve()
                .bodyToFlux<State>()
            val mutableMap: MutableMap<String, String>? = stateFlux
                .collectMap({ item -> item.key }, { item -> item.value })
                .block()
            return mutableMap?.toMap().orEmpty()

        } catch (e: WebClientResponseException) {
            val msg = "error when searching states, search string '$keyPart'"
            log.error (msg, e)
            throw DivaClientException(msg, e)
        }
    }


    /**
     * Submit a transaction for a data command.
     * @param data  data commmand
     * @return  the result of the transaction (an ident)
     * @throws DivaClientException  if an error occurred while calling the blockchain
     */
    fun putData(data: List<DataCommand>): TransactionResult? {
        log.debug("putData called")
        try {
            return client
                .put()
                .uri("/transaction")
                .header(HttpHeaders.CONTENT_TYPE, MediaType.APPLICATION_JSON_VALUE)
                .body(BodyInserters.fromValue(data))
//                .accept(MediaType.APPLICATION_JSON)
                .retrieve()
                .bodyToMono<TransactionResult>()
                .block()

////            val rawGetResponse: TransactionResult? = client
//            val respEntity = client
//                .put()
//                .uri("/transaction")
//                .bodyValue(data)
////                .body(BodyInserters.fromValue(data))
//                .retrieve()
////                .toEntity(TransactionResult::class.java)
//                .toEntity(String::class.java)
//            val rawGetResponse: String? = respEntity
//                .block()?.body
//            return TransactionResult(rawGetResponse!!)
        } catch (e: WebClientResponseException) {
            val msg = "could not put data: '${data}'"
            log.error (msg, e)
            throw DivaClientException(msg, e)
        }
    }


    /**
     * UNUSED
     * Get a data block by its height number.
     * Currently, this method can only handle data blocks.
     * @param height    int > 0 (for heights < 1, there are no blocks)
     * @return block on the given height, or null if it does not exist
     */
    fun getBlockByNumber(height: Int): DataBlock? {
        if (height < 1) return null

        log.debug("get block for height {}", height)
        return client
            .get()
            .uri("/block/$height")
            .retrieve()
//            .toEntity(DataBlock::class.java)
//            .block()?.body
            .bodyToMono<DataBlock>()
            .block()
    }

    /**
     * Search the Blockchain for blocks containing a string in a data block. (This method will only handle data blocks,
     * other blocks will cause errors.)
     * @param searchString the blocks containing the search string.
     * @return the list of data blocks containing the searchString
     * @throws DivaClientException  if an error occurred while calling the blockchain
     */
    fun searchBlocks(searchString: String): List<DataBlock> {
        log.debug("search blocks: searchString '{}'", searchString)        // TODO make sure that slf4j filters strings before logging
        try {
            val mutableList = client
                .get()
                .uri("/blocks/search/$searchString")
                .retrieve()
//                .toEntity(String::class.java)
//                .block()?.body
                .bodyToFlux<DataBlock>()
                .collectList()
                .block()
            return mutableList?.toList().orEmpty()

        } catch (e: WebClientResponseException) {
            val msg = "error while searching blocks for searchString '${searchString}'"
            log.error (msg, e)
            throw DivaClientException(msg, e)
        }
    }

}