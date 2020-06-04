using System.ComponentModel.DataAnnotations;

namespace CardVerifyer.Data.ViewModels
{
    public class IINRequestModel
    {
        [Required]
        [MaxLength(8, ErrorMessage = "IIN cannot be more than 8 characters"), MinLength(6, ErrorMessage = "IIN cannot be less than 6 characters")]
        public string IIN { get; set; }
    }
}