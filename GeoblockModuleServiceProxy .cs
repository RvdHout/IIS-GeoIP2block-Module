/* GeoblockModuleServiceProxy.cs
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

using Microsoft.Web.Management.Client;
using Microsoft.Web.Management.Server;
using System.Collections;
using System.Collections.Generic;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Uses the module service at the server to get and update the geoblock configuration
    /// </summary>
    public class GeoblockModuleServiceProxy : ModuleServiceProxy 
    {
        /// <summary>
        /// Gets the geoblock configuration from the module service
        /// </summary>
        /// <returns>The geoblock configuration</returns>
        public GeoblockConfiguration GetGeoblockConfiguration()
        {
            PropertyBag config = (PropertyBag)Invoke("GetGeoblockConfiguration");

            GeoblockConfiguration result = new GeoblockConfiguration();

            result.Enabled = (bool)config[0];
            result.DenyAction = (string)config[1];
            result.GeoIpFilepath = (string)config[2];
            result.VerifyAll = (bool)config[3];
            result.AllowedMode = (bool)config[4];

            result.SelectedCountryCodes = new List<Country>();
            ArrayList countries = (ArrayList)config[5];
            foreach (PropertyBag item in countries)
            {
                result.SelectedCountryCodes.Add(new Country((string)item[0], null));
            }

            result.ExceptionRules = new List<ExceptionRule>();
            ArrayList exceptionRules = (ArrayList)config[6];
            foreach (PropertyBag item in exceptionRules)
            {
                result.ExceptionRules.Add(new ExceptionRule((bool)item[0], (string)item[1], (string)item[2]));
            }

            return result;
        }

        /// <summary>
        /// Updates the geoblock configuration using the module service
        /// </summary>
        /// <param name="updatedGeoblockConfiguration">The new geoblock configuration</param>
        public void UpdateGeoblockConfiguration(GeoblockConfiguration updatedGeoblockConfiguration)
        {
            PropertyBag config = new PropertyBag();
            config.Add(0, updatedGeoblockConfiguration.Enabled);
            config.Add(1, updatedGeoblockConfiguration.DenyAction);
            config.Add(2, updatedGeoblockConfiguration.GeoIpFilepath);
            config.Add(3, updatedGeoblockConfiguration.VerifyAll);
            config.Add(4, updatedGeoblockConfiguration.AllowedMode);

            ArrayList countries = new ArrayList();
            foreach (Country country in updatedGeoblockConfiguration.SelectedCountryCodes)
            {
                PropertyBag item = new PropertyBag();
                item.Add(0, country.CountryCode);
                countries.Add(item);
            }
            config.Add(5, countries);

            ArrayList exceptionRules = new ArrayList();
            foreach (ExceptionRule exceptionRule in updatedGeoblockConfiguration.ExceptionRules)
            {
                PropertyBag item = new PropertyBag();
                item.Add(0, exceptionRule.AllowedMode);
                item.Add(1, exceptionRule.IpAddress);
                item.Add(2, exceptionRule.Mask);
                exceptionRules.Add(item);
            }
            config.Add(6, exceptionRules);

            Invoke("UpdateGeoblockConfiguration", config);
        }
    }
}
