using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace XUnit.Multiblog.DataAttributes.Users
{
    public class TestUsersEmailNotVerify : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[]
            {
                "anthonsandstrom@cuvox.de"
            };
            yield return new object[]
            {
                "sannefredriksson@armyspy.com"
            };
            yield return new object[]
            {
                "petrasvensson@jourrapide.com"
             };
        }
    }
}
