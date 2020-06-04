using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

namespace CardVerifyer.Domain.Utils
{
    public class HttpClientUtil : IHttpClientUtil
    {      
        private readonly ILogger<HttpClientUtil> _logger;

        public HttpClientUtil(ILogger<HttpClientUtil> logger)
        {
            _logger = logger;
        }

      
        /// <summary>
        /// Sending a Get request with a json body
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<string> GetWithBody(string url, Dictionary<string, string> headers = null, object req = null)
        {
            var responseBody = string.Empty;
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                };
                _logger.LogInformation($"URL: {url}....");
                
                try
                {
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            request.Headers.Add(item.Key, item.Value);
                        }
                    }

                    var result = client.SendAsync(request).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var jsonContent = new MediaTypeHeaderValue("application/json").MediaType;
                        var content = result.Content.Headers.ContentType.MediaType;
                        if (content != jsonContent)
                        {
                            responseBody = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                            _logger.LogInformation($"Response: {responseBody}....");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(responseBody);
                            var con = doc.LastChild;
                            string jsonString = JsonConvert.SerializeXmlNode(con);
                            _logger.LogInformation($"Response: {jsonString}....");
                            return jsonString;
                        }

                        responseBody = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                        _logger.LogInformation($"Response: {responseBody}....");
                    }
                    else
                    {
                        responseBody = null;
                        _logger.LogInformation($"Response: {responseBody}....");
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Error: {JsonConvert.SerializeObject(exception.InnerException?.Message)} {JsonConvert.SerializeObject(exception.InnerException?.StackTrace)}....");
                    return responseBody;
                }
            }

            return responseBody;
        }        
        
    }
}