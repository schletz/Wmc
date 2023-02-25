using Docker.DotNet;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Configuration;
using Spengernews.Application.Infrastructure;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Spengernews.Application.Model;

namespace Spengernews.Webapi.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;  // Needed to read the secret from appsettings.json
        private readonly SpengernewsContext _db;

        /// <summary>
        /// Read the HTTP Context and extract Username, GUID and Role from the submittet JWT.
        /// </summary>
        public AuthService(
            IConfiguration config, SpengernewsContext db, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _db = db;
            var httpContext = httpContextAccessor.HttpContext ?? throw new ApplicationException("No context");
            CurrentUser = httpContext.User.Identity?.Name;
            // We have written the Guid in the Claim, so we can read this value without asking
            // the database, see new Claim("Guid", user.Guid.ToString()) in Login.
            if (Guid.TryParse(
                httpContext.User.Claims.FirstOrDefault(c => c.Type == "Guid")?.Value,
                out var guid))
            {
                CurrentUserGuid = guid;
            }
            if (Enum.TryParse<Role>(httpContext.User.FindFirstValue(ClaimTypes.Role), out var role))
            {
                CurrentUserRole = role;
            }
        }

        public string? CurrentUser { get; private set; }
        public Guid? CurrentUserGuid { get; private set; }
        public Role? CurrentUserRole { get; private set; }

        /// <summary>
        /// Returns a JWT and sets the Username, GUID and Role.
        /// </summary>
        public async Task<string?> Login(string username, string password)
        {
            // Read the secret from appsettings.json via IConfiguration
            // This is NOT the salt of the user password! It is the key to sign the JWT, so
            // the client cannot manupulate our token.
            var secret = Convert.FromBase64String(_config["Secret"]);
            var lifetime = TimeSpan.FromHours(3);
            // User exists in our database and the calculated hash matches
            // the password hash in the database?
            var user = await _db.Authors.FirstOrDefaultAsync(a => a.Username == username);
            if (user is null) { return null; }
            if (!user.CheckPassword(password)) { return null; }

            CurrentUser = user.Username;
            CurrentUserRole = user.Role;
            CurrentUserGuid = user.Guid;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Payload for our JWT.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Write username to the claim (the "data zone" of the JWT).
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    // Write the role to the claim (optional)
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                    new Claim("Guid", user.Guid.ToString())
                }),
                Expires = DateTime.UtcNow + lifetime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}