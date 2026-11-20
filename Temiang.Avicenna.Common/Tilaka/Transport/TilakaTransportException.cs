using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaTransportException : Exception
    {
        public int StatusCode { get; private set; }
        public string ResponseBody { get; private set; }

        public TilakaTransportException(string message, int statusCode, string responseBody)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public TilakaTransportException(string message, int statusCode, string responseBody, Exception inner)
            : base(message, inner)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
