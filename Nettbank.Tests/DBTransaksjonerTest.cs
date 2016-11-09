// <copyright file="DBTransaksjonerTest.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettbank;

namespace Nettbank.Tests
{
    [TestClass]
    [PexClass(typeof(DBTransaksjoner))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DBTransaksjonerTest
    {

        [PexMethod]
        public bool slettTransaksjon([PexAssumeUnderTest]DBTransaksjoner target, int tID)
        {
            bool result = target.slettTransaksjon(tID);
            return result;
            // TODO: add assertions to method DBTransaksjonerTest.slettTransaksjon(DBTransaksjoner, Int32)
        }
    }
}
