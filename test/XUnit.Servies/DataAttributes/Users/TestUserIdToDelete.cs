using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Multiblog.Model.User;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes.Users
{
    public class TestUserIdToDeleteAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "5a465bc246063a4faca14008"
            };
            yield return new object[]
            {
                "5a465bc246063a4faca14007"
            };
        }
    }
}
