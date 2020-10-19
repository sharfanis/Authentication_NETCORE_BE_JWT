using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationUserController : Controller
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _applicationSettings;

        public ApplicationUserController(UserManager<ApplicationUser> userManager , IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            // _signInManager = signInManager;
            _applicationSettings = appSettings.Value;

        }


        [HttpGet]
        [Route("welcome")]

        public string test()
        {
            return "Welcome to Family API Backend .";
        }


        [HttpPost]
        [Route("Register")]
        // Post: /api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel userModel)
        {
            userModel.Role = "Level1Customer";

            var applicationUser = new ApplicationUser()
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                FullName = userModel.FullName
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, userModel.Password);
                //Adding the roles
                await _userManager.AddToRoleAsync(applicationUser, userModel.Role);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpPost]
        [Route("Login")]
        // Post: /api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.UserName);

            if( user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                // Get role assigned to the user.
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {

                        new Claim("UserID", user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType , role.FirstOrDefault())
                    }),

                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_applicationSettings.JWT_Secret)), 
                    SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);

                var token = tokenHandler.WriteToken(securityToken);

                return Ok(new { token });


            } else
            {
                return BadRequest(new { message = "Username or Password is incorrect." });
            }

        }


    }
}