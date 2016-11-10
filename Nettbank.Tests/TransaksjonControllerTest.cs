using System.Web.Mvc;
// <copyright file="TransaksjonControllerTest.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettbank.Controllers;

namespace Nettbank.Controllers.Tests
{
    [TestClass]
    [PexClass(typeof(TransaksjonController))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TransaksjonControllerTest
    {

        [PexMethod]
        public ActionResult visTransaksjoner([PexAssumeUnderTest]TransaksjonController target, int id)
        {
            ActionResult result = target.visTransaksjoner(id);
            return result;
            // TODO: add assertions to method TransaksjonControllerTest.visTransaksjoner(TransaksjonController, Int32)
        }
    }
}
