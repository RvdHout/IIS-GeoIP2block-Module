Installation example:

    IISGeoBlockInstall.ps1 "2.4.1.0" "C:\Program Files\IIS"

Example of module configuration for every site via powershell:

    $Sites = (Get-IISite).Name
    Foreach ($site in $sites) {
    Set-WebConfigurationProperty //geoblockModule -name geoIpFilepath -value C:\MaxMind\GeoIP2Lite\GeoLite2-Country.mmdb -PSPath "iis:sites/$site"
    Set-WebConfigurationProperty //geoblockModule -name DenyAction -value Abort -PSPath "iis:sites/$site"
    Add-WebConfigurationProperty //geoblockModule/selectedCountryCodes -name collection -value @{code='CountryCode'} -PSPath "iis:sites/$site"
    Set-WebConfigurationProperty //geoblockModule -name allowedMode -value False -PSPath "iis:sites/$site"
    Set-WebConfigurationProperty //geoblockModule -name enabled -value True -PSPath "iis:sites/$site"
    }
