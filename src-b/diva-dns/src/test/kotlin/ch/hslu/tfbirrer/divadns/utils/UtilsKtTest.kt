package ch.hslu.tfbirrer.divadns.utils

import org.junit.jupiter.api.Test

import org.junit.jupiter.api.Assertions.*

class UtilsKtTest {

    @Test
    fun toB32() {
        // same test as in implementation in diva-exchange:
        // https://github.com/diva-exchange/i2p-sam/blob/a509b15e22a16df24f417371ac60aee9effddc08/test/i2p-sam.spec.ts#L18
        // arrange
        val expected = "z3v47ifwlen474b5aprlf52k6ixa5fpnu5aamuxnulr2qhagrvmq"
        val s = "-hX6726R7xIX0Bvb9eKZlADgwCquImj8950Sy1zrrJK5kMFd0jHXXD3ky8iWLYmRi-MN3obBC2z4s0E1Bsl~EfVtWEAou9dlK7OnW9pbDIxQu6p1yRPBzHNdBM5jTWplZkx5VBL63FsjhIpDRBhTqGUaLFyT40jwD92ks4uAUpZkQwTeNmc9pbWAro6T2SXgVdDTF5U~8Hk9N~-126hlfATDikoPjUiFr0KD1Yi5~ufWxTwzifHwYmb6SGcBUiKc9L8wFuPOAchH33vBTmAGBoyhZkhWLRjIiQKpE9U5W4LcnrLs2rB40c5F0--esAKUCHA59I~FQXtzbtSbHoFVvYjIHJNGp6UP-CmJWCJs2be9XVI5ltFaiKK6qH7n3p0vKfiJeh43CqKaubX5s93LXNsl~qlil~92T~58FRL36-4FpfXo0AoSJiGgG3kvnB7cJoI2Owjw5oRE7UoXHLFXr8MUpBYqAcsCt3d1tsoHfA1r2bNSuITynWJUWYBDMTocBQAEAAcAAA=="
        // act & assert
        assertEquals(expected, toB32(s))
    }



    @Test
    fun isValidDomainName_short() {
        assert(isValidDomainName("abc.i2p"))
    }
    @Test
    fun isValidDomainName_blank() {
        assert(!isValidDomainName(""))
    }
    @Test
    fun isValidDomainName_tooShort() {
        assert(!isValidDomainName("ab.i2p"))
    }
    @Test
    fun isValidDomainName_long() {
        assert(isValidDomainName("abcdefghijklmnopqrstuvwxyz_0123456789-abcdefghijklmnopqrstuvwxyz.i2p"))
    }
    @Test
    fun isValidDomainName_tooLong() {
        assert(!isValidDomainName("-abcdefghijklmnopqrstuvwxyz_0123456789-abcdefghijklmnopqrstuvwxyz.i2p"))
    }

    @Test
    fun isValidDomainName_wrongChars1() {
        assert(!isValidDomainName("ABC.i2p"))
    }
    @Test
    fun isValidDomainName_wrongChars2() {
        assert(!isValidDomainName("ab:cd.i2p"))
    }

    @Test
    fun isValidContent_ok() {
        assert(isValidContent("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"))
    }
    @Test
    fun isValidContent_tooLong() {
        assert(!isValidContent("aabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"))
    }
    @Test
    fun isValidContent_tooShort() {
        assert(!isValidContent("bcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"))
    }
    @Test
    fun isValidContent_wrongChars() {
        assert(!isValidContent("ABCdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"))
    }

}