using AspNet.Security.OpenIdConnect.Primitives;
using CardVerifyer.Contract;
using CardVerifyer.Data.DataTransferObjects;
using CardVerifyer.Data.Persistence;
using CardVerifyer.Domain.Utils;
using CardVerifyer.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CardVerifyer
{
    public partial class Startup
    {
        public IServiceCollection ConfigureDIServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<DbContext, CardVerifyerDbContext>();
            serviceCollection.AddTransient<ICardValidationService<RootDataDto>, CardValidationService>();
            serviceCollection.AddTransient<IHttpClientUtil, HttpClientUtil>();
            serviceCollection.AddDbContext<CardVerifyerDbContext>(options =>
                                                        {
                                                            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                                                            options.UseOpenIddict();
                                                        });
        
            serviceCollection.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader
                    = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector
                     = new CurrentImplementationApiVersionSelector(options);
            });

            serviceCollection.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<CardVerifyerDbContext>()
                .AddDefaultTokenProviders();

            serviceCollection.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
            return serviceCollection;
        }
    }
}
