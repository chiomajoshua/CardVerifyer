using CardVerifyer.Contract;
using CardVerifyer.Data.DataTransferObjects;
using CardVerifyer.Domain.Utils;
using CardVerifyer.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CardVerifyer
{
    public partial class Startup
    {
        public IServiceCollection ConfigureDIServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICardValidationService<RootDataDto>, CardValidationService>();
            serviceCollection.AddTransient<IHttpClientUtil, HttpClientUtil>();

            return serviceCollection;
        }
    }
}
