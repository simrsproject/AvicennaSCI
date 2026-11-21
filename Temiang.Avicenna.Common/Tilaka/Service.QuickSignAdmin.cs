using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public partial class Service
    {
        public QuickSignAddUsersResponse AddQuickSignUsers(string[] emails)
        {
            var request = new QuickSignAddUsersRequest();
            var users = new System.Collections.Generic.List<QuickSignAddUser>();
            if (emails != null)
            {
                foreach (var email in emails)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                        users.Add(new QuickSignAddUser { Email = email.Trim() });
                }
            }

            request.Users = users.ToArray();
            var body = JsonConvert.SerializeObject(request, QuickSignSerializer);
            return Execute<QuickSignAddUsersResponse>(
                "QuickSignAddUsers", TilakaQuickSignUrls.AddUsers(_config), body);
        }

        public QuickSignAddTemplatesResponse AddTemplates(QuickSignAddTemplatesRequest request)
        {
            var body = JsonConvert.SerializeObject(request, QuickSignSerializer);
            return Execute<QuickSignAddTemplatesResponse>(
                "QuickSignAddTemplates", TilakaQuickSignUrls.AddTemplates(_config), body);
        }

        public QuickSignAuthSessionResponse GetAuthSessions(string email, string authStatus,
            int pageNumber, int limit)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (limit <= 0 || limit > 25) limit = 25;

            return ExecuteGet<QuickSignAuthSessionResponse>(
                "QuickSignAuthSessions",
                TilakaQuickSignUrls.AuthSessions(_config, email, authStatus, pageNumber, limit));
        }
    }
}
