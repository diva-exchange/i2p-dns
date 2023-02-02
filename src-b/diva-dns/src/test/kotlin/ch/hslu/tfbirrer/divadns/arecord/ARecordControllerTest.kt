package ch.hslu.tfbirrer.divadns.arecord

import ch.hslu.tfbirrer.divadns.utils.AlreadyRegisteredException
import com.ninjasquad.springmockk.MockkBean
import io.mockk.every
import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Test
import org.springframework.beans.factory.annotation.Autowired
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest
import org.springframework.http.MediaType
import org.springframework.test.web.servlet.MockMvc
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders
import org.springframework.test.web.servlet.result.MockMvcResultMatchers

@WebMvcTest(ARecordController::class)
internal class ARecordControllerTest {

    @Autowired
    private lateinit var mvc: MockMvc

    @MockkBean
    private lateinit var aRecordServiceMock: ARecordService

    private val EXISTING_DOMAIN = "abcdef.i2p"
    private val EXISTING_VALUE = "someexistingvalue123456789abcdefghijklmnopqrsstuvwxyz"
    private val NEW_DOMAIN = "newdomain.i2p"
    private val NEW_VALUE = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"

    @BeforeEach
    fun setUp() {
        every { aRecordServiceMock.getARecord(any()) }.returns(null)
        every { aRecordServiceMock.getARecord(eq("")) }.throws(IllegalArgumentException())
        every { aRecordServiceMock.getARecord(eq(EXISTING_DOMAIN)) }.returns(EXISTING_VALUE)

        every { aRecordServiceMock.addARecord(eq(""), any()) }.throws(IllegalArgumentException())
        every { aRecordServiceMock.addARecord(any(), eq("")) }.throws(IllegalArgumentException())
        every { aRecordServiceMock.addARecord(eq(EXISTING_DOMAIN), any()) }.throws(AlreadyRegisteredException("host $EXISTING_DOMAIN has already been registered"))
        every { aRecordServiceMock.addARecord(eq(NEW_DOMAIN), eq(NEW_VALUE)) }
            .returns("OK - someIdent: $NEW_DOMAIN = $NEW_VALUE")
    }

    @Test
    fun getExistingEntryReturnsCorrectValue() {
        // act & assert
        val mvcResult = mvc.perform(
            MockMvcRequestBuilders
                .get("/${EXISTING_DOMAIN}")
                .contentType(MediaType.APPLICATION_JSON)
        )
            .andExpect(MockMvcResultMatchers.status().isOk)
            .andExpect(MockMvcResultMatchers.content().contentType("text/plain;charset=UTF-8"))
            .andReturn()
        val bodyOfResult = mvcResult.response.contentAsString
        assert (bodyOfResult == EXISTING_VALUE)
    }

    @Test
    fun getNonexistingEntryReturnsCorrectValue() {
        // act & assert
        val mvcResult = mvc.perform(
            MockMvcRequestBuilders
                .get("/nonexisting-host.i2p")
                .contentType(MediaType.APPLICATION_JSON)
        )
            .andExpect(MockMvcResultMatchers.status().isOk)
            .andExpect(MockMvcResultMatchers.content().contentType("text/plain;charset=UTF-8"))
            .andReturn()
        val bodyOfResult = mvcResult.response.contentAsString
        assert (bodyOfResult == "")
    }

    @Test
    fun putNewEntryReturnsOk() {
        // act & assert
        val mvcResult = mvc.perform(
            MockMvcRequestBuilders
                .put("/${NEW_DOMAIN}/${NEW_VALUE}")
                .contentType(MediaType.APPLICATION_JSON)
        )
            .andExpect(MockMvcResultMatchers.status().isOk)
            .andExpect(MockMvcResultMatchers.content().contentType("text/plain;charset=UTF-8"))
            .andReturn()
        val bodyOfResult = mvcResult.response.contentAsString
        assert (bodyOfResult.contains(NEW_DOMAIN))
        assert (bodyOfResult.contains(NEW_VALUE))
    }

    @Test
    fun putExistingEntryReturnsNotAllowed() {
        // act & assert
        val mvcResult = mvc.perform(
            MockMvcRequestBuilders
                .put("/${EXISTING_DOMAIN}/${NEW_VALUE}")
                .contentType(MediaType.APPLICATION_JSON)
        )
            .andExpect(MockMvcResultMatchers.status().isForbidden)
            .andExpect(MockMvcResultMatchers.content().contentType("text/plain;charset=UTF-8"))
            .andReturn()
        val bodyOfResult = mvcResult.response.contentAsString
        assert (bodyOfResult.contains("already registered"))
    }

    @Test
    fun putInvalidDomainReturnsBadRequest() {
        // act & assert
        val mvcResult = mvc.perform(
            MockMvcRequestBuilders
                .put("/thisIsAnInvalidDomain.i2p/${NEW_VALUE}")
                .contentType(MediaType.APPLICATION_JSON)
        )
            .andExpect(MockMvcResultMatchers.status().isBadRequest)
            .andExpect(MockMvcResultMatchers.content().contentType("text/plain;charset=UTF-8"))
            .andReturn()
        val bodyOfResult = mvcResult.response.contentAsString
        assert (bodyOfResult.contains("does not match the specification"))
    }

    @Test
    fun putInvalidAddressReturnsBadRequest() {
        // act & assert
        val mvcResult = mvc.perform(
            MockMvcRequestBuilders
                .put("/${NEW_DOMAIN}/thisIsAnInvalidValue")
                .contentType(MediaType.APPLICATION_JSON)
        )
            .andExpect(MockMvcResultMatchers.status().isBadRequest)
            .andExpect(MockMvcResultMatchers.content().contentType("text/plain;charset=UTF-8"))
            .andReturn()
        val bodyOfResult = mvcResult.response.contentAsString
        assert (bodyOfResult.contains("does not match the specification"))
    }


}
