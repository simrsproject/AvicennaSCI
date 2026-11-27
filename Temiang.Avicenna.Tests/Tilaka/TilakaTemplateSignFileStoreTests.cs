using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaTemplateSignFileStoreTests
    {
        [TestMethod]
        public void Delete_RemovesSavedTemplateFile()
        {
            var root = CreateRoot();
            try
            {
                var store = new TilakaTemplateSignFileStore(root);
                var relativePath = store.Save("A001", "TTD-A001-01",
                    Encoding.UTF8.GetBytes("%PDF-1.4 test"), "application/pdf", DateTime.Now);

                Assert.IsNotNull(store.Load(relativePath));

                store.Delete(relativePath);

                Assert.IsNull(store.Load(relativePath));
            }
            finally
            {
                DeleteRoot(root);
            }
        }

        [TestMethod]
        public void Delete_MissingFile_DoesNotThrow()
        {
            var root = CreateRoot();
            try
            {
                var store = new TilakaTemplateSignFileStore(root);

                store.Delete("A001/missing.pdf");
            }
            finally
            {
                DeleteRoot(root);
            }
        }

        private static string CreateRoot()
        {
            return Path.Combine(Path.GetTempPath(), "TilakaTemplateStore_" + Guid.NewGuid().ToString("N"));
        }

        private static void DeleteRoot(string root)
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }
}
