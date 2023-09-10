using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BffDemo.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public record UserloginDto(string Username, string Password);

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserloginDto userloginDto)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userloginDto.Username),
                        new Claim(ClaimTypes.Role, "admin")
                    };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3),
            };

            await HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return Ok(new { Username = userloginDto.Username });
        }

        /// <summary>
        /// We cannot access the cookie in JavaScript. To check the auth state, we can send a request
        /// to /api/user/userinfo. So we can set our application state.
        /// </summary>
        [Authorize]
        [HttpGet("userinfo")]
        public IActionResult GetAccountinfo()
        {
            var authenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (!authenticated) { return Unauthorized(); }
            return Ok(new
            {
                Username = HttpContext.User.Identity?.Name,
                IsAdmin = HttpContext.User.IsInRole("admin"),
            });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return NoContent();
        }
    }
}