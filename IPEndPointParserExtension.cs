#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Summary description for IPEndPointParserExtension
    /// https://stackoverflow.com/a/72050494
    /// </summary>
    public static class IPEndPointParserExtension
    {
        public static bool TryParseAsIPEndPoint(this string s, out IPEndPoint result)
        {
#if NETCOREAPP3_0_OR_GREATER
            return IPEndPoint.TryParse(s, out result);
#else
            int addressLength = s.Length;  // If there's no port then send the entire string to the address parser
            int lastColonPos = s.LastIndexOf(':');
            // Look to see if this is an IPv6 address with a port.
            if (lastColonPos > 0)
            {
                if (s[lastColonPos - 1] == ']')
                    addressLength = lastColonPos;
                // Look to see if this is IPv4 with a port (IPv6 will have another colon)
                else if (s.Substring(0, lastColonPos).LastIndexOf(':') == -1)
                    addressLength = lastColonPos;
            }
            if (IPAddress.TryParse(s.Substring(0, addressLength), out IPAddress address))
            {
                long port = 0;
                if (addressLength == s.Length ||
                    (long.TryParse(s.Substring(addressLength + 1), out port)
                        && port <= IPEndPoint.MaxPort))
                {
                    result = new IPEndPoint(address, (int)port);
                    return true;
                }
            }
            result = null;
            return false;
#endif
        }

        public static IPEndPoint AsIPEndPoint(this string s) =>
            s.TryParseAsIPEndPoint(out var endpoint)
                ? endpoint
                : throw new FormatException($"'{s}' is not a valid IP Endpoint");

    }
}
