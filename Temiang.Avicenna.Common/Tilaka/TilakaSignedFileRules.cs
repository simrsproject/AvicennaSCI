using System;
using System.IO;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaSignedFileRules
    {
        public static string BuildRelativePath(string requestId, string fileName, DateTime now)
        {
            var safeRequest = SafeName(requestId);
            var safeFile = SafeName(Path.GetFileName(fileName ?? string.Empty));

            return string.Format("{0:yyyy}/{0:MM}/{1}/{2}", now, safeRequest, safeFile);
        }

        public static bool ShouldUpdateDownloadPath(string existingPath,
            long? lastQuickSignRequestID, long quickSignRequestID)
        {
            return string.IsNullOrWhiteSpace(existingPath)
                   || !lastQuickSignRequestID.HasValue
                   || lastQuickSignRequestID.Value == quickSignRequestID;
        }

        private static string SafeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "UNKNOWN";

            foreach (var ch in Path.GetInvalidFileNameChars())
                value = value.Replace(ch, '_');

            value = value.Replace("..", "_").Trim().Trim('.');
            return string.IsNullOrWhiteSpace(value) ? "UNKNOWN" : value;
        }
    }
}
