using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaKtpFileStoreTests
    {
        [TestMethod]
        public void Save_WritesFileAndReturnsRelativePath()
        {
            var root = Path.Combine(Path.GetTempPath(), "tilaka-test-" + Guid.NewGuid().ToString("N"));
            var store = new TilakaKtpFileStore(root, 2048);

            using (var bitmap = new Bitmap(100, 60))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                var relative = store.Save("DR001", "3273010101900001", stream.ToArray(), "image/jpeg", new DateTime(2026, 11, 20, 14, 35, 1));

                Assert.AreEqual("DR001/3273010101900001_20261120143501.jpg", relative);
                Assert.IsTrue(File.Exists(Path.Combine(root, relative.Replace('/', '\\'))));
            }

            Directory.Delete(root, true);
        }

        [TestMethod]
        public void Save_RejectsNonImage()
        {
            var root = Path.Combine(Path.GetTempPath(), "tilaka-test-" + Guid.NewGuid().ToString("N"));
            var store = new TilakaKtpFileStore(root, 2048);

            try
            {
                store.Save("DR001", "3273010101900001", new byte[] { 1, 2, 3 }, "application/pdf", DateTime.Now);
                Assert.Fail("Harus menolak konten non-image.");
            }
            catch (InvalidOperationException)
            {
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }
    }
}
