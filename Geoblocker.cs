/* Geoblocker.cs
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
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Checks whether an ip address has access to the IIS application
    /// </summary>
    public class Geoblocker
    {
        //Note: Each request to the server is an entirely new instance of the module
        //      Keeping the GeoIP.dat file in a static memory stream is of no use.

        /// <summary>
        /// The path that points to the geo ip data file
        /// </summary>
        private string geoIpFilepath;

        /// <summary>
        /// The countrycodes to allow or deny access
        /// </summary>
        private string[] selectedCountryCodes;

        /// <summary>
        /// Indicates whether the selected countrycodes are allowed or denied access
        /// </summary>
        private bool allowedMode;

        /// <summary>
        /// The exception rules to check first
        /// </summary>
        private ExceptionRule[] exceptionRules;

        /// <summary>
        /// Indicates whether or not if any proxy in HTTP_X_FORWARDED_FOR should be ignored if previous checked ip matches
        /// </summary>
        private bool verifyAll;

        /// <summary>
        /// Creates a new Geoblocker instance
        /// </summary>
        /// <param name="geoIpFilepath">The path to the geo ip data file</param>
        /// <param name="selectedCountryCodes">The countrycodes to look for</param>
        /// <param name="allowedMode">Whether the selected country codes are allowed or denied access</param>
        /// <param name="verifyAll">Indicates whether or not if any proxy in HTTP_X_FORWARDED_FOR should be ignored if previous checked ip matches</param>
        public Geoblocker(string geoIpFilepath, string[] selectedCountryCodes, bool allowedMode, ExceptionRule[] exceptionRules, bool verifyAll)
        {
            this.geoIpFilepath = geoIpFilepath;
            this.selectedCountryCodes = selectedCountryCodes;
            this.allowedMode = allowedMode;
            this.exceptionRules = exceptionRules;
            this.verifyAll = verifyAll;
        }

        /// <summary>
        /// Checks the country to which the ip's belong to and determines whether or not access is denied or allowed
        /// </summary>
        /// <param name="ipAddressesToCheck">The ip addresses to check</param>
        /// <param name="resultMessage">An explanation of the result</param>
        /// <returns>True if access is allowed. False otherwise</returns>
        /// <remarks>All IP addresses must be allowed for the request to be allowed</remarks>
        public bool Allowed(List<System.Net.IPAddress> ipAddressesToCheck, out string resultMessage)
        {
            if (ipAddressesToCheck == null || ipAddressesToCheck.Count == 0)
            {
                resultMessage = "No valid IP found in request";
                return false;
            }

            using (var reader = new MaxMind.GeoIP2.DatabaseReader(geoIpFilepath, MaxMind.Db.FileAccessMode.MemoryMapped))
            {
                //sanity check
                if (reader == null)
                {
                    resultMessage = "IP check failed";
                    return true;
                }

                foreach (System.Net.IPAddress ipAddress in ipAddressesToCheck)
                {
                    //first check if the IP is a private ip. It turns out that proxy servers put private ip addresses in the X_FORWARDED_FOR header.
                    //we won't base our geoblocking on private ip addresses
                    if (IPUtilities.IsPrivateIpAddress(ipAddress))
                        continue;

                    //next check the exception rules
                    bool matchedOnExceptionRule = false;
                    bool allowedByExceptionRule = false;
                    foreach (ExceptionRule exceptionRule in exceptionRules)
                    {
                        if (IpAddressMatchesExceptionRule(ipAddress, exceptionRule))
                        {
                            matchedOnExceptionRule = true;
                            if (exceptionRule.AllowedMode)
                                allowedByExceptionRule = true;
                            else
                            {
                                allowedByExceptionRule = false;
                                //one IP denied = deny access alltogether
                                break;
                            }
                        }
                    }
                    if (matchedOnExceptionRule)
                    {
                        if (allowedByExceptionRule)
                        {
                            //IP found that matches an allow exception rule, don't check the country
                            //We continue if verifyAll is specified, because another IP could be denied
                            if (verifyAll)
                                continue;
                            else
                                break;
                        }
                        else
                        {
                            //IP found that matches a deny exception rule, deny access immediately
                            resultMessage = "Blocked IP: [" + ipAddress.ToString() + "]";
                            return false;
                        }
                    }

                    if (reader.Metadata.DatabaseType.ToLower().IndexOf("country") == -1)
                        throw new System.InvalidOperationException("This is not a GeoLite2-Country or GeoIP2-Country database");

                    //not found in exception rule, so base access rights on the country
                    MaxMind.GeoIP2.Responses.CountryResponse countryResponse = reader.Country(ipAddress);
                    string countryCode = !string.IsNullOrEmpty(countryResponse.Country.IsoCode) ? countryResponse.Country.IsoCode : !string.IsNullOrEmpty(countryResponse.RegisteredCountry.IsoCode) ? countryResponse.RegisteredCountry.IsoCode : string.Empty;

                    if (string.IsNullOrEmpty(countryCode))
                        countryCode = "--";

                    bool selected = CountryCodeSelected(countryCode);
                    bool allowed = (selected == allowedMode);

                    /*
                     * allowedmode     selected     allowed
                     *  1               1            1
                     *  1               0            0
                     *  0               1            0
                     *  0               0            1
                    */

                    if (!allowed)
                    {
                        resultMessage = "Blocked IP: [" + ipAddress.ToString() + "] from [" + countryCode + "]";
                        return false;
                    }
                    else
                    {
                        // If a proxy in HTTP_X_FORWARDED_FOR should be ignored if previous checked ip matches previous found country or exceptionRule                      
                        if (!verifyAll)
                            break;
                    }
                }
            }
            resultMessage = "None";
            return true;
        }

        /// <summary>
        /// Checks whether the country code is present in the selected country code array
        /// </summary>
        /// <param name="countryCode">The country code to look up</param>
        /// <returns>True if the country code is present. False otherwise</returns>
        private bool CountryCodeSelected(string countryCode)
        {
            foreach (string selectedCountryCode in selectedCountryCodes)
                if (countryCode == selectedCountryCode)
                    return true;
            return false;
        }

        /// <summary>
        /// Checks whether an IP address matches to an exception rule
        /// </summary>
        /// <param name="ipAddress">The IP address to check</param>
        /// <param name="exceptionRule">The exception rule to match it against</param>
        /// <returns>True if the IP address matches to the exception rule. False otherwise</returns>
        private bool IpAddressMatchesExceptionRule(IPAddress ipAddress, ExceptionRule exceptionRule)
        {
            if (String.IsNullOrEmpty(exceptionRule.Mask) && IPAddress.Parse(exceptionRule.IpAddress).Equals(ipAddress))
                return true;
            if (!String.IsNullOrEmpty(exceptionRule.Mask) && IPUtilities.IsInSameSubnet(ipAddress, exceptionRule.IpAddress, exceptionRule.Mask))
                return true;
            return false;
        }
    }
}
