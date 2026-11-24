using System;
using System.IO;
using System.Net;
using System.Text;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaSignedFileDownloader
    {
        string Download(string requestId, string fileName, string presignedUrl);
    }

    public class TilakaSignedFileDownloader : ITilakaSignedFileDownloader
    {
        private readonly string _rootPath;
        private readonly Service _service;

        public TilakaSignedFileDownloader(string rootPath, Service service)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new InvalidOperationException("TilakaQuickSignSignedFolder belum dikonfigurasi.");

            _rootPath = Path.GetFullPath(rootPath);
            _service = service;
        }

        public string Download(string requestId, string fileName, string presignedUrl)
        {
            if (string.IsNullOrWhiteSpace(presignedUrl))
                throw new InvalidOperationException("Presigned URL kosong.");

            var relative = TilakaSignedFileRules.BuildRelativePath(
                requestId, fileName, DateTime.Now);
            var fullPath = ResolveFullPath(relative);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            try
            {
                DownloadToFile(presignedUrl, fullPath);
            }
            catch (Exception)
            {
                if (_service == null) throw;
                var refreshed = _service.GetPresignedUrl(fileName);
                DownloadToFile(refreshed, fullPath);
            }

            return relative;
        }

        private static void DownloadToFile(string url, string fullPath)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 120000;
            request.ReadWriteTimeout = 120000;
            request.AllowAutoRedirect = true;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var file = File.Create(fullPath))
            {
                if (stream == null) throw new InvalidOperationException("Respons unduhan kosong.");
                stream.CopyTo(file);
            }
        }

        private string ResolveFullPath(string relativePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(
                _rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar)));
            var rootWithSeparator = _rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Path dokumen bertanda tangan tidak valid.");

            return fullPath;
        }

    }
}
