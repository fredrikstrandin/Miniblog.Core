using Multiblog.Core.Model.User;
using Multiblog.Core.Model.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes
{
    public class TestUsersAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                new UserItem()
                {
                    Id = "5a398c373f7ce3384c6c4edf",
                    FirstName = "Viktor",
                    LastName = "Andersson",
                    Email = "ViktorAndersson@jourrapide.com",
                    EmailVerified = true,
                    City = "Kalvträsk",
                    DateOfBirth = new DateTime(1976, 7, 6),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GPSPosition() { Longitude = 65.441274, Latitude = 18.910866 }
                }
            };
            yield return new object[]
            {
                new UserItem()
                {
                    Id = "5a398c363f7ce3384c6c4edd",
                    FirstName = "Kristina",
                    LastName = "Holmberg",
                    Email = "KristinaHolmberg@gustr.com",
                    EmailVerified = true,
                    City = "Nyhammar",
                    DateOfBirth = new DateTime(1974, 12, 22),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GPSPosition() { Longitude = 60.352036, Latitude = 14.998837 }
                }
            };
            yield return new object[]
            {

                new UserItem()
                {
                    Id = "5a398c373f7ce3384c6c4ede",
                    FirstName = "Holger",
                    LastName = "Henriksson",
                    Email = "HolgerHenriksson@dayrep.com",
                    EmailVerified = true,
                    City = "Östhammar",
                    DateOfBirth = new DateTime(1972, 11, 4),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GPSPosition() { Longitude = 62.683247, Latitude = 17.856905 }
                }
            };
        }
    }
}


