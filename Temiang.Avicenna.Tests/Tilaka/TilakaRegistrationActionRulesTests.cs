using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaRegistrationActionRulesTests
    {
        private static TilakaRegistrationRecord Record()
        {
            return new TilakaRegistrationRecord { TilakaName = "anita001" };
        }

        [TestMethod]
        public void ChangeMfa_AllowsActiveWithTilakaName()
        {
            Assert.IsTrue(TilakaRegistrationActionRules.CanChangeMfa(
                Record(), TilakaRegistrationState.Aktif));
        }

        [TestMethod]
        public void ChangeMfa_RejectsBlankTilakaName()
        {
            var record = Record();
            record.TilakaName = " ";
            Assert.IsFalse(TilakaRegistrationActionRules.CanChangeMfa(
                record, TilakaRegistrationState.Aktif));
        }

        [TestMethod]
        public void ChangeMfa_RejectsNonActiveState()
        {
            Assert.IsFalse(TilakaRegistrationActionRules.CanChangeMfa(
                Record(), TilakaRegistrationState.SertifikatTerbit));
        }

        [TestMethod]
        public void Revoke_RejectsAlreadySuccessfulRevoke()
        {
            var record = Record();
            record.RevokeStatus = TilakaConstant.RevokeStatus.Success;
            Assert.IsFalse(TilakaRegistrationActionRules.CanRevoke(
                record, TilakaRegistrationState.Aktif));
        }

        [TestMethod]
        public void Revoke_AllowsRequestedRetry()
        {
            var record = Record();
            record.RevokeStatus = TilakaConstant.RevokeStatus.Requested;
            Assert.IsTrue(TilakaRegistrationActionRules.CanRevoke(
                record, TilakaRegistrationState.Aktif));
        }

        [TestMethod]
        public void Revoke_RejectsNonESignatureDoctor()
        {
            Assert.IsFalse(TilakaRegistrationActionRules.CanRevoke(
                Record(), TilakaRegistrationState.Aktif, false));
            Assert.IsTrue(TilakaRegistrationActionRules.CanRevoke(
                Record(), TilakaRegistrationState.Aktif, true));
        }
    }
}
