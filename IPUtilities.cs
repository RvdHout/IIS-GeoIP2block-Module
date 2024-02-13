#nullable disable
/* IPUtilities.cs
 *
 * Copyright (C) 2009 Triple IT.  All Rights Reserved.
 * Author: Frank Lippes, Modified for IIS 10 (.Net 4.6) by RvdH
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 */

using NetTools;
using System;
using System.Net;
using System.Net.Sockets;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Contains some useful IP address functions
    /// </summary>
    public static class IPUtilities
    {
        /// <summary>
        /// Determines whether or not an IP address lies within a private IP range
        /// </summary>
        /// <param name="ipAddress">The IP address to check</param>
        /// <returns>True if the IP address is a private IP address. False otherwise</returns>
        public static bool IsPrivateIpAddress(IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                //IPv4 Loopback
                var rangeIpv4Loopback = IPAddressRange.Parse("127.0.0.0/8");
                //IPv4 Private
                var rangeIpv4Priv1 = IPAddressRange.Parse("10.0.0.0/8");
                var rangeIpv4Priv2 = IPAddressRange.Parse("172.16.0.0/12");
                var rangeIpv4Priv3 = IPAddressRange.Parse("192.168.0.0/16");
                //IPv4 Link Local
                var rangeIpv4Local = IPAddressRange.Parse("169.254.0.0/16");
                //IPv4 Reserved
                var rangeIpv4Reserved = IPAddressRange.Parse("0.0.0.0/8");

                //Loopback
                if (rangeIpv4Loopback.Contains(ipAddress))
                    return true;
                //Private
                if (rangeIpv4Priv1.Contains(ipAddress) || rangeIpv4Priv2.Contains(ipAddress) || rangeIpv4Priv3.Contains(ipAddress))
                    return true;
                //Reserved
                if (rangeIpv4Reserved.Contains(ipAddress))
                    return true;
                //Link Local
                if (rangeIpv4Local.Contains(ipAddress))
                    return true;
            }
            else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                //IPv6 Loopback
                var rangeIpv6Loopback = IPAddressRange.Parse("::1/128");
                //IPv6 Unique Local
                var rangeIpv6Priv = IPAddressRange.Parse("fc00::/7");
                //IPv6 Link Local
                var rangeIpv6Local = IPAddressRange.Parse("fe80::/10");

                //Loopback
                if (rangeIpv6Loopback.Contains(ipAddress))
                    return true;
                //Unique Local
                if (rangeIpv6Priv.Contains(ipAddress))
                    return true;
                //Link Local
                if (rangeIpv6Local.Contains(ipAddress))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether two IP addresses are in the same subnet
        /// </summary>
        /// <param name="ipAddress1">IP address 1</param>
        /// <param name="ipAddress2">IP address 2</param>
        /// <param name="subnetMask">The subnet</param>
        /// <returns>True if the IP addresses are within the same subnet. False otherwise</returns>
        public static bool IsInSameSubnet(string ipAddress1, string ipAddress2, string subnetMask)
        {
            return IsInSameSubnet(IPAddress.Parse(ipAddress1), ipAddress2, subnetMask);
        }

        /// <summary>
        /// Determines whether two IP addresses are in the same subnet
        /// </summary>
        /// <param name="ipAddress1">IP address 1</param>
        /// <param name="ipAddress2">IP address 2</param>
        /// <param name="subnetMask">The subnet</param>
        /// <returns>True if the IP addresses are within the same subnet. False otherwise</returns>
        public static bool IsInSameSubnet(IPAddress ipAddress1, string ipAddress2, string subnetMask)
        {
            var range = IPAddressRange.Parse(ipAddress2 + "/" + subnetMask);
            return range.Contains(ipAddress1);
        }
    }
}