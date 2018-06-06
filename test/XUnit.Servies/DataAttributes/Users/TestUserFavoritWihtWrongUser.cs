using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes
{
    public class TestUserFavoritWihtWrongUser : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "WrongObjectId",
                "592dd88bcdb1bbd35cc592f5"
            };
        }
    }
}


