using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Core.Repository.MongoDB
{
    public class MongoDbDatabaseSetting
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
