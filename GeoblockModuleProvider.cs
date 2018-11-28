/* GeoblockModuleProvider.cs
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
using Microsoft.Web.Management.Server;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Provides the module to IIS
    /// </summary>
    class GeoblockModuleProvider : ConfigurationModuleProvider
    {
        public override Type ServiceType
        {
            get { return typeof(IISGeoIP2blockModule.GeoblockModuleService); }
        }

        public override ModuleDefinition GetModuleDefinition(IManagementContext context)
        {
            return new ModuleDefinition(Name, typeof(GeoblockModule).AssemblyQualifiedName);
        }

        public override bool SupportsScope(ManagementScope scope)
        {
            return true;
        }

        protected override string ConfigurationSectionName
        {
            get { return GeoblockConfigurationSection.SectionName; }
        }
    }
}
