using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {

        private UserManager<ApplicationUser> _userManager;

        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize]
        [Route("GetUserInfo")]
        // Get : /api/UserProfile/GetUserInfo
        // We need to send the JWT token to access the auth
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;

            var user = await _userManager.FindByIdAsync(userId);

            return new
            {

                user.FullName,
                user.Email,
                user.UserName
            };

        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        [Route("ForAdmin")]

        public string GetForAdmin()
        {
            return "Web Mthod for admin";

        }


        [HttpGet]
        [Authorize(Roles = "Level1Customer")]
        [Route("ForLevel1Customer")]

        public string GetLevel1Customer()
        {
            return "Web Mthod for Level 1 Customer";

        }


        [HttpGet]
        [Authorize(Roles = "Admin,Level1Customer")]
        [Route("ForAdminOrCustomer")]

        public string GetAdminOrCustomer()
        {
            return "Web Mthod for admin or Level 1 Customer";

        }


    }
}