using CardVerifyer.Contract;
using CardVerifyer.Data.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CardVerifyer.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CardVerificationController : ControllerBase
    {
        private readonly ICardValidationService<RootDataDto> _cardValidationService;
        private readonly ILogger<CardVerificationController> _logger;
        private ResponseDataDto _responseDataDto;
        public CardVerificationController(ICardValidationService<RootDataDto> cardValidationService,
                                          ILogger<CardVerificationController> logger)
        {
            _cardValidationService = cardValidationService;
            _logger = logger;
            _responseDataDto = new ResponseDataDto{ ResponseCode = StatusCodes.Status400BadRequest, ResponseMessage = "IIN is either invalid or does not exist", ResponseObject = null};
        }

        [HttpPost(Name = nameof(Validator))]
        [ProducesResponseType(200)]
        [ResponseCache(Duration = 30)]
        public async Task<IActionResult> Validator(string iinRequest)
        {
            if(ModelState.IsValid)
            {
                var result = await _cardValidationService.ValidateCardInformation(iinRequest.Trim());
                if (result != null)
                {
                    result.Href = Url.Link(nameof(CardVerificationController.Validator), null);
                    _responseDataDto = new ResponseDataDto
                    {
                        ResponseObject = result,
                        ResponseCode = StatusCodes.Status200OK,
                        ResponseMessage = "Validation Successful"
                    };
                    return Ok(_responseDataDto);
                }                
            }
            return Ok(_responseDataDto);
        }
    }
}