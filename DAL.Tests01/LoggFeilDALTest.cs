// <copyright file="LoggFeilDALTest.cs">Copyright ©  2016</copyright>

using System;
using DAL;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DAL.Tests
{
    [TestClass]
    [PexClass(typeof(LoggFeilDAL))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class LoggFeilDALTest
    {

        [PexMethod]
        public void SkrivTilFil([PexAssumeUnderTest]LoggFeilDAL target, Exception feil)
        {
            target.SkrivTilFil(feil);
            // TODO: add assertions to method LoggFeilDALTest.SkrivTilFil(LoggFeilDAL, Exception)
        }
    }
}
