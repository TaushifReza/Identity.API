using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(string email, string password)
        {
            var user = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var result = await this._userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                /*var message = new MailMessage("ecoshop420@gmail.com", user.Email, "Please Confirm your email",
                    $"Please click on this link to confirm your email address: {confirmationToken}");
                using (var emailClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    emailClient.Credentials = new NetworkCredential("ecoshop420@gmail.com", "heaxhbpdjeszzhfy");
                    await emailClient.SendMailAsync(message);
                }*/
                return Ok(value: new
                {
                    userId = user.Id,
                    token = confirmationToken
                });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid email or password.");
            }
            var result  = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
            {
                return Ok("Login Success");
            }
            if (result.IsLockedOut)
            {
                return BadRequest("You are locked Out.");
            }
            return BadRequest("Login Failed");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok("Email address is successfully confirmed, you can now try to login.");
                }
            }

            return BadRequest("failed to validate");
        }
    }
}
