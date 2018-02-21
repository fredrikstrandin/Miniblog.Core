// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


//using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Vivus.OAuth.IdentityServer
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddTenant(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<IRedirectUriValidator, TenantRedirectUriValidator>();
            
            return builder;
        }
    }
}
