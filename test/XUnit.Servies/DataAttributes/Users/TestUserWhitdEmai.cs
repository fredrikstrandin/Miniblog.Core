using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes.Users
{
    public class TestUserWhitdEmail : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "5a465bc046063a4faca13fd8",
                "dannysandberg@rhyta.com"
            };
        }
    }
}
