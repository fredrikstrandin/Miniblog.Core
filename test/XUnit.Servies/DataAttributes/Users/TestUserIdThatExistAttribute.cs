using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace XUnit.Multiblog.DataAttributes
{
    public class TestUserIdThatExistAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "5a465bc146063a4faca14004"
            };

            yield return new object[]
            {
                "5a465bc146063a4faca14003"
            };

            yield return new object[]
            {
                "5a465bc146063a4faca14002"
            };
        }
    }
}


