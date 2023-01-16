Local installation example:

    IISGeoBlockInstall.ps1 "2.4.1.0" "C:\Program Files\IIS"
    
Remote installation example:

    Invoke-Command -ComputerName ru2-v001-web01 {
        Invoke-WebRequest https://github.com/6eh01der/IIS-GeoIP2block-Module/raw/master/InstallScripts/IISGeoBlockInstall.ps1 -OutFile IISGeoBlockInstall.ps1
        .\IISGeoBlockInstall.ps1 "2.4.1.0" "C:\Program Files\IIS"
        Remove-Item IISGeoBlockInstall.ps1 -Force
    }

Example of module configuration for every site on server via powershell:

    $Sites = (Get-IISite).Name
    Foreach ($site in $sites) {
    Set-WebConfigurationProperty //geoblockModule -name geoIpFilepath -value C:\MaxMind\GeoIP2Lite\GeoLite2-Country.mmdb -PSPath "iis:sites/$site"
    Set-WebConfigurationProperty //geoblockModule -name DenyAction -value Abort -PSPath "iis:sites/$site"
    Add-WebConfigurationProperty //geoblockModule/selectedCountryCodes -name collection -value @{code='CountryCode'} -PSPath "iis:sites/$site"
    Set-WebConfigurationProperty //geoblockModule -name allowedMode -value False -PSPath "iis:sites/$site"
    Set-WebConfigurationProperty //geoblockModule -name enabled -value True -PSPath "iis:sites/$site"
    }
