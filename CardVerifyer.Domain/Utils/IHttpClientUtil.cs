using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardVerifyer.Domain.Utils
{
    public interface IHttpClientUtil
    {
        Task<string> GetWithBody(string url, Dictionary<string, string> headers = null, object req = null);        
    }
}