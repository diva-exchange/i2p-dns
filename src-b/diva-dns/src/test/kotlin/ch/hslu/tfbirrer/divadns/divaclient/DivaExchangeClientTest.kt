package ch.hslu.tfbirrer.divadns.divaclient

import ch.hslu.tfbirrer.divadns.utils.DivaClientException
import org.junit.jupiter.api.*
import org.mockserver.client.MockServerClient
import org.mockserver.integration.ClientAndServer
import org.mockserver.integration.ClientAndServer.startClientAndServer
import org.mockserver.model.HttpRequest
import org.mockserver.model.HttpResponse
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.boot.test.context.SpringBootTest
import org.mockserver.model.MediaType


@SpringBootTest
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
internal class DivaExchangeClientTest {

    private lateinit var mockServer: ClientAndServer

    @Autowired
    private lateinit var divaExchangeClient: DivaExchangeClient

    private val exampleDataCommands = "[{\"seq\": 1, \"command\": \"data\", \"ns\": \"testnet:explorer:diva:exchange\", \"d\": \"blatest1\"}]"

    private val exampleDataBlock = "{\n" +
            "            \"version\": 5,\n" +
            "            \"previousHash\": \"7ChNRls4E2z1CuCTXhp-rXOv_90FuGTeA9EM3kejOjE\",\n" +
            "            \"hash\": \"2WUfLPr97cqS9GaAonXrheBqeeuJ4tljfrhiHnZZEm0\",\n" +
            "            \"tx\": [\n" +
            "            {\n" +
            "                \"ident\": \"_4DCMdqM\",\n" +
            "                \"origin\": \"EKW1H8537sMF2iyca-TcJjwHoCfYwRbH0y0cJNj5qqg\",\n" +
            "                \"commands\": " + exampleDataCommands + ",\n" +
            "                \"sig\": \"NgEyZkEN3NnL91Fi_6bi3ynl1pfGIL1I5cEbMKfUzfMbxeVqVHwnnKhz54oIQyPjgc7O0x8QhzfvW3n4MDJoCw\"\n" +
            "            }\n" +
            "            ],\n" +
            "            \"height\": 14,\n" +
            "            \"votes\": [\n" +
            "            {\n" +
            "                \"origin\": \"G7pJLdWtf4SuMuswVBa_ZVlxYhC3neaUBzMditUV1II\",\n" +
            "                \"sig\": \"zPg7-Xn9lKlPlS9QvBmo7AXSQSh9Tqo4-om8wLW73VM4anZfYvX9GMJX_OA5feSvY3ie5TpY7yuRh24M2fFgDg\"\n" +
            "            },\n" +
            "            {\n" +
            "                \"origin\": \"yxiF5aL-VDNo_ZW5M61tPE_Ccz7793yD3IoJVU6eG1A\",\n" +
            "                \"sig\": \"w4uetGyrawI19Mx77dg-4rLVP6tPb1Gt23QpkA3CWCCy3NDhvYRUfWtwDVqhiZmdNYFc4aRZYzGZKy5ugTUsDw\"\n" +
            "            }\n" +
            "            ]\n" +
            "        }"

    private val exampleDataBlocklist = "[" + exampleDataBlock + "]"

    // same as in the exampleDataCommands:
    private val expectedCommand = listOf(DataCommand(ns = "testnet:explorer:diva:exchange", d = "blatest1"))

    private val state1 = State("abc:123:something", "Value 1")
    private val state2 = State("abc:456:whatevers", "Value 2")
    private val state3 = State("xyz:123:irgendwas", "Value 3")

    private val exampleIdent = TransactionResult("A8TCPiVr")


    @BeforeAll
    fun setUp() {
        mockServer = startClientAndServer(10080)
        
        setUpSearchResponses()
        setUpStateResponses()
    }

    @AfterAll
    fun tearDown() {
        mockServer.stop()
    }


    /**
     * Set up responses for state search:
     * <ul>
     *     <li>search for "abc" finds state1 and state2</li>
     *     <li>search for "123" finds state1 and state3</li>
     *     <li>search for "xyz" finds state3</li>
     *     <li>all other searches find nothing (empty list)</li>     
     * </ul>     
     */
    private fun setUpSearchResponses() {
        add200Response("/state/search/abc", "[${state1.toString()},${state2.toString()}]")
        add200Response("/state/search/123", "[${state1.toString()},${state3.toString()}]")
        add200Response("/state/search/xyz", "[${state3.toString()}]")
        add200Response("/state/search/.*", "[]")    // response for all other requests is empty list
    }

    /**
     * Set up responses for state search:
     * <ul>
     *     <li>search for "abc" finds state1 and state2</li>
     *     <li>search for "123" finds state1 and state3</li>
     *     <li>search for "xyz" finds state3</li>
     *     <li>all other searches find nothing (empty list)</li>
     * </ul>
     */
    private fun setUpStateResponses() {
        add200Response("/state/${state1.key}", "$state1")
        add200Response("/state/${state2.key}", "$state2")
        add200Response("/state/${state3.key}", "$state3")
        add404Response("/state/.*", "")    // response for all other requests is 404 and empty??? TODO
    }


