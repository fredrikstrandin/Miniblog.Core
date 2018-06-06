using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Multiblog.Model.User;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes
{
    public class TestUserIdThatDontExistAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "592dd88bcdb1bbd35cc592f5"
            };

            yield return new object[]
            {
                "592dd8e28fd6edde64ff6481"
            };

            yield return new object[]
            {
                "592dd93e26d826dc10e140ab"
            };
        }
    }
}


