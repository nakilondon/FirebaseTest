using AuthSeries.Models;
using AuthSeries.Services;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;

namespace AuthSeries.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;

        public UsersController(IConfiguration configuration, ITokenService tokenService)
        {
            this.configuration = configuration;
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route("sign-in")]
        public IActionResult Post(UserModel userModel)
        {
            return Ok(tokenService.BuildToken(configuration["Jwt:AuthDemo:Key"], configuration["Jwt:AuthDemo:ValidIssuer"], userModel));
        }

        [HttpPost]
        [Route("createwithClaims")]
        public async Task<IActionResult> createwithClaims(UserModel userModel)
        {

            UserRecordArgs args = new UserRecordArgs
            {
                Email = userModel.Email,
                Password = userModel.Password,
                DisplayName = "Jane Doe",
            };

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            var additionalClaims = new Dictionary<string, object>()
            {
                { "edit", true },
                { "admin", true },
                { "verified", true }
            };
       
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, additionalClaims);
            var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userRecord.Uid);

            return Ok(customToken);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> create(UserModel userModel)
        {

            UserRecordArgs args = new UserRecordArgs
            {
                Email = userModel.Email,
                Password = userModel.Password,
                DisplayName = "Jane Doe",
            };

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
    
            return Ok();
        }


    }
}