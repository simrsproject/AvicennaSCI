using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaConfigTests
    {
        [TestMethod]
        public void DefaultConfig_UsesDevelopmentUrlAndSafeDefaults()
        {
            var config = new AdmedikaConfig();

            StringAssert.Contains(config.ServiceUrl, "customerhost_dev");
            StringAssert.Contains(config.ApiServiceUrl, "/api/customerhost_dev");
            Assert.AreEqual(60, config.TimeoutSeconds);
            Assert.IsTrue(config.IsLogEnabled);
            Assert.AreEqual(string.Empty, config.CustomerID);
            Assert.AreEqual(string.Empty, config.SecurityWord);
            Assert.AreEqual(string.Empty, config.TerminalID);
        }

        [TestMethod]
        public void Properties_CanBeOverriddenForTests()
        {
            var config = new AdmedikaConfig
            {
                ServiceUrl = "https://example.test/customerhost",
                ApiServiceUrl = "https://example.test/api/customerhost",
                CustomerID = "CUST001",
                SecurityWord = "secret",
                TerminalID = "12380291",
                TimeoutSeconds = 15,
                IsLogEnabled = false
            };

            Assert.AreEqual("https://example.test/customerhost", config.ServiceUrl);
            Assert.AreEqual("https://example.test/api/customerhost", config.ApiServiceUrl);
            Assert.AreEqual("CUST001", config.CustomerID);
            Assert.AreEqual("secret", config.SecurityWord);
            Assert.AreEqual("12380291", config.TerminalID);
            Assert.AreEqual(15, config.TimeoutSeconds);
            Assert.IsFalse(config.IsLogEnabled);
        }

        [TestMethod]
        public void ServiceIdConstants_MatchDocumentation()
        {
            Assert.AreEqual("HELLO_WORLD", AdmedikaConstant.ServiceID.HelloWorld);
            Assert.AreEqual("CHECK_ICD_EXCLUSION", AdmedikaConstant.ServiceID.CheckIcdExclusion);
            Assert.AreEqual("ELIGIBILITY", AdmedikaConstant.ServiceID.Eligibility);
            Assert.AreEqual("GET_ENTITLEMENT", AdmedikaConstant.ServiceID.GetEntitlement);
            Assert.AreEqual("DISCHARGE_OP", AdmedikaConstant.ServiceID.DischargeOp);
            Assert.AreEqual("CHECK_MEMBER_CLAIM_TXN_STATUS", AdmedikaConstant.ServiceID.CheckMemberClaimTxnStatus);
            Assert.AreEqual("GET_MEMBER_ENROLLED_PLAN_TC", AdmedikaConstant.ServiceID.GetMemberEnrolledPlanTc);
            Assert.AreEqual("CANCEL_OPEN_CLAIMS_TXN", AdmedikaConstant.ServiceID.CancelOpenClaimsTxn);
            Assert.AreEqual("DAILY_MONITORING", AdmedikaConstant.ServiceID.DailyMonitoring);
            Assert.AreEqual("EDOCS_UPLOAD", AdmedikaConstant.ServiceID.EdocsUpload);
            Assert.AreEqual("INVOICE_TRACKING", AdmedikaConstant.ServiceID.InvoiceTracking);
            Assert.AreEqual("RE_PRINT_OUT", AdmedikaConstant.ServiceID.RePrintOut);
        }
    }
}
