using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaUrlsTests
    {
        private static TilakaConfig Config()
        {
            return new TilakaConfig { BaseUrl = "https://sb-api.tilaka.id", AuthPath = "/auth", ChannelId = "chan-1" };
        }

        [TestMethod]
        public void Auth_UsesAuthBaseUrlWhenConfigured()
        {
            var config = Config();
            config.AuthBaseUrl = "https://auth.example";

            Assert.AreEqual("https://auth.example/auth", TilakaUrls.Auth(config));
        }

        [TestMethod]
        public void GenerateUuid_Personal_OmitsNameAndEmail()
        {
            var config = Config();
            config.ChannelType = "Personal";
            var url = TilakaUrls.GenerateUuid(config, "Nama KTP", "a@b.com");
            Assert.AreEqual("https://sb-api.tilaka.id/generateUUID", url);
        }

        [TestMethod]
        public void GenerateUuid_Company_AppendsEncodedNameAndEmail()
        {
            var config = Config();
            config.ChannelType = "Company";
            var url = TilakaUrls.GenerateUuid(config, "Nama KTP & Co", "a@b.com");
            Assert.AreEqual("https://sb-api.tilaka.id/generateUUID?name=Nama%20KTP%20%26%20Co&email=a%40b.com", url);
        }

        [TestMethod]
        public void RegistrationWebview_EncodesRedirectUrl()
        {
            var config = Config();
            config.WebviewRedirectUrl = "https://rs.example/Ret.aspx?a=1&b=2";
            var url = TilakaUrls.RegistrationWebview(config, "uuid-1");
            Assert.AreEqual(
                "https://sb-api.tilaka.id/personal-webview/guide?request_id=uuid-1&redirect_url=https%3A%2F%2Frs.example%2FRet.aspx%3Fa%3D1%26b%3D2",
                url);
        }

        [TestMethod]
        public void ActivationWebview_IncludesSettingAndChannel()
        {
            var config = Config();
            config.WebviewRedirectUrl = "https://rs.example/Ret.aspx";
            var url = TilakaUrls.ActivationWebview(config, "uuid-1");
            StringAssert.Contains(url, "setting=1");
            StringAssert.Contains(url, "channel_id=chan-1");
            StringAssert.Contains(url, "request_id=uuid-1");
        }

        [TestMethod]
        public void GenerateUuidReEnroll_IncludesRequestTypeAndUser()
        {
            var url = TilakaUrls.GenerateUuidReEnroll(Config(), "anita 001");

            StringAssert.Contains(url, "request_type=re_enroll");
            StringAssert.Contains(url, "user_identifier=anita%20001");
        }

        [TestMethod]
        public void ReEnrollWebview_UsesIssueId()
        {
            var url = TilakaUrls.ReEnrollWebview(Config(), "issue-1");

            StringAssert.Contains(url, "issue_id=issue-1");
        }

        [TestMethod]
        public void ActivationWebviewByIssueId_UsesIssueId()
        {
            var url = TilakaUrls.ActivationWebviewByIssueId(Config(), "issue-1");

            StringAssert.Contains(url, "issue_id=issue-1");
            StringAssert.Contains(url, "setting=1");
            StringAssert.Contains(url, "channel_id=chan-1");
        }
    }
}
