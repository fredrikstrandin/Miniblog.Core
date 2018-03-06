using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Multiblog.Model.User;
using Xunit.Sdk;

namespace XUnit.Multiblog.DataAttributes
{
    public class TestUserIdThatDontExistAndFavoriteAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "592dd88bcdb1bbd35cc592f5",
                "592dd88bcdb1bbd35cc592f5"
            };

            yield return new object[]
            {
                "592dd8e28fd6edde64ff6481",
                "592dd88bcdb1bbd35cc592f5"
            };

            yield return new object[]
            {
                "592dd93e26d826dc10e140ab",
                "592dd88bcdb1bbd35cc592f5"
            };
        }
    }
}


