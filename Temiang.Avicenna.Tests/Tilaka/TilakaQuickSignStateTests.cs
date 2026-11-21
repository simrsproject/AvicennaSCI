using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaQuickSignStateTests
    {
        private static TilakaQuickSignRecord Request()
        {
            return new TilakaQuickSignRecord
            {
                QuickSignRequestID = 1,
                RequestID = "req-1",
                Status = TilakaQuickSignConstant.RequestStatus.Requested
            };
        }

        private static TilakaQuickSignSignerRecord Signer(string status, int sequence)
        {
            return new TilakaQuickSignSignerRecord { Sequence = sequence, Status = status };
        }

        [TestMethod]
        public void AllDone_IsCompleted()
        {
            var signers = new List<TilakaQuickSignSignerRecord>
            {
                Signer(TilakaQuickSignConstant.SignerStatus.Done, 1),
                Signer(TilakaQuickSignConstant.SignerStatus.Done, 2)
            };

            Assert.AreEqual(TilakaQuickSignConstant.RequestStatus.Completed,
                TilakaQuickSignState.Evaluate(Request(), signers));
        }

        [TestMethod]
        public void OneProcess_IsInProgress()
        {
            var signers = new List<TilakaQuickSignSignerRecord>
            {
                Signer(TilakaQuickSignConstant.SignerStatus.Done, 1),
                Signer(TilakaQuickSignConstant.SignerStatus.Process, 2)
            };

            Assert.AreEqual(TilakaQuickSignConstant.RequestStatus.InProgress,
                TilakaQuickSignState.Evaluate(Request(), signers));
        }

        [TestMethod]
        public void AllFinalFailure_IsFailed()
        {
            var signers = new List<TilakaQuickSignSignerRecord>
            {
                Signer(TilakaQuickSignConstant.SignerStatus.Failed, 1),
                Signer(TilakaQuickSignConstant.SignerStatus.Denied, 2)
            };

            Assert.AreEqual(TilakaQuickSignConstant.RequestStatus.Failed,
                TilakaQuickSignState.Evaluate(Request(), signers));
        }

        [TestMethod]
        public void Done_IsTerminal()
        {
            Assert.IsFalse(TilakaQuickSignState.CanApplySignerStatus(
                TilakaQuickSignConstant.SignerStatus.Done, TilakaQuickSignConstant.SignerStatus.Process));
            Assert.IsFalse(TilakaQuickSignState.CanApplySignerStatus(
                TilakaQuickSignConstant.SignerStatus.Done, TilakaQuickSignConstant.SignerStatus.Failed));
        }

        [TestMethod]
        public void Failed_CannotReturnToProcess()
        {
            Assert.IsFalse(TilakaQuickSignState.CanApplySignerStatus(
                TilakaQuickSignConstant.SignerStatus.Failed, TilakaQuickSignConstant.SignerStatus.Process));
            Assert.IsTrue(TilakaQuickSignState.CanApplySignerStatus(
                TilakaQuickSignConstant.SignerStatus.Process, TilakaQuickSignConstant.SignerStatus.Done));
        }
    }
}
