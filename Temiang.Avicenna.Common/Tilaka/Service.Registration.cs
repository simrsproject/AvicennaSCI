using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public partial class Service
    {
        public GenerateUuidResponse GenerateUuid(string name, string email)
        {
            var url = TilakaUrls.GenerateUuid(_config, name, email);
            return Execute<GenerateUuidResponse>("GenerateUUID", url, "{}");
        }

        public GenerateUuidResponse GenerateUuidReEnroll(string tilakaName)
        {
            return Execute<GenerateUuidResponse>("GenerateUUIDReEnroll",
                TilakaUrls.GenerateUuidReEnroll(_config, tilakaName), "{}");
        }

        public UserReEnrollStatusResponse GetUserReEnrollStatus(string registrationId)
        {
            var body = JsonConvert.SerializeObject(new { registration_id = registrationId });
            return Execute<UserReEnrollStatusResponse>("UserReEnrollStatus",
                TilakaUrls.UserReEnrollStatus(_config), body);
        }

        public RegisterForKycCheckResponse RegisterForKycCheck(RegisterForKycCheckRequest request)
        {
            var body = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            });

            return Execute<RegisterForKycCheckResponse>(
                "RegisterForKycCheck", TilakaUrls.RegisterForKycCheck(_config), body);
        }

        public UserRegStatusResponse GetUserRegistrationStatus(string registrationId)
        {
            var body = JsonConvert.SerializeObject(new { register_id = registrationId });
            return Execute<UserRegStatusResponse>("UserRegStatus", TilakaUrls.UserRegStatus(_config), body);
        }
    }
}
