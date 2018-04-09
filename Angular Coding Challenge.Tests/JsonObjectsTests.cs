using System;
using System.Dynamic;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Angular_Coding_Challenge.Tests
{
    [TestClass]
    public class JsonObjectsTests
    {
        [TestMethod]
        public void HasPropertyFalse()
        {
            dynamic test = new ExpandoObject();
            bool hasProperty = JsonObjects.HasProperty(test, "variableName");
            Assert.AreEqual(false, hasProperty);
        }

        [TestMethod]
        public void HasPropertyTrue()
        {
            dynamic test = new ExpandoObject();
            test.variableName = "valid string";
            bool hasProperty = JsonObjects.HasProperty(test, "variableName");
            Assert.AreEqual(true, hasProperty);
        }

        [TestMethod]
        public void StringNullIfObjectWithoutProperty()
        {
            dynamic test = new ExpandoObject();
            bool isNullOrEmpty = JsonObjects.IsStringNullOrEmpty(test, "variableName");
            Assert.AreEqual(true, isNullOrEmpty);
        }

        [TestMethod]
        public void StringNullIfObjectWithEmptyString()
        {
            dynamic test = new ExpandoObject();
            test.variableName = "";
            bool isNullOrEmpty = JsonObjects.IsStringNullOrEmpty(test, "variableName");
            Assert.AreEqual(true, isNullOrEmpty);
        }

        [TestMethod]
        public void StringNotNullIfObjectWithString()
        {
            dynamic test = new ExpandoObject();
            test.variableName = "valid string";
            bool isNullOrEmpty = JsonObjects.IsStringNullOrEmpty(test, "variableName");
            Assert.AreEqual(false, isNullOrEmpty);
        }
    }
}
