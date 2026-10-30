using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaTokenTests
    {
        [TestMethod]
        public void CreateToken_MatchesDocumentedExample()
        {
            var token = TokenGenerator.CreateToken(
                "MY_CUSTOMER_ID",
                "MySecurityWordHere",
                new DateTime(2017, 11, 8, 18, 17, 56),
                "51515",
                AdmedikaConstant.ServiceID.HelloWorld);

            Assert.AreEqual(
                "0f58fb94999566130f8b4cce6408473968403684fc6db24ac142b6a33d908bfe",
                token);
        }

        [TestMethod]
        public void CreateToken_WithUnicodeSecurityWord_IsStableAndCaseSensitive()
        {
            var first = TokenGenerator.CreateToken(
                "CUST001", " Rahasiaü ", new DateTime(2026, 10, 30, 7, 0, 0), "100001",
                AdmedikaConstant.ServiceID.HelloWorld);
            var second = TokenGenerator.CreateToken(
                "CUST001", "Rahasiaü", new DateTime(2026, 10, 30, 7, 0, 0), "100001",
                AdmedikaConstant.ServiceID.HelloWorld);

            Assert.AreEqual(64, first.Length);
            Assert.AreEqual(first.ToLowerInvariant(), first);
            Assert.AreNotEqual(first, second, "Security word harus dipakai apa adanya, tanpa trim.");
        }

        [TestMethod]
        public void CreateToken_DifferentServiceId_ProducesDifferentToken()
        {
            var hello = TokenGenerator.CreateToken(
                "CUST001", "secret", new DateTime(2026, 10, 30, 7, 0, 0), "100001",
                AdmedikaConstant.ServiceID.HelloWorld);
            var icd = TokenGenerator.CreateToken(
                "CUST001", "secret", new DateTime(2026, 10, 30, 7, 0, 0), "100001",
                AdmedikaConstant.ServiceID.CheckIcdExclusion);

            Assert.AreNotEqual(hello, icd);
        }

        [TestMethod]
        public void Next_ReturnsSixDigitsAndUniqueValues()
        {
            var values = new HashSet<string>();
            for (var i = 0; i < 1000; i++)
            {
                var id = RequestIdGenerator.Next();
                Assert.AreEqual(6, id.Length);
                Assert.IsTrue(int.Parse(id, CultureInfo.InvariantCulture) >= 100000);
                Assert.IsTrue(values.Add(id), "requestID duplikat: " + id);
            }
        }
    }
}
