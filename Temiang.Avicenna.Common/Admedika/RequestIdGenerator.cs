using System.Globalization;
using System.Threading;

namespace Temiang.Avicenna.Common.Admedika
{
    public static class RequestIdGenerator
    {
        private static int _counter;

        public static string Next()
        {
            var value = Interlocked.Increment(ref _counter);
            var requestId = 100000 + (value % 900000);
            return requestId.ToString(CultureInfo.InvariantCulture);
        }
    }
}
