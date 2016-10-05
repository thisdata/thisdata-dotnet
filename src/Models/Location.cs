using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class Location
    {
        [DataMember(Name = "ip")]
        public string IPAddress { get; private set; }

        [DataMember(Name = "coordinates")]
        public Coordinates Coordinates { get; private set; }

        [DataMember(Name = "address")]
        public Address Address { get; private set; }
    }

    [DataContract]
    public class Coordinates
    {
        [DataMember(Name = "longitude")]
        public double Longitude { get; private set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; private set; }
    }

    [DataContract]
    public class Address
    {
        [DataMember(Name = "country_name")]
        public string CountryName { get; private set; }

        [DataMember(Name = "country_iso_code")]
        public string CountryISOCode { get; private set; }

        [DataMember(Name = "region_name")]
        public string RegionName { get; private set; }

        [DataMember(Name = "city_name")]
        public string CityName { get; private set; }

        [DataMember(Name = "postal_code")]
        public string PostalCode { get; private set; }

        [DataMember(Name = "timezone")]
        public string TimeZone { get; private set; }
    }
}