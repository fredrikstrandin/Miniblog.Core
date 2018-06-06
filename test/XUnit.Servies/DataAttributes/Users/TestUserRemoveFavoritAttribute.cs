using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace XUnit.Test.DataAttributes
{
    public class TestUserRemoveFavoritAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "ThomasLarsson@dayrep.com",
                "58cc5ea4ff482532383b948e"
            };
        }
    }
}


