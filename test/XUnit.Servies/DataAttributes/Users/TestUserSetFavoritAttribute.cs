using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace XUnit.Multiblog.DataAttributes
{
    public class TestUserSetFavoritAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "ThomasLarsson@dayrep.com",
                "592dcafafc00b7d25c0c4bc8"
            };
        }
    }
}


