using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes.Users
{
    public class TestUsersEmail : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "mariambergman@rhyta.com"
            };
            yield return new object[]
            {
                "dannysandberg@rhyta.com"
            };
            yield return new object[]
            {
                "jeanetteforsberg@dayrep.com"
             };
        }
    }
}
