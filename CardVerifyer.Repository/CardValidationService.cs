using CardVerifyer.Contract;
using CardVerifyer.Data.DataTransferObjects;
using CardVerifyer.Domain.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CardVerifyer.Repository
{
    public class CardValidationService : ICardValidationService<RootDataDto>
    {
        private readonly ILogger<CardValidationService> _logger;
        private readonly IHttpClientUtil _httpClientUtil;
        private readonly IConfiguration _configuration;
        public CardValidationService(ILogger<CardValidationService> logger,
                                     IHttpClientUtil httpClientUtil,
                                     IConfiguration configuration)
        {
            _logger = logger;
            _httpClientUtil = httpClientUtil;
            _configuration = configuration;
        }

        public async Task<RootDataDto> ValidateCardInformation(string iin)
        {
            try
            {
                _logger.LogInformation($"iin Request: {iin}");
                #region GetFromAppSettings
                var baseUrl = _configuration.GetSection("BaseURL").Value;
                _logger.LogInformation($"BaseURL: {baseUrl}");
                #endregion
                return JsonConvert.DeserializeObject<RootDataDto>(await _httpClientUtil.GetWithBody(url: baseUrl + $"{iin}"));
            }
            catch(Exception exception)
            {
                _logger.LogError($"Error: {JsonConvert.SerializeObject(exception.InnerException?.Message)} {JsonConvert.SerializeObject(exception.InnerException?.StackTrace)}....");
                return null;
            }
        }
    }
}