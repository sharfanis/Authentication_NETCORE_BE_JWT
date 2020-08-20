using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationUserController : Controller
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public ApplicationUserController(UserManager<ApplicationUser> userManager )
        {
            _userManager = userManager;
           // _signInManager = signInManager;

        }


        [HttpGet]
        [Route("test")]

        public string test()
        {
            return "shabih";
        }


        [HttpPost]
        [Route("Register")]
        // Post: /api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel userModel)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                FullName = userModel.FullName
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, userModel.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



    }
}