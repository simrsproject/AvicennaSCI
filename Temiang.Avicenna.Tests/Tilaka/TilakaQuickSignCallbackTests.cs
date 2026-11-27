using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaQuickSignCallbackTests
    {
        private class FakeRepository : ITilakaQuickSignRepository
        {
            public TilakaQuickSignRecord Record;
            public List<TilakaQuickSignSignerRecord> Signers = new List<TilakaQuickSignSignerRecord>();
            public int SaveCalls;

            public TilakaQuickSignRecord FindByRequestId(string requestId)
            {
                return Record != null && Record.RequestID == requestId ? Record : null;
            }

            public IList<TilakaQuickSignSignerRecord> LoadSigners(long quickSignRequestId)
            {
                return Signers;
            }

            public IList<TilakaQuickSignRecord> FindPending(int max)
            {
                return new List<TilakaQuickSignRecord> { Record };
            }

            public void Save(TilakaQuickSignRecord record, IList<TilakaQuickSignSignerRecord> signers)
            {
                SaveCalls++;
                Record = record;
                Signers = new List<TilakaQuickSignSignerRecord>(signers ?? new List<TilakaQuickSignSignerRecord>());
            }
        }

        private class FakeDownloader : ITilakaSignedFileDownloader
        {
            public bool ShouldFail;
            public int Calls;

            public string Download(string requestId, string fileName, string presignedUrl)
            {
                Calls++;
                if (ShouldFail) throw new InvalidOperationException("expired");
                return "2026/11/" + requestId + "/" + fileName;
            }
        }

        private class FakeCasemixRepository : ITilakaCasemixFileRepository
        {
            public string DownloadPath;
            public long RequestID;
            public int Calls;

            public TilakaCasemixFileRecord FindByID(long casemixFileID) { return null; }

            public IList<TilakaCasemixFileRecord> FindAll()
            {
                return new List<TilakaCasemixFileRecord>();
            }

            public void UpdateLink(long casemixFileID, long quickSignRequestID, DateTime processedDateTime) { }

            public void UpdateDownloadPath(long casemixFileID, long quickSignRequestID, string downloadPath)
            {
                Calls++;
                RequestID = quickSignRequestID;
                DownloadPath = downloadPath;
            }

            public void Delete(long casemixFileID) { }
        }

        private static FakeRepository Repository()
        {
            return new FakeRepository
            {
                Record = new TilakaQuickSignRecord
                {
                    QuickSignRequestID = 1,
                    CasemixFileID = 10,
                    RequestID = "req-1",
                    FileName = "doc.pdf",
                    Status = TilakaQuickSignConstant.RequestStatus.Requested,
                    IsActive = true
                },
                Signers = new List<TilakaQuickSignSignerRecord>
                {
                    new TilakaQuickSignSignerRecord
                    {
                        QuickSignSignerID = 1,
                        Sequence = 1,
                        UserIdentifier = "dr001",
                        Status = TilakaQuickSignConstant.SignerStatus.Process
                    }
                }
            };
        }

        private static CallbackSigningRequest DonePayload()
        {
            return new CallbackSigningRequest
            {
                RequestId = "req-1",
                Success = true,
                Message = "DONE",
                UserInfo = new QuickSignSignerInfo
                {
                    Sequence = 1,
                    UserIdentifier = "dr001",
                    Status = TilakaQuickSignConstant.SignerStatus.Done,
                    NumSignatures = 1,
                    NumSignaturesDone = 1
                },
                SignInfo = new[]
                {
                    new QuickSignSignInfo
                    {
                        FileName = "doc.pdf",
                        Status = "DONE",
                        PresignedUrl = "http://file"
                    }
                }
            };
        }

        [TestMethod]
        public void Done_DownloadsAndCompletes()
        {
            var repository = Repository();
            var downloader = new FakeDownloader();
            var casemix = new FakeCasemixRepository();
            var processor = new TilakaQuickSignCallbackProcessor(repository, downloader, casemix);

            var outcome = processor.Process(DonePayload());

            Assert.IsTrue(outcome.Applied);
            Assert.AreEqual(1, downloader.Calls);
            Assert.AreEqual(TilakaQuickSignConstant.RequestStatus.Completed, repository.Record.Status);
            Assert.IsFalse(repository.Record.IsActive);
            Assert.AreEqual("2026/11/req-1/doc.pdf", repository.Record.SignedFilePath);
            Assert.AreEqual("2026/11/req-1/doc.pdf", casemix.DownloadPath);
            Assert.AreEqual(1, casemix.RequestID);
        }

        [TestMethod]
        public void DownloadFailure_KeepsInProgress()
        {
            var repository = Repository();
            var downloader = new FakeDownloader { ShouldFail = true };
            var casemix = new FakeCasemixRepository();
            var processor = new TilakaQuickSignCallbackProcessor(repository, downloader, casemix);

            processor.Process(DonePayload());

            Assert.AreEqual(TilakaQuickSignConstant.RequestStatus.InProgress, repository.Record.Status);
            Assert.IsTrue(repository.Record.IsActive);
            StringAssert.Contains(repository.Record.ErrorMessage, "Gagal mengunduh");
            Assert.AreEqual(0, casemix.Calls);
        }

        [TestMethod]
        public void ProcessAfterDone_DoesNotRegress()
        {
            var repository = Repository();
            repository.Record.Status = TilakaQuickSignConstant.RequestStatus.Completed;
            repository.Signers[0].Status = TilakaQuickSignConstant.SignerStatus.Done;
            var processor = new TilakaQuickSignCallbackProcessor(
                repository, new FakeDownloader(), new FakeCasemixRepository());

            var outcome = processor.Process(DonePayload());

            Assert.IsTrue(outcome.Applied);
            Assert.AreEqual(TilakaQuickSignConstant.RequestStatus.Completed, repository.Record.Status);
        }

        [TestMethod]
        public void UnknownRequest_IsNotApplied()
        {
            var repository = Repository();
            var processor = new TilakaQuickSignCallbackProcessor(
                repository, new FakeDownloader(), new FakeCasemixRepository());
            var payload = DonePayload();
            payload.RequestId = "unknown";

            var outcome = processor.Process(payload);

            Assert.IsFalse(outcome.Applied);
            Assert.AreEqual(0, repository.SaveCalls);
        }
    }
}
