using Nettbank.Models;
// <copyright file="DBKundeTest.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettbank;

namespace Nettbank.Tests
{
    [TestClass]
    [PexClass(typeof(DBKunde))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DBKundeTest
    {

        [PexMethod]
        public bool settKunde([PexAssumeUnderTest]DBKunde target, Kunde innKunde)
        {
            bool result = target.settKunde(innKunde);
            return result;
            // TODO: add assertions to method DBKundeTest.settKunde(DBKunde, Kunde)
        }
    }
}
