using System;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Minibank.Web.HostedServices;
using Microsoft.OpenApi.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Minibank.Web.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHostedService<MigrationHostedService>();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        ClientCredentials = new OpenApiOAuthFlow()
                        {
                            TokenUrl = new Uri(configuration["OpenApiOAuthFlow:TokenUrl"]),
                            Scopes = new Dictionary<string, string>()
                        }
                    }
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = SecuritySchemeType.OAuth2.GetDisplayName()
                            }

                        },
                        new List<string>()
                    }
                });
            });
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = configuration["JwtBearer:Audience"];
                    options.Authority = configuration["JwtBearer:Authority"];
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateLifetime = false,
                        ValidateAudience = false
                    };
                });
            return services;
        }
    }
}