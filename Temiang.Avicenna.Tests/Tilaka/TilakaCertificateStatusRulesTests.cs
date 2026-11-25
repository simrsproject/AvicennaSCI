using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaCertificateStatusRulesTests
    {
        [TestMethod]
        public void NullCurrent_AllowsRegisteredStatuses()
        {
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(null, TilakaConstant.CertificateStatus.None));
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(null, TilakaConstant.CertificateStatus.InVerification));
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(null, TilakaConstant.CertificateStatus.Registered));
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(null, TilakaConstant.CertificateStatus.Active));
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(null, TilakaConstant.CertificateStatus.Rejected));
        }

        [TestMethod]
        public void ZeroCurrent_AllowsAnyRegisteredStatus()
        {
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.None, TilakaConstant.CertificateStatus.Active));
        }

        [TestMethod]
        public void InVerificationCurrent_RejectsRegression()
        {
            Assert.IsFalse(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.InVerification, TilakaConstant.CertificateStatus.None));
        }

        [TestMethod]
        public void InVerificationCurrent_AllowsSameAndHigher()
        {
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.InVerification, TilakaConstant.CertificateStatus.InVerification));
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.InVerification, TilakaConstant.CertificateStatus.Registered));
        }

        [TestMethod]
        public void RegisteredCurrent_AllowsActive()
        {
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.Registered, TilakaConstant.CertificateStatus.Active));
        }

        [TestMethod]
        public void ActiveCurrent_AllowsSameOnly()
        {
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.Active, TilakaConstant.CertificateStatus.Active));
            Assert.IsFalse(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.Active, TilakaConstant.CertificateStatus.Registered));
            Assert.IsFalse(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.Active, TilakaConstant.CertificateStatus.Rejected));
        }

        [TestMethod]
        public void RejectedCurrent_AllowsSameOnly()
        {
            Assert.IsTrue(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.Rejected, TilakaConstant.CertificateStatus.Rejected));
            Assert.IsFalse(TilakaCertificateStatusRules.CanApply(
                TilakaConstant.CertificateStatus.Rejected, TilakaConstant.CertificateStatus.Active));
        }

        [TestMethod]
        public void OutOfRangeStatus_IsRejected()
        {
            Assert.IsFalse(TilakaCertificateStatusRules.CanApply(null, -1));
            Assert.IsFalse(TilakaCertificateStatusRules.CanApply(null, 5));
        }
    }
}
