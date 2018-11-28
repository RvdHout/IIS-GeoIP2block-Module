# IIS GeoIP2block Module

This is a modified version of the geoblock module originaly created by Triple IT for IIS 7/8.x. It can be added to any application pool using integrated pipeline mode running .Net 4 (now also on IIS 10). It uses the IPv4 address to determine the geographic location of the request by using Maxminds GeoIP2 database and takes action accordingly.

Orginal source:
https://sourceforge.net/projects/iis7geoblockmod/


# New features

- Upgraded to .Net 4.6
- Supports new GeoIP2 database format (free GeoLite2-Country.mmdb or commercial GeoIP2-Country.mmdb) 
- Extra deny actions:
    - Unauthorized
    - Forbidden
    - Not Found
    - Abort
    
# Requirements
- IIS 7 or higher
- Net 4 Application pool running integrated pipeline mode
