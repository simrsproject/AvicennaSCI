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
                    "CancelOpenClaimsTxn", "CheckIcdExclusion", "CheckMemberClaimTxnStatus", "DailyMonitoring",
                    "DischargeOp", "EdocsUpload", "Eligibility", "GetEntitlement", "GetMemberEnrolledPlanTc",
                    "HelloWorld", "InvoiceTracking", "RePrintOut"
                },
                methods);
        }

        [TestMethod]
        public void AdmedikaWS_WebMethodsHaveUniqueDescriptions()
        {
            var descriptions = typeof(Temiang.Avicenna.WebService.AdmedikaWS)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(m => m.GetCustomAttributes(typeof(WebMethodAttribute), true)
                    .Cast<WebMethodAttribute>().FirstOrDefault())
                .Where(attribute => attribute != null)
                .Select(attribute => attribute.Description)
                .ToArray();

            Assert.AreEqual(12, descriptions.Length);
            Assert.IsTrue(descriptions.All(d => !string.IsNullOrWhiteSpace(d)),
                "Setiap WebMethod harus memiliki Description.");
            Assert.IsTrue(descriptions.All(d => d.Contains(". from source : ")),
                "Setiap Description harus menyebutkan file sumber.");
            Assert.IsTrue(descriptions.All(d => d.EndsWith(".pdf")),
                "Setiap Description harus diakhiri nama file PDF.");
            Assert.AreEqual(12, descriptions.Distinct().Count(),
                "Description antar WebMethod harus unik.");

            var service = typeof(Temiang.Avicenna.WebService.AdmedikaWS)
                .GetCustomAttributes(typeof(WebServiceAttribute), true)
                .Cast<WebServiceAttribute>()
                .First();

            Assert.IsFalse(string.IsNullOrWhiteSpace(service.Description));
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
