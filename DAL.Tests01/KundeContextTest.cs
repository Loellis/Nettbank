// <copyright file="KundeContextTest.cs">Copyright ©  2016</copyright>

using System;
using DAL;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DAL.Tests
{
    [TestClass]
    [PexClass(typeof(KundeContext))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class KundeContextTest
    {

        [PexMethod]
        public KundeContext Constructor()
        {
            KundeContext target = new KundeContext();
            return target;
            // TODO: add assertions to method KundeContextTest.Constructor()
        }
    }
}
