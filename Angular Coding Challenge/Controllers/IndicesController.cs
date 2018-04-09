using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace Angular_Coding_Challenge.Controllers
{
    [RoutePrefix("api/indices")]    
    public class IndicesController : ApiController
    {

        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetAllIndicesWithoutAuthorization(string username) {
            List<Models.Index> results;
            using (Models.ChallengeContext cc = new Models.ChallengeContext()) {
                Models.User user = cc.Users
                    .FirstOrDefault(u => u.Username.Equals(username));

                if (user == null) {
                    results = new List<Models.Index>();
                } else {
                    if (user.IsAdministrator) {
                        // return all indices
                        results = cc.Indices.ToList();
                    } else {
                        // return user's indices
                        results = user.Indices.ToList();
                    }
                }
            }
            return Json(JsonObjects.Jsonify(results));
        }

        /// <summary>
        /// Return a list of indices
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("")]
        public IHttpActionResult GetAllIndices()
        {
            return GetAllIndicesWithoutAuthorization(Thread.CurrentPrincipal.Identity.Name);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetIndexWithoutAuthorization(string username, string ticker) {
            using (Models.ChallengeContext cc = new Models.ChallengeContext()) {
                Models.User user = cc.Users
                    .FirstOrDefault(u => u.Username.Equals(username));

                if (user == null) {
                    return NotFound();
                } else {
                    Models.Index index;
                    if (user.IsAdministrator) {
                        // look for ticker in all indices
                        index = cc.Indices
                        .Where(i => i.Ticker.Equals(ticker))
                        .FirstOrDefault();
                    } else {
                        // look for ticker in user's indices
                        index = user.Indices
                            .Where(i => i.Ticker.Equals(ticker))
                            .FirstOrDefault();
                    }
                    if (index == null) {
                        return NotFound();
                    }
                    return Json(JsonObjects.Jsonify(index));
                }
            }
        }

        [Authorize]
        [Route("{ticker}")]
        public IHttpActionResult GetIndex(string ticker)
        {
            return GetIndexWithoutAuthorization(Thread.CurrentPrincipal.Identity.Name, ticker);
        }
    }
}
