// <copyright file="TransaksjonDALTest.cs">Copyright ©  2016</copyright>

using System;
using DAL;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DAL.Tests
{
    [TestClass]
    [PexClass(typeof(TransaksjonDAL))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TransaksjonDALTest
    {

        [PexMethod]
        public bool slettTransaksjon([PexAssumeUnderTest]TransaksjonDAL target, int tID)
        {
            bool result = target.slettTransaksjon(tID);
            return result;
            // TODO: add assertions to method TransaksjonDALTest.slettTransaksjon(TransaksjonDAL, Int32)
        }
    }
}
