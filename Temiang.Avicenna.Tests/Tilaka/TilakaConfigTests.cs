using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaConfigTests
    {
        [TestMethod]
        public void ResolveFilePath_RootedPath_IsUsedAsIs()
        {
            var result = TilakaConfig.ResolveFilePath(@"C:\Tilaka\TilakaConsent.txt", p => @"D:\app\" + p);
            Assert.AreEqual(@"C:\Tilaka\TilakaConsent.txt", result);
        }

        [TestMethod]
        public void ResolveFilePath_VirtualPath_IsMapped()
        {
            var result = TilakaConfig.ResolveFilePath("~/App_Data/TilakaConsent.txt", p => @"D:\app\App_Data\TilakaConsent.txt");
            Assert.AreEqual(@"D:\app\App_Data\TilakaConsent.txt", result);
        }

        [TestMethod]
        public void ResolveFilePath_Empty_ReturnsNull()
        {
            Assert.IsNull(TilakaConfig.ResolveFilePath("", p => @"D:\app"));
        }

        [TestMethod]
        public void IsCompanyChannel_TrueWhenCompany()
        {
            var config = new TilakaConfig { ChannelType = "Company" };
            Assert.IsTrue(config.IsCompanyChannel);
        }
    }
}
