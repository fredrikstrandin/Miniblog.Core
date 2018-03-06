using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace XUnit.Multiblog.DataAttributes
{
    public class TestUserFavoritWihtWrongFavorite : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "592dd88bcdb1bbd35cc592f5",
                "WrongObjectId"

            };
        }
    }
}


