package ch.hslu.tfbirrer.divadns.divaclient

import com.fasterxml.jackson.annotation.JsonIgnoreProperties


@JsonIgnoreProperties(ignoreUnknown = true)
data class DataCommand(
    val seq: Int = 1,
    val command: String = "data",
    val ns: String,
    val d: String,
) {}


@JsonIgnoreProperties(ignoreUnknown = true)
data class TxDataElement(
    val ident: String,
    val origin: String,
    val commands: List<DataCommand>,    // could also be other commands (we could use subtypes), but our calls only return data blocks, so DataCommand should suffice
    val sig: String,
) {}

@JsonIgnoreProperties(ignoreUnknown = true)
data class DataBlock(
    val hash: String,
    val tx: List<TxDataElement>,
    val height: Int,
) {}

@JsonIgnoreProperties(ignoreUnknown = true)
data class State(
    val key: String,
    val value: String,
) {
    override fun toString(): String = "{\"key\": \"$key\", \"value\": \"$value\"}"
}

@JsonIgnoreProperties(ignoreUnknown = true)
data class TransactionResult(
    val ident: String,
) {
    override fun toString(): String = "{\"ident\": \"${ident}\"}"
}

