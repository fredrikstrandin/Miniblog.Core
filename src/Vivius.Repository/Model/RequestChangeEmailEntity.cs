using System;
using System.Collections.Generic;
using System.Text;

namespace Miniblog.Core.Repository.Model
{
    internal class RequestChangeEmailEntity
    {
        public string Code { get; set; }
        public string NewEmail { get; set; }
        public string OldEmail { get; set; }
    }
}
