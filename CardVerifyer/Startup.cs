using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSwag.AspNetCore;

namespace CardVerifyer
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDIServices(services);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowInProduction",
                           policy => policy.WithOrigins("https://cardverifyer.bot"));
                options.AddPolicy("AllowInDevelopment",
                     policy => policy.AllowAnyOrigin());
            });

            services.AddRouting(options => options.LowercaseUrls = true)
            .AddResponseCaching()
            .AddLogging();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader
                    = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector
                     = new CurrentImplementationApiVersionSelector(options);
            });
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("default", new CacheProfile
                {
                   Location = ResponseCacheLocation.Any
                });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("AllowInDevelopment");
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi3WithApiExplorer(options =>
                {
                    options.GeneratorSettings
                        .DefaultPropertyNameHandling
                    = NJsonSchema.PropertyNameHandling.CamelCase;
                });
            }
            else
            {
                app.UseCors("AllowInProduction");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;
                var result = JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));
            app.UseAuthentication();
            app.UseResponseCaching();
            app.UseMvc();
        }
    }
}
