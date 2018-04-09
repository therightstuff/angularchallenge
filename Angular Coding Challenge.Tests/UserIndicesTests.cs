using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Angular_Coding_Challenge.Tests
{
    [TestClass]
    public class UserIndicesTests
    {

        [ClassInitialize]
        public static void prepareTestData(TestContext tc)
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            IHttpActionResult result = controller.AddUserIndicesWithoutAuthorization(2, new List<string> {
                "DIA"
            });
        }

        [TestMethod]
        public void AddUserIndexSuccess()
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            IHttpActionResult result = controller.AddUserIndicesWithoutAuthorization(2, new List<string> {
                "DIA"
            });
            Assert.IsInstanceOfType(result, typeof(JsonResult<List<dynamic>>));
            List<dynamic> results = ((JsonResult<List<dynamic>>)result).Content;
            bool found = false;
            foreach (dynamic index in results) {
                if ("DIA".Equals(index.Ticker)) {
                    found = true;
                }
            }
            Assert.IsTrue(found);
        }

        [TestMethod]
        public void AddUserFakeIndexFailure()
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            IHttpActionResult result = controller.AddUserIndicesWithoutAuthorization(2, new List<string> {
                "FAKE"
            });
            Assert.IsInstanceOfType(result, typeof(JsonResult<List<dynamic>>));
            List<dynamic> results = ((JsonResult<List<dynamic>>)result).Content;
            bool found = false;
            foreach (dynamic index in results) {
                if ("FAKE".Equals(index.Ticker)) {
                    found = true;
                }
            }
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void UserShowIndexSuccess()
        {
            Controllers.IndicesController indicesController = new Controllers.IndicesController();
            IHttpActionResult result = indicesController.GetIndexWithoutAuthorization("A", "DIA");

            Assert.IsInstanceOfType(result, typeof(JsonResult<ExpandoObject>));
            dynamic obj = ((JsonResult<ExpandoObject>)result).Content;
            Assert.AreEqual("DIA", obj.Ticker);
        }

        [TestMethod]
        public void UserShowIndexFailure()
        {
            // bad test, only guaranteed to work for initial state
            Controllers.IndicesController indicesController = new Controllers.IndicesController();
            IHttpActionResult result = indicesController.GetIndexWithoutAuthorization("A", "SPY");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        // too much of a hurry to test all the indices
    }
}
