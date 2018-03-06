using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Multiblog.Core.Model.User;
using Multiblog.Model.User;
using Xunit.Sdk;

namespace XUnit.Multiblog.DataAttributes.Users
{
    public class TestUserProfileAndIdAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {

                "5a465bc046063a4faca13fdd",
                new UserProfileItem()
                {
                    FirstName = "Lisa",
                    LastName = "Ivarsson",
                    DateOfBirth = new DateTime(1973, 10, 11),
                    City = "Karlskrona",
                    ProfileImageUrl = "http://picture.fake"
                }
            };
            yield return new object[]
            {
                "5a465bc046063a4faca13fde",
                new UserProfileItem()
            };
        }
    }
}
