using System;
using System.Dynamic;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Angular_Coding_Challenge.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void UserPasswordBadRequest()
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            dynamic data = new ExpandoObject();
            data.username = "unregistered";
            IHttpActionResult result = controller.ValidatePassword(data);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(Controllers.UsersController.INVALID_LOGIN_DATA, ((BadRequestErrorMessageResult)result).Message);
        }

        [TestMethod]
        public void UserPasswordInvalidUsername()
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            dynamic data = new ExpandoObject();
            data.username = "unregistered";
            data.password = "irrelevant";
            IHttpActionResult result = controller.ValidatePassword(data);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(Controllers.UsersController.INVALID_USERNAME_OR_PASSWORD, ((BadRequestErrorMessageResult)result).Message);
        }

        [TestMethod]
        public void UserPasswordValidUsernameBadPassword()
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            dynamic data = new ExpandoObject();
            data.username = "admin";
            data.password = "wrong password";
            IHttpActionResult result = controller.ValidatePassword(data);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(Controllers.UsersController.INVALID_USERNAME_OR_PASSWORD, ((BadRequestErrorMessageResult)result).Message);
        }

        [TestMethod]
        public void UserPasswordValidUsernameValidPassword()
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            dynamic data = new ExpandoObject();
            data.username = "admin";
            data.password = "secure password admin";
            IHttpActionResult result = controller.ValidatePassword(data);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));
        }

        [TestMethod]
        public void CreateJwtToken()
        {
            Controllers.UsersController controller = new Controllers.UsersController();
            string token = controller.createToken("anyusername");
        }
    }
}
