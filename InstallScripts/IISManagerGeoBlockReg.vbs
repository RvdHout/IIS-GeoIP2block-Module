Set Args = WScript.Arguments
Version = Args(0)
Set adminManager = WScript.CreateObject("Microsoft.ApplicationHost.WritableAdminManager")
adminManager.CommitPath = "MACHINE/WEBROOT"
adminManager.SetMetadata "pathMapper", "AdministrationConfig"

Set moduleProvidersSection = adminManager.GetAdminSection("moduleProviders", "MACHINE/WEBROOT")
Set moduleProvidersCollection = moduleProvidersSection.Collection
Set addElement = moduleProvidersCollection.CreateNewElement("add")
addElement.Properties.Item("name").Value = "Geoblocker"
addElement.Properties.Item("type").Value = "IISGeoIP2blockModule.GeoblockModuleProvider, IISGeoIP2blockModule, Version=" & Args(0) & ", Culture=neutral, PublicKeyToken=50262f380b75b73d"
moduleProvidersCollection.AddElement(addElement)

Set modulesSection = adminManager.GetAdminSection("modules", "MACHINE/WEBROOT")
Set modulesCollection = modulesSection.Collection
Set addElement = modulesCollection.CreateNewElement("add")
addElement.Properties.Item("name").Value = "Geoblocker"
modulesCollection.AddElement(addElement)

adminManager.CommitChanges()
