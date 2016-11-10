using Nettbank.Models;
using System.Web.Mvc;
// <copyright file="KontoControllerTest.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettbank.Controllers;

namespace Nettbank.Controllers.Tests
{
    [TestClass]
    [PexClass(typeof(KontoController))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class KontoControllerTest
    {

        [PexMethod]
        public ActionResult OpprettKonto01([PexAssumeUnderTest]KontoController target, Konto innKonto)
        {
            ActionResult result = target.OpprettKonto(innKonto);
            return result;
            // TODO: add assertions to method KontoControllerTest.OpprettKonto01(KontoController, Konto)
        }
    }
}
