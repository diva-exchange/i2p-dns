package ch.hslu.tfbirrer.divadns.arecord

import ch.hslu.tfbirrer.divadns.divaclient.*
import com.ninjasquad.springmockk.MockkBean
import io.mockk.every
import org.jeasy.random.EasyRandom
import org.junit.jupiter.api.*
import org.junit.jupiter.api.Assertions.*
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.boot.test.context.SpringBootTest
import org.springframework.context.annotation.Import

@SpringBootTest
@Import(ARecordService::class)
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
internal class ARecordServiceTest {

    @MockkBean
    private lateinit var divaExchangeClientMock: DivaExchangeClient

//    @MockkBean
//    private lateinit var tokenServiceMock: TokenService

    @Autowired
    private lateinit var aRecordService: ARecordService

    private val expectedTransactionResult = TransactionResult("someIdent")


    @BeforeEach
    fun setUp() {
        // Arrange a virtual blockchain with 4 data blocks: one for URL "xyz.i2p", and three for URL "abc.i2p", where
        // the middle one is legit and the first and last one are blocks with similar keys (first one: abcdef instead
        // of abc, last one: abc, but after the real one). Of course there would be more blocks, but we need only those
        // returned by the searches. So the entries are (note that ".i2p" is omitted from key):
        // - height 11, key "IIPDNS:abcdef:someOrigin", value "wrong IP for abc 1"
        // - height 32, key "IIPDNS:abc:origin333", value "correct IP for abc"
        // - height 43, key "IIPDNS:xyz:someOrigin", value "correct IP for xyz"
        // - height 54, key "IIPDNS:abc:otherOrigin", value "wrong IP for abc 2"
        val heights = arrayOf(11, 32, 43, 54)
        val origins = arrayOf("someOrigin", "origin333", "someOrigin", "otherOrigin")
        val dataCommands = arrayOf(
            DataCommand(ns = "IIPDNS:abcdef", d = "wrong IP for abcdef"),
            DataCommand(ns = "IIPDNS:abc", d = "correct IP for abc"),
            DataCommand(ns = "IIPDNS:xyz", d = "correct IP for xyz"),
            DataCommand(ns = "IIPDNS:abc", d = "wrong IP for abc")
        )
        val txDataElements = origins.zip(dataCommands).map { pair -> // now origins is pair.first and command is pair.second
            TxDataElement(ident = EasyRandom().nextObject(String::class.java),
                origin = pair.first, commands = listOf(pair.second), sig = EasyRandom().nextObject(String::class.java)
            )
        }
        val dataBlocks: Array<DataBlock> = heights.zip(txDataElements).map { pair ->
            DataBlock(hash = EasyRandom().nextObject(String::class.java), tx = listOf(pair.second), height = pair.first)
        }.toTypedArray()
        val states = origins.zip(dataCommands).map { pair -> // origin is pair.first, data command is pair.second
            State("${pair.second.ns}:${pair.first}", pair.second.d )
        }.toTypedArray()

        val statesForAbc = states.filterIndexed { index, _ -> index != 2 }.associate { it.key to it.value }
        val statesForXyz = states.filterIndexed { index, _ -> index == 2 }.associate { it.key to it.value }
        val blocksForAbc: List<DataBlock> = dataBlocks.filterIndexed { index, _ -> index != 2 }
        val blocksForXyz: List<DataBlock> = dataBlocks.filterIndexed { index, _ -> index == 2 }
        // with MockK, the most general setup must be first, will be overwritten by more specific ones
        every { divaExchangeClientMock.searchStates(any()) }.returns(emptyMap())
        every { divaExchangeClientMock.searchStates(eq("IIPDNS:abc")) }.returns(statesForAbc)
        every { divaExchangeClientMock.searchStates(eq("IIPDNS:xyz")) }.returns(statesForXyz)
        every { divaExchangeClientMock.searchBlocks(any()) }.returns(emptyList())
        every { divaExchangeClientMock.searchBlocks(eq("IIPDNS:abc")) }.returns(blocksForAbc)
        every { divaExchangeClientMock.searchBlocks(eq("IIPDNS:xyz")) }.returns(blocksForXyz)

        every { divaExchangeClientMock.putData(any()) }.returns(expectedTransactionResult)
    }

    @Test
    fun getARecord_onceInChain() {
        // act
        val foundIp = aRecordService.getARecord("xyz.i2p")
        // assert
        assertEquals("correct IP for xyz", foundIp)
    }

    @Test
    fun getARecord_multiInChain() {
        // act
        val foundIp = aRecordService.getARecord("abc.i2p")
        // assert
        assertEquals("correct IP for abc", foundIp)
    }

    @Test
    fun getARecord_notInChain() {
        // act
        val foundIp = aRecordService.getARecord("unknown.i2p")
        // assert
        assertNull(foundIp)
    }


    @Test
    fun hasARecord_onceInChain() {
        // act & assert
        assertTrue(aRecordService.hasARecord("xyz.i2p"))
    }

    @Test
    fun hasARecord_multiInChain() {
        // act & assert
        assertTrue(aRecordService.hasARecord("abc.i2p"))
    }

    @Test
    fun hasARecord_notInChain() {
        // act & assert
        assertFalse(aRecordService.hasARecord("unknown.i2p"))
    }


    @Test
    fun addARecord_success() {
        // act
        val newHostname = "newHost.i2p"
        val newIp = "newIp"
        val result = aRecordService.addARecord(newHostname, newIp)
        // assert
        assertNotNull(result)
        assertTrue(result!!.contains("OK"))
        assertTrue(result.contains(expectedTransactionResult.ident))
        assertTrue(result.contains(newHostname))
        assertTrue(result.contains(newIp))
    }

    @Test
    fun addARecord_noHost() {
        // act & assert
        assertThrows<IllegalArgumentException> {
            aRecordService.addARecord("", "newIp")
        }
    }

    @Test
    fun addARecord_noIp() {
        // act & assert
        assertThrows<IllegalArgumentException> {
            aRecordService.addARecord("newHost.i2p", "")
        }
    }


    @Test
    fun domainnameToKey() {
        // arrange
        val domainName = "some-thing-55_long.i2p"
        val expected = "IIPDNS:some-thing-55_long"

        // act & assert
        assertEquals(expected, aRecordService.domainnameToKey(domainName))
    }

}