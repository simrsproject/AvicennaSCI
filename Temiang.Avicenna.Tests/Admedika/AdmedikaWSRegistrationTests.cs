using System;
using System.Linq;
using System.Reflection;
using System.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaWSRegistrationTests
    {
        [TestMethod]
        public void AdmedikaWS_ExposesExpectedWebMethods()
        {
            var methods = typeof(Temiang.Avicenna.WebService.AdmedikaWS)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(typeof(WebMethodAttribute), true).Length > 0)
                .Select(m => m.Name)
                .OrderBy(n => n)
                .ToArray();

            CollectionAssert.AreEqual(
                new[]
                {
                    "CancelOpenClaimsTxn", "CheckIcdExclusion", "CheckMemberClaimTxnStatus", "DischargeOp",
                    "Eligibility", "GetEntitlement", "GetMemberEnrolledPlanTc", "HelloWorld"
                },
                methods);
        }

        [TestMethod]
        public void AdmedikaWS_HasPublicParameterlessConstructor()
        {
            var constructor = typeof(Temiang.Avicenna.WebService.AdmedikaWS)
                .GetConstructor(Type.EmptyTypes);

            Assert.IsNotNull(constructor, "ASMX memerlukan constructor tanpa parameter.");
        }
    }
}
