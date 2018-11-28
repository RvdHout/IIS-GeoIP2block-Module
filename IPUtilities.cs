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

using System;
using System.Net;

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
            //127.0.0.1 is also a candidate
            if (ipAddress.Equals(IPAddress.Parse("127.0.0.1")))
                return true;

            //24-bit block	10.0.0.0 – 10.255.255.255	16,777,216	single class A	10.0.0.0/8 (255.0.0.0)	24 bits
            //20-bit block	172.16.0.0 – 172.31.255.255	1,048,576	16 contiguous class Bs	172.16.0.0/12 (255.240.0.0)	20 bits
            //16-bit block	192.168.0.0 – 192.168.255.255	65,536	256 contiguous class Cs	192.168.0.0/16 (255.255.0.0)	16 bits

            //private ranges
            IPAddress private24Subnet = IPAddress.Parse("10.0.0.1");
            IPAddress private24SubnetMask = IPAddress.Parse("255.0.0.0");

            IPAddress private20Subnet = IPAddress.Parse("172.16.0.0");
            IPAddress private20SubnetMask = IPAddress.Parse("255.240.0.0");

            IPAddress private16Subnet = IPAddress.Parse("192.168.0.0");
            IPAddress private16SubnetMask = IPAddress.Parse("255.255.0.0");

            if (IsInSameSubnet(ipAddress, private16Subnet, private16SubnetMask))
                return true;
            if (IsInSameSubnet(ipAddress, private20Subnet, private20SubnetMask))
                return true;
            if (IsInSameSubnet(ipAddress, private24Subnet, private24SubnetMask))
                return true;
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
            return IsInSameSubnet(IPAddress.Parse(ipAddress1), IPAddress.Parse(ipAddress2), IPAddress.Parse(subnetMask));
        }

        /// <summary>
        /// Determines whether two IP addresses are in the same subnet
        /// </summary>
        /// <param name="ipAddress1">IP address 1</param>
        /// <param name="ipAddress2">IP address 2</param>
        /// <param name="subnetMask">The subnet</param>
        /// <returns>True if the IP addresses are within the same subnet. False otherwise</returns>
        public static bool IsInSameSubnet(IPAddress ipAddress1, IPAddress ipAddress2, IPAddress subnetMask)
        {
            IPAddress network1 = GetNetworkAddress(ipAddress1, subnetMask);
            IPAddress network2 = GetNetworkAddress(ipAddress2, subnetMask);

            return network1.Equals(network2);
        }

        /// <summary>
        /// Gets the network address based on an address and the subnetmask
        /// </summary>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="subnetMask">The subnet mask</param>
        /// <returns>The (broadcast) network address</returns>
        public static IPAddress GetNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = ipAddress.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet must be equal.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }
    }
}
