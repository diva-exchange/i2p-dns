package ch.hslu.tfbirrer.divadns.utils

import org.apache.commons.codec.binary.Base32   // Base32 encoding/decoding as defined by RFC 4648 (https://commons.apache.org/proper/commons-codec/apidocs/index.html)
import java.security.MessageDigest
import java.util.*      // for Base64


const val DOMAINNAME_PATTERN = """[a-z0-9-_]{3,64}\.i2p$"""    // use raw string so not to worry about double escaping chars
const val B32STRING_PATTERN = "[a-z0-9]{52}$"


/**
 * Test if the given string fits the pattern for domain names ([a-z0-9-_]{3,64}\.i2p$)
 */
fun isValidDomainName(domainName: String): Boolean {
    return DOMAINNAME_PATTERN.toRegex().matches(domainName)
}

/**
 * Test if the given string fits the pattern for domain content ("b32String: ([a-z0-9-_]{58}$)
 * (It does not test if the string really is valid according to b32 encoding.)
 */
fun isValidContent(content: String): Boolean {
    return B32STRING_PATTERN.toRegex().matches(content)
}

/**
 * Definition of B32-String for this application:
 * <code>
 * static toB32(base64Destination: string): string {
 *     const s: Buffer = Buffer.from(base64Destination.replace(/-/g, '+').replace(/~/g, '/'), 'base64');
 *     return base32.stringify(crypto.createHash('sha256').update(s).digest(), { pad: false }).toLowerCase();
 * }
 * </code>
 */
fun toB32(s: String): String {
    val filtered = s.replace('-', '+').replace('~', '/')
    val bytes = MessageDigest.getInstance("SHA-256").digest(Base64.getDecoder().decode(filtered))
    return Base32().encodeToString(bytes).substringBefore('=').lowercase()
}
