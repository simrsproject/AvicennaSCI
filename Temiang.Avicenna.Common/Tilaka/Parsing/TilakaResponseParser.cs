using System;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaResponseParser
    {
        public static T Parse<T>(string json, int httpStatus) where T : TilakaResponse, new()
        {
            var result = new T { RawResponse = json, HttpStatus = httpStatus };

            if (string.IsNullOrWhiteSpace(json))
            {
                result.TechnicalError = "Respons Tilaka kosong.";
                return result;
            }

            try
            {
                JsonConvert.PopulateObject(json, result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                result.TechnicalError = "Respons bukan JSON valid: " + ex.Message;
            }

            if (string.IsNullOrEmpty(result.TechnicalError) && httpStatus >= 400)
                result.TechnicalError = string.Format("HTTP {0} dari Tilaka.", httpStatus);

            return result;
        }
    }
}
