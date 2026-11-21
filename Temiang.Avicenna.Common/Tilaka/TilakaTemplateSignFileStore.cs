using System;
using System.IO;

namespace Temiang.Avicenna.Common.Tilaka
{
    /// <summary>
    /// Menyimpan dan membaca PDF template tanda tangan Quick Sign.
    /// </summary>
    public class TilakaTemplateSignFileStore
    {
        private const long MaxSizeBytes = 20L * 1024 * 1024;

        private readonly string _rootPath;

        public TilakaTemplateSignFileStore(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new InvalidOperationException("TilakaUploadTemplateSignFilePath belum dikonfigurasi.");

            _rootPath = Path.GetFullPath(rootPath);
        }

        public string Save(string paramedicID, string templateNo, byte[] content,
            string contentType, DateTime now)
        {
            if (content == null || content.Length == 0)
                throw new InvalidOperationException("Isi file template kosong.");

            if (content.Length > MaxSizeBytes)
                throw new InvalidOperationException("Ukuran file template melebihi 20 MB.");

            var normalized = (contentType ?? string.Empty).Trim().ToLowerInvariant();
            if (normalized.Contains("pdf") == false && !TilakaCasemixFileReader.IsPdf(content))
                throw new InvalidOperationException("File template harus berupa PDF.");

            if (!TilakaCasemixFileReader.IsPdf(content))
                throw new InvalidOperationException("File template harus berupa PDF.");

            var relative = string.Format("{0}/{1}_{2:yyyyMMddHHmmss}.pdf",
                SafeName(paramedicID), SafeName(templateNo), now);
            var fullPath = ResolveFullPath(relative);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllBytes(fullPath, content);
            return relative;
        }

        public byte[] Load(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;
            var fullPath = ResolveFullPath(relativePath);
            return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null;
        }

        private string ResolveFullPath(string relativePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(
                _rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar)));
            var rootWithSeparator = _rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Path file template tidak valid.");

            return fullPath;
        }

        private static string SafeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "UNKNOWN";
            foreach (var ch in Path.GetInvalidFileNameChars()) value = value.Replace(ch, '_');
            value = value.Replace("..", "_").Trim().Trim('.');
            return string.IsNullOrWhiteSpace(value) ? "UNKNOWN" : value;
        }
    }
}
