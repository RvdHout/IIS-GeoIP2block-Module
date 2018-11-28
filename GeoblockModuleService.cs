/* GeoblockModuleService.cs
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
using Microsoft.Web.Management.Server;
using System.Collections;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Module service to enable the client to get the settings
    /// </summary>
    public class GeoblockModuleService : ModuleService
    {
        /// <summary>
        /// Gets the configuration of the geoblock module
        /// </summary>
        /// <returns>The geoblock configuration in a propertybag</returns>
        [ModuleServiceMethod(PassThrough = true)]
        public PropertyBag GetGeoblockConfiguration()
        {
            GeoblockConfigurationSection config = (GeoblockConfigurationSection)ManagementUnit.Configuration.GetSection(GeoblockConfigurationSection.SectionName, typeof(GeoblockConfigurationSection));

            PropertyBag result = new PropertyBag();
            result.Add(0, config.Enabled);
            result.Add(1, config.DenyAction);
            result.Add(2, config.GeoIpFilepath);
            result.Add(3, config.AllowedMode);

            ArrayList countries = new ArrayList();
            foreach (CountryConfigurationElement country in config.SelectedCountryCodes)
            {
                PropertyBag item = new PropertyBag();
                item.Add(0, country.Code);
                countries.Add(item);
            }
            result.Add(4, countries);

            ArrayList exceptionRules = new ArrayList();
            foreach (ExceptionRuleConfigurationElement exceptionRule in config.ExceptionRules)
            {
                PropertyBag item = new PropertyBag();
                item.Add(0, exceptionRule.AllowedMode);
                item.Add(1, exceptionRule.IpAddress);
                item.Add(2, exceptionRule.Mask);
                exceptionRules.Add(item);
            }
            result.Add(5, exceptionRules);

            return result;
        }

        /// <summary>
        /// Writes the configuration of the geoblock module
        /// </summary>
        /// <param name="updatedGeoblockConfiguration">The new geoblock configuration in a propertybag</param>
        [ModuleServiceMethod(PassThrough = true)]
        public void UpdateGeoblockConfiguration(PropertyBag updatedGeoblockConfiguration)
        {
            if (updatedGeoblockConfiguration == null)
            {
                throw new ArgumentNullException("updatedGeoblockConfiguration");
            }

            GeoblockConfigurationSection config = (GeoblockConfigurationSection)ManagementUnit.Configuration.GetSection(GeoblockConfigurationSection.SectionName, typeof(GeoblockConfigurationSection));

            config.Enabled = (bool)updatedGeoblockConfiguration[0];
            config.DenyAction = (string)updatedGeoblockConfiguration[1];
            config.GeoIpFilepath = (string)updatedGeoblockConfiguration[2];
            config.AllowedMode = (bool)updatedGeoblockConfiguration[3];
            config.SelectedCountryCodes.Clear();
            ArrayList countries = (ArrayList)updatedGeoblockConfiguration[4];
            foreach (PropertyBag item in countries)
            {
                config.SelectedCountryCodes.Add((string)item[0]);
            }
            config.ExceptionRules.Clear();
            ArrayList exceptionRules = (ArrayList)updatedGeoblockConfiguration[5];
            foreach (PropertyBag item in exceptionRules)
            {
                config.ExceptionRules.Add((bool)item[0], (string)item[1], (string)item[2]);
            }
            
            ManagementUnit.Update();
        }
    }
}
