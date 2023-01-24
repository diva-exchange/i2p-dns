package ch.hslu.tfbirrer.divadns.utils

class AlreadyRegisteredException(message: String) : Exception(message)

class DivaClientException(message: String, t: Throwable) : Exception(message, t)