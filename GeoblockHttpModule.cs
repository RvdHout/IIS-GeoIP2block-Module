/* GeoblockHttpModule.cs
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
using System.Web;
using Microsoft.Web.Administration;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// This is the module that IIS calls to check whether the ip of the request is allowed or denied access
    /// </summary>
    public class GeoblockHttpModule : IHttpModule
    {
        /// <summary>
        /// Attaches itself to the BeginRequest event of the IIS flow
        /// </summary>
        /// <param name="context">The IIS context to attach to</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        /// <summary>
        /// Handles the begin request event. this is the place where the actual check is performed
        /// </summary>
        /// <param name="sender">The HttpApplication to extract the request from</param>
        /// <param name="e">Not used</param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            //Get module config
            GeoblockConfigurationSection moduleConfiguration = (GeoblockConfigurationSection)WebConfigurationManager.GetSection(context, GeoblockConfigurationSection.SectionName, typeof(GeoblockConfigurationSection));
            
            if (!moduleConfiguration.Enabled)
                return;

            //Get the ip's of the request. All the IP's must be checked
            List<System.Net.IPAddress> ipAddressesToCheck = new List<System.Net.IPAddress>();
            string ipNotificationString = string.Empty;
            try 
            {
                string ip = context.Request.UserHostAddress;
                ipNotificationString += "Request IP: [" + ip + "]";
                System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(ip.Trim());
                ipAddressesToCheck.Add(ipAddress);
            }
            catch { }

            //Could be behind proxy, so check forwarded IP's
            string forwardedIps = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!String.IsNullOrEmpty(forwardedIps))
            {
                ipNotificationString += " Forwarded IP's: [" + forwardedIps + "]";
                //The HTTP_X_FORWARDED_FOR value can contain more then one entry (, seperated)
                string[] ips = forwardedIps.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach(string ip in ips){
                    try 
                    {
                        System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(ip.Trim());
                        ipAddressesToCheck.Add(ipAddress);
                    }
                    catch { }
                }
            }
            
            string[] selectedCountryCodes = new string[moduleConfiguration.SelectedCountryCodes.Count];
            int i = 0;
            foreach (CountryConfigurationElement country in moduleConfiguration.SelectedCountryCodes)
            {
                selectedCountryCodes[i] = country.Code;
                i++;
            }
            ExceptionRule[] exceptionRules = new ExceptionRule[moduleConfiguration.ExceptionRules.Count];
            i = 0;
            foreach (ExceptionRuleConfigurationElement exceptionRule in moduleConfiguration.ExceptionRules)
            {
                exceptionRules[i] = new ExceptionRule(exceptionRule.AllowedMode, exceptionRule.IpAddress, exceptionRule.Mask);
                i++;
            }

            //Perform the check
            string resultMessage;
            Geoblocker geoBlocker = new Geoblocker(moduleConfiguration.GeoIpFilepath, selectedCountryCodes, moduleConfiguration.AllowedMode, exceptionRules);
            if (!geoBlocker.Allowed(ipAddressesToCheck, out resultMessage))
            {
                switch (moduleConfiguration.DenyAction)
                {
                    case "Unauthorized":
                        context.Response.StatusCode = 401;
                        context.Response.SubStatusCode = 503;
                        context.Response.StatusDescription = "IP is blocked by GeoIP2block Module. " + ipNotificationString + ". " + resultMessage;
                        context.Response.SuppressContent = true;
                        context.Response.End();
                        break;
                    case "Forbidden":
                        context.Response.StatusCode = 403;
                        context.Response.SubStatusCode = 503;
                        context.Response.StatusDescription = "IP is blocked by GeoIP2block Module. " + ipNotificationString + ". " + resultMessage;
                        context.Response.SuppressContent = true;
                        context.Response.End();
                        break;
                    case "NotFound":
                        context.Response.StatusCode = 404;
                        context.Response.SubStatusCode = 503;
                        context.Response.StatusDescription = "IP is blocked by GeoIP2block Module. " + ipNotificationString + ". " + resultMessage;
                        context.Response.SuppressContent = true;
                        context.Response.End();
                        break;
                    case "Abort":
                        context.Request.Abort();
                        break;
                }
            }
        }
        
        /// <summary>
        /// Needed for IHttpmodule interface
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}
