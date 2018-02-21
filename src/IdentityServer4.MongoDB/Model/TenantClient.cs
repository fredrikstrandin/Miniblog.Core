using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.MongoDB.Model
{
    public class TenantClient : Client
    {
        public bool Tenant { get; set; }
    }
}
