using Model;
// <copyright file="KontoBLLTest.cs">Copyright ©  2016</copyright>

using System;
using BLL;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    [PexClass(typeof(KontoBLL))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class KontoBLLTest
    {

        [PexMethod]
        public bool lagKonto(
            [PexAssumeUnderTest]KontoBLL target,
            Konto innKonto,
            int id
        )
        {
            bool result = target.lagKonto(innKonto, id);
            return result;
            // TODO: add assertions to method KontoBLLTest.lagKonto(KontoBLL, Konto, Int32)
        }
    }
}
