using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace Angular_Coding_Challenge
{
    internal class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1) {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO: this awful hack is to manually prevent checking login route,
            // this circumvents a swagger ui issue that i'm not investing more
            // time in where once you've entered a value into the Authorization
            // (api_key) field you effectively cannot clear it
            if (request.RequestUri.AbsoluteUri.Contains("api/login")) {
                return base.SendAsync(request, cancellationToken);
            }

            HttpStatusCode statusCode;
            string token;
            //determine whether a jwt exists or not
            if (!TryRetrieveToken(request, out token)) {
                statusCode = HttpStatusCode.Unauthorized;
                //allow requests with no token - whether a action method needs an authentication can be set with the claimsauthorization attribute
                return base.SendAsync(request, cancellationToken);
            }

            try {
                //"401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
                string secret = ConfigurationManager.AppSettings.Get("jwtSecret");
                var now = DateTime.UtcNow;
                var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secret));

                SecurityToken securityToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                Uri uri = HttpContext.Current.Request.Url;
                String host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                TokenValidationParameters validationParameters = new TokenValidationParameters() {
                    ValidAudience = host,
                    ValidIssuer = host,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey
                };
                //extract and assign the user of the jwt
                Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
                HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            } catch (SecurityTokenValidationException) {
                statusCode = HttpStatusCode.Unauthorized;
                HttpContext.Current.Response.Write("invalid authorization token");
            } catch (Exception otherException) {
                statusCode = HttpStatusCode.InternalServerError;
                HttpContext.Current.Response.Write(string.Format(
                    "authorization validation error: {0}",
                    otherException.Message
                ));
            }
            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null) {
                if (DateTime.UtcNow < expires)
                    return true;
            }
            return false;
        }
    }
}