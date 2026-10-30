using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Temiang.Avicenna.Common.Admedika
{
    public static class ResponseParser
    {
        public static T Parse<T>(string json) where T : AdmedikaResponse, new()
        {
            var result = new T { RawResponse = json };

            if (string.IsNullOrWhiteSpace(json))
            {
                result.TechnicalError = "Respons AdMedika kosong.";
                return result;
            }

            JObject root;
            try
            {
                root = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                result.TechnicalError = "Respons bukan JSON valid: " + ex.Message;
                return result;
            }

            var output = root["output"] as JObject ?? root;
            var nestedOutput = output["output"] as JObject;
            if (nestedOutput != null) output = nestedOutput;

            result.ReferenceID = Str(output, "referenceID");
            result.StatusCode = Str(output, "statusCode");
            result.StatusMsg = Str(output, "statusMsg");
            result.Format = Str(output, "format");
            result.TxnResponseDateTime = Str(output, "txnResponseDateTime");

            var txnData = AsObject(output["txnData"]);
            if (txnData == null) return result;

            result.ErrorCode = Str(txnData, "errorCode");
            result.ErrorDesc = Str(txnData, "errorDesc");

            var payload = FindPayload(txnData);
            if (payload == null) return result;

            try
            {
                JsonConvert.PopulateObject(payload.ToString(), result);
            }
            catch (Exception ex)
            {
                result.TechnicalError = "Payload respons tidak dapat dipetakan: " + ex.Message;
            }

            if (string.IsNullOrEmpty(result.ErrorCode)) result.ErrorCode = Str(payload, "errorCode");
            if (string.IsNullOrEmpty(result.ErrorDesc)) result.ErrorDesc = Str(payload, "errorDesc");

            return result;
        }

        private static JObject AsObject(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null) return null;
            if (token is JObject) return (JObject)token;

            if (token.Type == JTokenType.String)
            {
                try
                {
                    return JObject.Parse(token.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        private static JObject FindPayload(JObject txnData)
        {
            foreach (var property in txnData.Properties())
            {
                var wrapper = property.Value as JObject;
                if (wrapper == null) continue;

                foreach (var inner in wrapper.Properties())
                {
                    var payload = inner.Value as JObject;
                    if (payload != null) return payload;
                }

                return wrapper;
            }

            return null;
        }

        private static string Str(JObject obj, string name)
        {
            var token = obj[name];
            return token == null || token.Type == JTokenType.Null ? null : token.ToString();
        }
    }
}
