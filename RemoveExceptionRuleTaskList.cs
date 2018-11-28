/* RemoveExceptionRuleTaskList.cs
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

using System.Collections;
using Microsoft.Web.Management.Client;
using Microsoft.Web.Management.Client.Win32;

namespace IISGeoIP2blockModule
{
    /// <summary>
    /// Provides a task list for the geoblock module page
    /// </summary>
    class RemoveExceptionRuleTaskList : TaskList
    {
        private GeoblockModuleDialogPage owner = null;

        private MethodTaskItem removeExceptionRuleTaskItem = null;

        /// <summary>
        /// Creates a new tasklist instance
        /// </summary>
        /// <param name="owner">The page for which the tasklist is created</param>
        public RemoveExceptionRuleTaskList(GeoblockModuleDialogPage owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Returns a collection of task items to show next to the module page
        /// </summary>
        /// <returns>A collection of task items</returns>
        public override System.Collections.ICollection GetTaskItems()
        {
            ArrayList items = new ArrayList();

            System.IO.Stream icoStream = this.GetType().Assembly.GetManifestResourceStream("IISGeoIP2blockModule.resources.remove.png");
            System.Drawing.Bitmap ico = new System.Drawing.Bitmap(icoStream);
            icoStream.Close();

            removeExceptionRuleTaskItem = new MethodTaskItem("RemoveExceptionRule", "Remove Exception Rule", "Actions", "Remove selected exception rule", ico);
            items.Add(removeExceptionRuleTaskItem);
            
            return items;
        }

        /// <summary>
        /// Fires when the delete exception rule task is selected
        /// </summary>
        public void RemoveExceptionRule()
        {
            owner.RemoveExceptionRule();
        }
    }
}
