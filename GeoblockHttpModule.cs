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
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Collections;
using System.Linq;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// This is the module that IIS calls to check whether the ip of the request is allowed or denied access
    /// </summary>
    public class GeoblockHttpModule : IHttpModule
    {
#if DEBUG
        private readonly static string _my_name;

        static GeoblockHttpModule()
        {
            GeoblockHttpModule._my_name = Assembly.GetExecutingAssembly().GetName().Name.ToString();
        }
#endif

        public GeoblockHttpModule()
        {
        }

        /// <summary>
        /// Attaches itself to the BeginRequest event of the IIS flow
        /// </summary>
        /// <param name="context">The IIS context to attach to</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

#if DEBUG
        private void DbgWrite(string format, params object[] args)
        {
            try
            {
                string str = string.Format(format, args);
                Trace.WriteLine(string.Format("[{0}]: {1}", GeoblockHttpModule._my_name, str));
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("DbgWrite::Error: {0}", exception.Message));
            }
        }

        private static string ConvertStringArrayToStringJoin(string[] array)
        {
            return string.Join(",", array);
        }
#endif

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
#if DEBUG
                this.DbgWrite(string.Format("REMOTE_ADDR: {0}", ip));
#endif
                ipNotificationString += "Request IP: [" + ip + "]";
                System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(ip.Trim());
                ipAddressesToCheck.Add(ipAddress);
            }
            catch { }

            //Could be behind proxy, so check forwarded IP's
            string forwardedIps = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
#if DEBUG
            this.DbgWrite(string.Format("HTTP_X_FORWARDED_FOR: {0}", forwardedIps));
#endif
            if (!String.IsNullOrEmpty(forwardedIps))
            {
                ipNotificationString += " Forwarded IP's: [" + forwardedIps + "]";
                //The HTTP_X_FORWARDED_FOR value can contain more then one entry (, seperated)
                string[] ips = forwardedIps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ip in ips)
                {
                    try
                    {
                        System.Net.IPAddress ipAddress;
                        // Forwarded IP's in Application Request Routing might contain port
                        if (ip.Contains(":"))
                            ipAddress = System.Net.IPAddress.Parse(ip.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim());
                        else
                            ipAddress = System.Net.IPAddress.Parse(ip.Trim());
                        ipAddressesToCheck.Add(ipAddress);
                    }
                    catch { }
                }
            }
            // Make new unique list
            List<System.Net.IPAddress> ipAddressesToCheckUnique = ipAddressesToCheck.Distinct().ToList();
#if DEBUG
            ArrayList ipToCheck = new ArrayList();
            foreach (IPAddress ipAddress in ipAddressesToCheckUnique)
            {
                ipToCheck.Add(ipAddress.ToString());
            }
            this.DbgWrite(string.Format("Requestor IP address('s): {0}{1}", GeoblockHttpModule.ConvertStringArrayToStringJoin((string[])ipToCheck.ToArray(typeof(string))), Environment.NewLine));
#endif
            string[] selectedCountryCodes = new string[moduleConfiguration.SelectedCountryCodes.Count];
            int i = 0;
            foreach (CountryConfigurationElement country in moduleConfiguration.SelectedCountryCodes)
            {
                selectedCountryCodes[i] = country.Code;
                i++;
            }
#if DEBUG
            this.DbgWrite(string.Format("CountryCodes: {0} AllowedMode: {1}{2}", GeoblockHttpModule.ConvertStringArrayToStringJoin(selectedCountryCodes), moduleConfiguration.AllowedMode, Environment.NewLine));
#endif
            ExceptionRule[] exceptionRules = new ExceptionRule[moduleConfiguration.ExceptionRules.Count];
            i = 0;
            foreach (ExceptionRuleConfigurationElement exceptionRule in moduleConfiguration.ExceptionRules)
            {
                exceptionRules[i] = new ExceptionRule(exceptionRule.AllowedMode, exceptionRule.IpAddress, exceptionRule.Mask);
#if DEBUG
                if (!string.IsNullOrEmpty(exceptionRule.Mask))
                    this.DbgWrite(string.Format("exceptionRule: AllowedMode: {0}, Requestor: {1} ({2}){3}", exceptionRule.AllowedMode, exceptionRule.IpAddress, exceptionRule.Mask, Environment.NewLine));
                else
                    this.DbgWrite(string.Format("exceptionRule: AllowedMode: {0}, Requestor: {1}{2}", exceptionRule.AllowedMode, exceptionRule.IpAddress, Environment.NewLine));
#endif
                i++;
            }

            //Perform the check
            string resultMessage;
            Geoblocker geoBlocker = new Geoblocker(moduleConfiguration.GeoIpFilepath, selectedCountryCodes, moduleConfiguration.AllowedMode, exceptionRules, moduleConfiguration.VerifyAll);
            if (!geoBlocker.Allowed(ipAddressesToCheckUnique, out resultMessage))
            {
#if DEBUG
                this.DbgWrite(string.Concat("IP is blocked by GeoIP2block Module. ", ipNotificationString, ". ", resultMessage));
                this.DbgWrite(string.Format("DenyAction: {0} ", moduleConfiguration.DenyAction));
#endif
                switch (moduleConfiguration.DenyAction)
                {
                    case "Unauthorized":
                        context.Response.StatusCode = 401;
                        context.Response.SubStatusCode = 503;
                        context.Response.StatusDescription = string.Concat("IP is blocked by GeoIP2block Module. ", ipNotificationString, ". ", resultMessage);
                        context.Response.SuppressContent = true;
                        context.Response.End();
                        break;
                    case "Forbidden":
                        context.Response.StatusCode = 403;
                        context.Response.SubStatusCode = 503;
                        context.Response.StatusDescription = string.Concat("IP is blocked by GeoIP2block Module. ", ipNotificationString, ". ", resultMessage);
                        context.Response.SuppressContent = true;
                        context.Response.End();
                        break;
                    case "NotFound":
                        context.Response.StatusCode = 404;
                        context.Response.SubStatusCode = 503;
                        context.Response.StatusDescription = string.Concat("IP is blocked by GeoIP2block Module. ", ipNotificationString, ". ", resultMessage);
                        context.Response.SuppressContent = true;
                        context.Response.End();
                        break;
                    case "Abort":
                        context.Request.Abort();
                        break;
                }
            }
#if DEBUG
            else
            this.DbgWrite(string.Format("DenyAction: {0} ", resultMessage));
#endif
        }

        /// <summary>
        /// Needed for IHttpmodule interface
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}