    /** adds a status 200 response with the given JSON body to the mock server */
    private fun add200Response(path: String, responseBody: String) {
        MockServerClient("localhost", 10080)    // must correspond to setting in test application.properties
            .`when`( //<-- back ticks needed since 'when' is keyword in kotlin...
                HttpRequest.request()
                    .withMethod("GET")
                    .withPath(path)
            )
            .respond(
                HttpResponse.response()
                    .withStatusCode(200)
                    .withContentType(MediaType.APPLICATION_JSON)
                    .withBody(responseBody)
            )
    }

    private fun add404Response(path: String, responseBody: String) {
        MockServerClient("localhost", 10080)    // must correspond to setting in test application.properties
            .`when`( //<-- back ticks needed since 'when' is keyword in kotlin...
                HttpRequest.request()
                    .withMethod("GET")
                    .withPath(path)
            )
            .respond(
                HttpResponse.response().withStatusCode(404).withBody(responseBody)
            )
    }


    @Test
    fun getState1() {
        // arrange: done in common setUp
        // act
        val response = this.divaExchangeClient.getState(state1.key)
        // assert
        assert(response == state1)
    }

    @Test
    fun getState_nonexisting() {
        // arrange: done in common setUp
        // act & assert
        assertThrows<DivaClientException> {
            this.divaExchangeClient.getState("nonexistingKey")
        }
    }


    @Test
    fun searchStates1() {
        // this tests the call and mainly the deserialization of the data types
        // arrange: done in common setUp
        // act
        val responseMap = this.divaExchangeClient.searchStates("abc")
        // assert
        assert(responseMap.size == 2)
        assert(responseMap[state1.key] == state1.value)
        assert(responseMap[state2.key] == state2.value)
    }

    @Test
    fun searchStates2() {
        // arrange: done in common setUp
        // act
        val responseMap = this.divaExchangeClient.searchStates("123")
        // assert
        assert(responseMap.size == 2)
        assert(responseMap[state1.key] == state1.value)
        assert(responseMap[state3.key] == state3.value)
    }

    @Test
    fun searchStatesEmpty() {
        // arrange: done in common setUp
        // act
        val responseMap = this.divaExchangeClient.searchStates("unknown")
        // assert
        assert(responseMap.isEmpty())
    }


    @Test
    fun putData() {
        // arrange: respond with ident when a data block is put
        MockServerClient("localhost", 10080)    // must correspond to setting in test application.properties
            .`when`( //<-- back ticks needed since 'when' is keyword in kotlin...
                HttpRequest.request()
                    .withMethod("PUT")
                    .withContentType(MediaType.APPLICATION_JSON)
                    .withPath("/transaction")
//                    .withBody(exampleDataCommands)
            )
            .respond(
                HttpResponse.response()
                    .withStatusCode(200)
                    .withContentType(MediaType.APPLICATION_JSON)
                    .withBody(exampleIdent.toString())
            )
        // act
        val response = this.divaExchangeClient.putData(expectedCommand)
        // assert
        assert(response == exampleIdent)
    }


    @Test
    fun getBlockByNumber() {
        // this tests the call and mainly the deserialization of the data types
        // arrange:
        // - Tell the MockServer to respond with HTTP 200 and a valid response (hardcoded) when making a GET Request to /block/14 .
        MockServerClient("localhost", 10080)    // must correspond to setting in test application.properties
            .`when`( //<-- back ticks needed since 'when' is keyword in kotlin...
                HttpRequest.request()
                    .withMethod("GET")
                    .withPath("/block/14")
            )
            .respond(
                HttpResponse.response()
                    .withStatusCode(200)
                    .withContentType(MediaType.APPLICATION_JSON)
                    .withBody(
                        exampleDataBlock
                    )
            )
        // act
        val responseValue = this.divaExchangeClient.getBlockByNumber(14)
        // assert
        assert(responseValue != null)
        assertIsExampleBlock(responseValue!!)
    }

    @Test
    fun searchBlocks() {
        // this tests the call and mainly the deserialization of the data types
        // arrange:
        // - Tell the MockServer to respond with HTTP 200 and a valid response (hardcoded) when making a GET Request to /blocks/search/blatest
        MockServerClient("localhost", 10080)    // must correspond to setting in test application.properties
            .`when`( //<-- back ticks needed since 'when' is keyword in kotlin...
                HttpRequest.request()
                    .withMethod("GET")
                    .withPath("/blocks/search/blatest")
            )
            .respond(
                HttpResponse.response()
                    .withStatusCode(200)
                    .withContentType(MediaType.APPLICATION_JSON)
                    .withBody(exampleDataBlocklist)
            )
        // act
        val responseList = this.divaExchangeClient.searchBlocks("blatest")
        // assert
        assert(responseList.size == 1)
        assertIsExampleBlock(responseList[0])
    }


    /**
     * Compares the given block to the example block and expected command
     */
    private fun assertIsExampleBlock(responseBlock: DataBlock) {

        assert(responseBlock.hash == "2WUfLPr97cqS9GaAonXrheBqeeuJ4tljfrhiHnZZEm0")
        assert(responseBlock.height == 14)
        assert(responseBlock.tx.size == 1)

        val tx0: TxDataElement = responseBlock.tx[0]
        assert(tx0.ident == "_4DCMdqM")
        assert(tx0.commands.size == 1)

        val command = tx0.commands[0]
        assert(command == expectedCommand.first())
    }

}
