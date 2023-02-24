using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Spengernews.Application.Infrastructure;
using Spengernews.Webapi.Services;
using System.Threading.Tasks;

namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        // DTO class for the JSON body of the login request
        public record CredentialsDto(string username, string password);

        private readonly SpengernewsContext _db;
        private readonly AuthService _authService;
        // DI von AuthService funktioniert nur, wenn es im Service Provider
        // registriert wurde.
        public UserController(SpengernewsContext db, AuthService authService)
        {
            _db = db;
            _authService = authService;
        }

        /// <summary>
        /// POST /api/user/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CredentialsDto credentials)
        {
            var jwt = await _authService.Login(credentials.username, credentials.password);
            if (jwt is null) { return Unauthorized(); }
            // Return the token so the client can save this to send a bearer token in the
            // subsequent requests.
            return Ok(new
            {
                Username = _authService.CurrentUser,
                UserGuid = _authService.CurrentUserGuid,
                Role = _authService.CurrentUserRole,
                Token = jwt
            });
        }

        /// <summary>
        /// GET /api/user/me
        /// Gets information about the current (authenticated) user.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetUserdata()
        {
            // No username is set in HttpContext? Should never occur because we added the
            // Authorize annotation. But the properties are nullable, so we have to
            // check.
            var username = _authService.CurrentUser;
            //var username = HttpContext?.User.Identity?.Name;
            if (username is null) { return Unauthorized(); }

            // Valid token, but no user match in the database (maybe deleted by an admin).
            var user = _db.Authors.FirstOrDefault(a => a.Username == username);
            if (user is null) { return Unauthorized(); }
            return Ok(new
            {
                user.Firstname,
                user.Lastname,
                user.Email,
                user.Phone
            });
        }

        /// <summary>
        /// GET /api/user/all
        /// List all users.
        /// Only for users which has the role admin in the claim of the JWT.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var user = _db.Authors
                .Select(a => new
                {
                    a.Firstname,
                    a.Lastname,
                    a.Email,
                    a.Phone
                })
                .ToList();
            if (user is null) { return BadRequest(); }
            return Ok(user);
        }
    }
}
