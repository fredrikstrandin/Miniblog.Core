using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.MongoDB.Model.OAuthClient
{
    public class LinkedInPersonData
    {
        public ApiStandardProfileRequest apiStandardProfileRequest { get; set; }
        public CurrentShare currentShare { get; set; }
        public string firstName { get; set; }
        public string formattedName { get; set; }
        public string headline { get; set; }
        public string id { get; set; }
        public string industry { get; set; }
        public string lastName { get; set; }
        public Location location { get; set; }
        public int numConnections { get; set; }
        public bool numConnectionsCapped { get; set; }
        public string pictureUrl { get; set; }
        public PictureUrls pictureUrls { get; set; }
        public Positions positions { get; set; }
        public string publicProfileUrl { get; set; }
        public SiteStandardProfileRequest siteStandardProfileRequest { get; set; }
        public string summary { get; set; }
    }

    public class Value
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Headers
    {
        public int _total { get; set; }
        public List<Value> values { get; set; }
    }

    public class ApiStandardProfileRequest
    {
        public Headers headers { get; set; }
        public string url { get; set; }
    }

    public class Author
    {
        public string firstName { get; set; }
        public string id { get; set; }
        public string lastName { get; set; }
    }

    public class Content
    {
        public string eyebrowUrl { get; set; }
        public string resolvedUrl { get; set; }
        public string shortenedUrl { get; set; }
        public string submittedImageUrl { get; set; }
        public string submittedUrl { get; set; }
        public string thumbnailUrl { get; set; }
    }

    public class ServiceProvider
    {
        public string name { get; set; }
    }

    public class Source
    {
        public ServiceProvider serviceProvider { get; set; }
    }

    public class Visibility
    {
        public string code { get; set; }
    }

    public class CurrentShare
    {
        public Author author { get; set; }
        public string comment { get; set; }
        public Content content { get; set; }
        public string id { get; set; }
        public Source source { get; set; }
        public long timestamp { get; set; }
        public Visibility visibility { get; set; }
    }

    public class Country
    {
        public string code { get; set; }
    }

    public class Location
    {
        public Country country { get; set; }
        public string name { get; set; }
    }

    public class PictureUrls
    {
        public int _total { get; set; }
        public List<string> values { get; set; }
    }

    public class Company
    {
        public int id { get; set; }
        public string industry { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string type { get; set; }
    }

    public class Country2
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Location2
    {
        public Country2 country { get; set; }
        public string name { get; set; }
    }

    public class StartDate
    {
        public int month { get; set; }
        public int year { get; set; }
    }

    public class Value2
    {
        public Company company { get; set; }
        public int id { get; set; }
        public bool isCurrent { get; set; }
        public Location2 location { get; set; }
        public StartDate startDate { get; set; }
        public string title { get; set; }
    }

    public class Positions
    {
        public int _total { get; set; }
        public List<Value2> values { get; set; }
    }

    public class SiteStandardProfileRequest
    {
        public string url { get; set; }
    }
}
