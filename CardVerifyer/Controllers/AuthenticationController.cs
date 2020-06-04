using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Core;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardVerifyer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly OpenIddictApplicationManager<OpenIddictApplication> _openIddictApplicationManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            OpenIddictApplicationManager<OpenIddictApplication> openIddictApplicationManager,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AuthenticationController> logger)
        {
            _openIddictApplicationManager = openIddictApplicationManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token(OpenIdConnectRequest openIdConnectRequest)
        {
            _logger.LogInformation($"OpenIdConnectRequest: {JsonConvert.SerializeObject(openIdConnectRequest)}");
            if (!openIdConnectRequest.IsPasswordGrantType())
            {
                // Return bad request if the request is not for password grant type
                _logger.LogError($"{OpenIdConnectConstants.Errors.UnsupportedGrantType}");
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported."
                });
            }

            var user = await _userManager.FindByNameAsync(openIdConnectRequest.Username);
            if (user == null)
            {
                _logger.LogError($"{OpenIdConnectConstants.Errors.InvalidGrant}, -Invalid username or password");
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "Invalid username or password"
                });
            }
            if (!await _signInManager.CanSignInAsync(user) || (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)))
            {
                _logger.LogError($"{OpenIdConnectConstants.Errors.InvalidGrant}, -Account has been locked out");
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "Account is locked"
                });
            }

            if (!await _userManager.CheckPasswordAsync(user, openIdConnectRequest.Password))
            {
                _logger.LogError($"{OpenIdConnectConstants.Errors.InvalidGrant}, -Invalid username or password");
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "Invalid username or password"
                });
            }

            // The user is now validated, so reset lockout counts, if necessary
            if (_userManager.SupportsUserLockout)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                _logger.LogInformation($"userObject: {JsonConvert.SerializeObject(user)}");
            }

            // Create the principal
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

           
            foreach (var claim in principal.Claims)
            {                
                claim.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            }

            // Create a new authentication ticket for the user's principal
            var ticket = new AuthenticationTicket(
                principal,
                new AuthenticationProperties(),
                OpenIdConnectServerDefaults.AuthenticationScheme);

            // Include resources and scopes, as appropriate
            var scope = new[]
            {
                OpenIdConnectConstants.Scopes.OpenId,
                OpenIdConnectConstants.Scopes.Email,
                OpenIdConnectConstants.Scopes.Profile,
                OpenIdConnectConstants.Scopes.OfflineAccess,
                OpenIddictConstants.Scopes.Roles
            }.Intersect(openIdConnectRequest.GetScopes());

            ticket.SetScopes(scope);

            _logger.LogInformation($"scope: {JsonConvert.SerializeObject(scope)}");
            // Sign in the user
            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

    }
}