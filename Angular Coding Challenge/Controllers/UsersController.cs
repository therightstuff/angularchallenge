using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;

namespace Angular_Coding_Challenge.Controllers
{
    [RoutePrefix("api")]
    public class UsersController : ApiController
    {
        public const string INVALID_LOGIN_DATA = "You must submit username and password";
        public const string INVALID_USERNAME_OR_PASSWORD = "Invalid username or password";

        private byte[] GetEncodedPassword(string password, string salt) {

            // set default global salt for unit testing
            string globalSalt = "randomly generated salt goes here";
            try {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("salt"))) {
                    globalSalt = ConfigurationManager.AppSettings.Get("salt");
                }
            } catch (Exception) { }

            // concatenate password global and private salt strings
            string saltedPassword = String.Concat(
                password,
                salt,
                globalSalt
            );

            SHA256Managed sha256hasher = new SHA256Managed();
            return sha256hasher.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
        }

        [Route("login")]
        [HttpPost]
        public IHttpActionResult ValidatePassword(dynamic loginInfo) {
            string username;
            string password;

            if (JsonObjects.IsStringNullOrEmpty(loginInfo, "username") ||
                JsonObjects.IsStringNullOrEmpty(loginInfo, "password")
            ) {
                return BadRequest(INVALID_LOGIN_DATA);
            }

            try {
                username = loginInfo.username;
                password = loginInfo.password;

            } catch (Exception) {
                return BadRequest(INVALID_LOGIN_DATA);
            }

            // get user entity
            using (Models.ChallengeContext cc = new Models.ChallengeContext()) {
                Models.User user = cc.Users
                    .FirstOrDefault((u) => u.Username.Equals(username));
                if (user == null) {
                    return BadRequest(INVALID_USERNAME_OR_PASSWORD);
                }

                byte[] hash = GetEncodedPassword(password, user.Salt);

                // get stored hash in bytes
                byte[] bUserPassword = Convert.FromBase64String(user.Password);

                if (hash.SequenceEqual(bUserPassword)) {
                    // create and return a token
                    string token = createToken(username);
                    return Ok(token);
                } else {
                    return BadRequest(INVALID_USERNAME_OR_PASSWORD);
                }
            }
        }

        [Authorize]
        [Route("users")]
        public IHttpActionResult GetUsers() {
            using (Models.ChallengeContext cc = new Models.ChallengeContext()) {
                // username in Thread.CurrentPrincipal.Identity.Name
                Models.User adminUser = cc.Users
                    .FirstOrDefault(u => u.Username.Equals(Thread.CurrentPrincipal.Identity.Name));

                if (adminUser == null || !adminUser.IsAdministrator) {
                    return Unauthorized();
                } else {
                    List<Models.User> results = cc.Users.ToList();
                    return Json(JsonObjects.Jsonify(results));
                }
            }
        }

        /// <summary>
        /// Add index permissions to a user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tickers">an array of index ticker symbols</param>
        /// <returns>an updated list of the indices the user is authorized to view</returns>
        [Authorize]
        [Route("user/{id}/indices")]
        [HttpPost]
        public IHttpActionResult AddUserIndices([FromUri] long id, [FromBody] List<string> tickers)
        {
            using (Models.ChallengeContext cc = new Models.ChallengeContext()) {
                // username in Thread.CurrentPrincipal.Identity.Name
                Models.User adminUser = cc.Users
                    .FirstOrDefault(u => u.Username.Equals(Thread.CurrentPrincipal.Identity.Name));

                if (adminUser == null || !adminUser.IsAdministrator) {
                    return Unauthorized();
                }
            }
            return AddUserIndicesWithoutAuthorization(id, tickers);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult AddUserIndicesWithoutAuthorization(long id, List<string> tickers){
            using (Models.ChallengeContext cc = new Models.ChallengeContext()) {
                Models.User user = cc.Users
                    .FirstOrDefault(u => u.UserId == id);
                if (user == null) {
                    return BadRequest("user not found");
                } else {
                    try {
                        List<Models.Index> indices = cc.Indices
                            .Where(i => tickers.Contains(i.Ticker))
                            .ToList();

                        foreach (Models.Index index in indices) {
                            if (!user.Indices.Contains(index)) {
                                user.Indices.Add(index);
                            }
                        }

                        cc.SaveChanges();
                    } catch (Exception e) {
                        // terrible error handling
                        return InternalServerError(e);
                    }
                }
                return Json(JsonObjects.Jsonify(user.Indices.ToList()));
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string createToken(string username)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;

            // default token expiration to 30 minutes
            long sessionExpirationInSeconds = 1800;
            try {
                sessionExpirationInSeconds = long.Parse(ConfigurationManager.AppSettings.Get("sessionExpirationInSeconds"));
            } catch (Exception) {
                // for unit testing and invalid expiration configuration
            }
            DateTime expires = DateTime.UtcNow.AddSeconds(sessionExpirationInSeconds);

            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            });

            string secret = "randomly generated jwt secret goes here";
            try {
                string configSecret = ConfigurationManager.AppSettings.Get("jwtSecret");
                if (!string.IsNullOrEmpty(configSecret)) {
                    secret = configSecret;
                }
            } catch (Exception) {
                // for unit testing
            }

            var now = DateTime.UtcNow;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secret));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


            String host = "http://localhost";
            try {
                Uri uri = Request.RequestUri;
                host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            } catch (Exception) {
                // for unit testing
            }

            //create the jwt
            JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(
                issuer: host,
                audience: host,
                subject: claimsIdentity,
                notBefore: issuedAt,
                expires: expires,
                signingCredentials: signingCredentials
            );

            return tokenHandler.WriteToken(token);
        }
    }
}
