namespace diva_dns.Util
{
    public static class DomainNameExtension
    {
        /// <summary>
        /// Convert a domain name to the format of Diva API v34.
        /// Workaround received from Samuel Abaecherli, Pascal Knecht.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string ConvertToV34(this string domainName)
        {
            return domainName.Replace(".i2p", ":i2p_");
        }

        /// <summary>
        /// Convert a domain name from the format of Diva API v34.
        /// Workaround received from Samuel Abaecherli, Pascal Knecht.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string ConvertFromV34(this string domainName)
        {
            return domainName.Replace(":i2p_", ".i2p");
        }

        /// <summary>
        /// Add the ip2 dns prefix.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string AddI2pPrefix(this string domainName)
        {
            return $"IIPDNS:{domainName}";
        }
    }
}
