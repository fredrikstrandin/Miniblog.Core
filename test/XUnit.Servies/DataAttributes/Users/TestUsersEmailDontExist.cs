using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes.Users
{
    public class TestUsersEmailDontExist : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "DontExist@spam.com"
            };
            yield return new object[]
            {
                "noreplay@spam.com"
            };
        }
    }
}
