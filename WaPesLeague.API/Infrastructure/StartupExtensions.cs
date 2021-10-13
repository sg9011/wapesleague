using Base.Bot.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Buffers;
using System.Linq;
using System.Reflection;
namespace WaPesLeague.API.Infrastructure
{
    public static class StartupExtensions
    {
        public static IMvcBuilder ConfigureControllers(this IServiceCollection services, bool requireAuthenticatedUser = false)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            jsonSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });
            services.AddVersionedApiExplorer(delegate (ApiExplorerOptions options)
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            }).AddApiVersioning(delegate (ApiVersioningOptions o)
            {
                o.ReportApiVersions = true;
            });
            return services.AddControllers(delegate (MvcOptions options)
            {
                options.AllowEmptyInputInBodyModelBinding = true;
                //options.OutputFormatters.RemoveType<NewtonsoftJsonOutputFormatter>();
                //options.OutputFormatters.Add(new WrappedJsonOutputFormatter(jsonSettings, ArrayPool<char>.Shared, options));
                //if (requireAuthenticatedUser)
                //{
                //    AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                //    options.Filters.Add(new AuthorizeFilter(policy));
                //}
            });
            //.AddNewtonsoftJson(delegate (MvcNewtonsoftJsonOptions opts)
            //{
            //    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
            //}).ConfigureApplicationPartManager(delegate (ApplicationPartManager x) { });
        }

        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            SwaggerConfig swaggerConfig = configuration.GetSection("Swagger").Get<SwaggerConfig>();
            if (swaggerConfig == null)
            {
                throw new ArgumentException("This application is missing a SwaggerConfiguration (Swagger) in your appsettings. Please add this first.");
            }

            services.AddSwaggerGen(delegate (SwaggerGenOptions options)
            {
                //options.DocumentFilter<LowercaseDocumentFilter>(Array.Empty<object>());
                foreach (ApiVersionDescription apiVersionDescription in services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
                {
                    options.SwaggerDoc(apiVersionDescription.GroupName, CreateInfoForApiVersion(swaggerConfig, apiVersionDescription));
                }

                //options.OperationFilter<OperationTagFilter>(Array.Empty<object>());
                //options.OperationFilter<SwaggerDefaultValues>(Array.Empty<object>());
                //if (configuration.IsDevelopment() || configuration.FakeUserEnabled())
                //{
                //    options.OperationFilter<FakeUserHeaderFilter>(Array.Empty<object>());
                //}

                if (!string.IsNullOrWhiteSpace(swaggerConfig.XmlDocumentationPath))
                {
                    options.IncludeXmlComments(swaggerConfig.XmlDocumentationPath);
                }

                options.IgnoreObsoleteProperties();
                options.CustomSchemaIds((Type x) => x.FullName);
            });
        }

        private static OpenApiInfo CreateInfoForApiVersion(SwaggerConfig swaggerConfig, ApiVersionDescription apiVersionDescription)
        {
            OpenApiInfo openApiInfo = new OpenApiInfo
            {
                Title = swaggerConfig.Title,
                Version = apiVersionDescription.ApiVersion.ToString(),
                Description = swaggerConfig.Description + "<p><strong>Version: </strong>" + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version + "</p>",
                Contact = new OpenApiContact
                {
                    Name = swaggerConfig.ContactName,
                    Email = swaggerConfig.ContactEmail,
                    Url = new Uri(swaggerConfig.ContactUrl)
                }
            };
            if (apiVersionDescription.IsDeprecated)
            {
                openApiInfo.Description += " This API version has been deprecated.";
            }

            return openApiInfo;
        }



        public static void UseSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(delegate (SwaggerUIOptions options)
            {
                if (
                    (from x in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetTypes())
                     where typeof(BaseApiController).IsAssignableFrom(x) && x.IsClass && x.Name != "BaseApiController"
                     select x
                    ).Count() > 3
                )
                {
                    options.DocExpansion(DocExpansion.None);
                }

                foreach (ApiVersionDescription item in provider.ApiVersionDescriptions.OrderByDescending((ApiVersionDescription avd) => avd.GroupName))
                {
                    options.SwaggerEndpoint("/swagger/" + item.GroupName + "/swagger.json", item.GroupName.ToUpperInvariant());
                }
            });
        }

    }
}
