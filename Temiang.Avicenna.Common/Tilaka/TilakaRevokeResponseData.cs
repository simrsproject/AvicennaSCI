namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaRevokeResponseData
    {
        public string RevokeId { get; set; }
        public string RevokeUrl { get; set; }

        public static bool TryCreate(RequestRevokeCertificateResponse response,
            out TilakaRevokeResponseData data)
        {
            data = null;
            if (response == null || response.Data == null || response.Data.Length < 2)
                return false;

            if (string.IsNullOrWhiteSpace(response.Data[0])
                || string.IsNullOrWhiteSpace(response.Data[1]))
                return false;

            data = new TilakaRevokeResponseData
            {
                RevokeId = response.Data[0],
                RevokeUrl = response.Data[1]
            };
            return true;
        }
    }
}
