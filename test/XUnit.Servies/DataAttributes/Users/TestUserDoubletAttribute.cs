using Multiblog.Core.Model.User;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace XUnit.Multiblog.DataAttributes
{
    public class TestUserDoubletAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return new object[] { new UserItem() { Email = "DoubletCheckNo1@eamil.com" } };
            yield return new object[] { new UserItem() { Email = "DoubletCheckNo2@eamil.com" } };
        }
    }
}
