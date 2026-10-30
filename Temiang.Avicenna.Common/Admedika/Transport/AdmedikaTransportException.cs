using System;

namespace Temiang.Avicenna.Common.Admedika
{
    public class AdmedikaTransportException : Exception
    {
        public string ResponseBody { get; private set; }

        public AdmedikaTransportException(string message, string responseBody, Exception innerException)
            : base(message, innerException)
        {
            ResponseBody = responseBody;
        }
    }
}
