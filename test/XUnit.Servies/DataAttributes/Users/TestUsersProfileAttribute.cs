using Multiblog.Core.Model.User;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes
{
    public class TestUsersProfileAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "5a465bc146063a4faca13ffe",
                new UserProfileItem()
                {
                    FirstName = "Muhammed",
                    LastName = "Holmgren",
                    City = "Tandsbyn",
                    DateOfBirth = new DateTime(1968, 10, 25)
                }
            };
            yield return new object[]
            {
                "5a465bc146063a4faca13fff",
                new UserProfileItem()
                {
                    FirstName = "Lova",
                    LastName = "Jönsson",
                    City = "Undrom",
                    DateOfBirth = new DateTime(1987, 11, 16)
                },
            };
            yield return new object[]
            {
                "5a465bc146063a4faca14000",
                new UserProfileItem()
                {
                    FirstName = "Jill",
                    LastName = "Eklund",
                    City = "Söderåkra",
                    DateOfBirth = new DateTime(1969, 1, 30)
                }
            };
        }
    }
}


