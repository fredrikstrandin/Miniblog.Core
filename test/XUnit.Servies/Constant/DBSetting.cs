using Microsoft.Extensions.Options;
using Multiblog.Core.Model.Setting;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnit.Multiblog.Constant
{
    public static class DBSetting
    {
        public static readonly IOptions<MongoDbDatabaseSetting> Value = Options.Create(new MongoDbDatabaseSetting()
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "Multiblog_Test"
        });
    }
}
