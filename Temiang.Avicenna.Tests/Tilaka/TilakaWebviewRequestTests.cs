using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaWebviewRequestTests
    {
        [TestMethod]
        public void ResolveRequestId_PrefersCanonicalThenAliases()
        {
            Assert.AreEqual("canonical",
                TilakaWebviewRequest.ResolveRequestId("canonical", "alias", "legacy"));
            Assert.AreEqual("alias",
                TilakaWebviewRequest.ResolveRequestId(" ", "alias", "legacy"));
            Assert.AreEqual("legacy",
                TilakaWebviewRequest.ResolveRequestId(null, null, "legacy"));
            Assert.IsNull(TilakaWebviewRequest.ResolveRequestId(" ", null, null));
        }
    }
}
