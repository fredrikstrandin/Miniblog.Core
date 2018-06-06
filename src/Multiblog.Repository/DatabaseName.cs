using System;
using System.Collections.Generic;
using System.Text;

namespace Multiblog.Repository
{
    internal static class DatabaseName
    {
        public static string UserEntity { get { return "UserEntity"; } }

        public static string RequestChangeEmailEntity { get { return "RequestChangeEmailEntity"; } }
        public static string VerifyEntity { get { return "VerifyEntity"; } }
        public static string PostEntity { get { return "PostEntity"; } }
        public static string CategoryEntity { get { return "CategoryEntity"; } }
        public static string BlogEntity { get { return "BlogEntity"; } }
    }
}
