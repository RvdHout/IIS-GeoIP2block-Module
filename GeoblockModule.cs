#nullable disable
/* GeoblockModule.cs
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
using Microsoft.Web.Management.Client;
using Microsoft.Web.Management.Server;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Represents the GeoblockModule in IIS
    /// </summary>
    internal class GeoblockModule : Module
    {
        /// <summary>
        /// Called by IIS to register the modulepage
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="moduleInfo">The module info</param>
        protected override void Initialize(IServiceProvider serviceProvider, ModuleInfo moduleInfo)
        {
            base.Initialize(serviceProvider, moduleInfo);

            //load the icon
            System.IO.Stream icoStream = this.GetType().Assembly.GetManifestResourceStream("IISGeoIP2blockModule.resources.geoblock.png");
            System.Drawing.Bitmap ico = new System.Drawing.Bitmap(icoStream);
            icoStream.Close();

            string description = $"[{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}] Blocks access by country referenced by client IP address.";
            IControlPanel controlPanel = (IControlPanel)GetService(typeof(IControlPanel));
            ModulePageInfo modulePageInfo = new ModulePageInfo(this, typeof(GeoblockModuleDialogPage), "Geoblock Module", "Blocks access by country referenced by client IP address.", ico, ico, description);

            controlPanel.RegisterPage(ControlPanelCategoryInfo.Security, modulePageInfo);
        }

        /// <summary>
        /// Whether or not the modulepage is enabled in the iis 7 management console
        /// </summary>
        /// <param name="pageInfo">The module page info</param>
        /// <returns>True if the module page is enabled. False otherwise</returns>
        protected override bool IsPageEnabled(ModulePageInfo pageInfo)
        {
            return base.IsPageEnabled(pageInfo);
        }
    }
}
