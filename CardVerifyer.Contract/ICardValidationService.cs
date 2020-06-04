using System.Threading.Tasks;

namespace CardVerifyer.Contract
{
    public interface ICardValidationService<T> where T : class
    {
        /// <summary>
        /// Method takes in the IIN and queries Binlist.net
        /// </summary>
        /// <param name="iin"></param>
        /// <returns></returns>
        Task<T> ValidateCardInformation(string iin);
    }
}