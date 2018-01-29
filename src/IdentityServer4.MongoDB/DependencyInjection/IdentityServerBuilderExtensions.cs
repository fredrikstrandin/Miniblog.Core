// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


//using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.MongoDB.Model.Setting;
using IdentityServer4.MongoDB.MonogDBContext;
using IdentityServer4.MongoDB.Repository;
using IdentityServer4.MongoDB.Serializer;
using IdentityServer4.MongoDB.Service;
using IdentityServer4.MongoDB.Store;
using IdentityServer4.MongoDB.Validator;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace IdentityServer4.MongoDB.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoDBUsers(this IIdentityServerBuilder builder, IConfigurationRoot configuration)
        {
            builder.Services.Configure<PasswordSetting>(configuration.GetSection("PasswordSetting"));
            builder.Services.Configure<MongoDatabaseSetting>(configuration.GetSection("MongoDBDatabaseSetting"));

            builder.Services.AddScoped<IUserMongoDBService, UserMongoDBService>();
            builder.Services.AddScoped<IUserMongoDBRepository, UserMongoDBRepository>();
            
            builder.Services.AddScoped<IPersistedGrantStore, MongoDBPersistedGrantStore>();
            builder.Services.AddScoped<IMongoIdentityContext, MongoIdentityContext>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();

            BsonSerializer.RegisterSerializationProvider(new ClaimProvider());
            
            builder.Services.AddSingleton<UserMongoDBService>();
            builder.AddProfileService<MongoDBUserProfileService>();
            builder.AddResourceOwnerValidator<MongoDBUserResourceOwnerPasswordValidator>();

            return builder;
        }
    }
}
