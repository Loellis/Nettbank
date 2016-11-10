using Nettbank.Models;
// <copyright file="DBKontoTest.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettbank;

namespace Nettbank.Tests
{
    [TestClass]
    [PexClass(typeof(DBKonto))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DBKontoTest
    {

        [PexMethod]
        public bool lagKonto([PexAssumeUnderTest]DBKonto target, Konto innKonto)
        {
            bool result = target.lagKonto(innKonto);
            return result;
            // TODO: add assertions to method DBKontoTest.lagKonto(DBKonto, Konto)
        }
    }
}
