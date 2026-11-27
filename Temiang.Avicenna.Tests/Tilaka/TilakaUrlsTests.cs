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
        public void QuickSignAdminEndpoints_UseCloudBase()
        {
            var config = new TilakaConfig
            {
                LiteBaseUrl = "http://lite:8088",
                CloudBaseUrl = "https://sb-api.tilaka.id"
            };

            Assert.AreEqual("https://sb-api.tilaka.id/addquicksignusers",
                TilakaQuickSignUrls.AddUsers(config));
            Assert.AreEqual("https://sb-api.tilaka.id/quicksign-addtemplates",
                TilakaQuickSignUrls.AddTemplates(config));
            StringAssert.StartsWith(TilakaQuickSignUrls.AuthSessions(config, "", "", 1, 25),
                "https://sb-api.tilaka.id/quicksign-authsessions");
        }

        [TestMethod]
        public void QuickSignLiteEndpoints_UseLiteBase()
        {
            var config = new TilakaConfig
            {
                LiteBaseUrl = "http://lite:8088",
                CloudBaseUrl = "https://sb-api.tilaka.id"
            };

            Assert.AreEqual("http://lite:8088/api/v1/upload",
                TilakaQuickSignUrls.Upload(config));
            Assert.AreEqual("http://lite:8088/api/v1/requestquicksign",
                TilakaQuickSignUrls.RequestSign(config));
            Assert.AreEqual("http://lite:8088/api/v1/checksignstatus",
                TilakaQuickSignUrls.CheckSignStatus(config));
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

        [TestMethod]
        public void ChangeMfaWebview_UsesSettingTwoAndRedirectModeMfa()
        {
            var config = Config();
            config.WebviewRedirectUrl = "https://rs.example/TilakaWebview/Return";

            var url = TilakaUrls.ChangeMfaWebview(config, "anita 001");

            StringAssert.StartsWith(url, "https://sb-api.tilaka.id/personal-webview/login?");
            StringAssert.Contains(url, "setting=2");
            StringAssert.Contains(url, "tilaka_name=anita%20001");
            StringAssert.Contains(url, "channel_id=chan-1");
            StringAssert.Contains(
                url,
                "redirect_url=https%3A%2F%2Frs.example%2FTilakaWebview%2FReturn%3Fmode%3Dmfa");
        }

        [TestMethod]
        public void WebviewRedirect_AppendsModeAndRequestIdToExistingQuery()
        {
            var config = Config();
            config.WebviewRedirectUrl = "https://rs.example/TilakaWebview/Return?a=1";

            var url = TilakaUrls.WebviewRedirect(config, "revoke", "uuid-1");

            StringAssert.Contains(url, "a=1");
            StringAssert.Contains(url, "mode=revoke");
            StringAssert.Contains(url, "request_id=uuid-1");
        }

        [TestMethod]
        public void AppendRedirectUrl_EncodesRedirectUrl()
        {
            var url = TilakaUrls.AppendRedirectUrl(
                "https://sb-api.tilaka.id/personal-webview/kyc/revoke?revoke_id=rev-1",
                "https://rs.example/TilakaWebview/Return?mode=revoke&request_id=uuid-1");

            Assert.AreEqual(
                "https://sb-api.tilaka.id/personal-webview/kyc/revoke?revoke_id=rev-1" +
                "&redirect_url=https%3A%2F%2Frs.example%2FTilakaWebview%2FReturn" +
                "%3Fmode%3Drevoke%26request_id%3Duuid-1",
                url);
        }

        [TestMethod]
        public void RequestRevokeCertificate_UsesBaseUrl()
        {
            Assert.AreEqual(
                "https://sb-api.tilaka.id/requestRevokeCertificate",
                TilakaUrls.RequestRevokeCertificate(Config()));
        }
    }
}
