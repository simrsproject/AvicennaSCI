using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaKtpFileStore : ITilakaKtpFileStore
    {
        private const int MaxDimension = 1600;

        private readonly string _rootPath;
        private readonly int _maxSizeKb;

        public TilakaKtpFileStore(string rootPath, int maxSizeKb)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new InvalidOperationException("TilakaUploadKTPImage belum dikonfigurasi.");

            _rootPath = Path.GetFullPath(rootPath);
            _maxSizeKb = maxSizeKb <= 0 ? 1024 : maxSizeKb;
        }

        public string Save(string relatedId, string ssn, byte[] content, string contentType, DateTime now)
        {
            if (content == null || content.Length == 0)
                throw new InvalidOperationException("Isi file KTP kosong.");

            var normalized = (contentType ?? string.Empty).Trim().ToLowerInvariant();
            if (normalized.Contains("pdf") || normalized.Contains("octet")
                || !normalized.StartsWith("image/", StringComparison.Ordinal))
                throw new InvalidOperationException("Foto KTP harus berupa gambar JPEG atau PNG.");

            using (var input = new MemoryStream(content))
            using (var original = ReadImage(input))
            using (var resized = Resize(original))
            using (var output = new MemoryStream())
            {
                resized.Save(output, ImageFormat.Jpeg);
                var bytes = output.ToArray();
                if (bytes.Length > _maxSizeKb * 1024)
                    throw new InvalidOperationException(
                        string.Format("Ukuran foto KTP melebihi {0} KB setelah kompresi.", _maxSizeKb));

                var relative = string.Format("{0}/{1}_{2:yyyyMMddHHmmss}.jpg",
                    SafeName(relatedId), SafeName(ssn), now);
                var fullPath = ResolveFullPath(relative);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                File.WriteAllBytes(fullPath, bytes);
                return relative;
            }
        }

        public byte[] Load(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;
            var fullPath = ResolveFullPath(relativePath);
            return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null;
        }

        private static Image ReadImage(Stream stream)
        {
            try
            {
                return Image.FromStream(stream);
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException("Isi file KTP bukan gambar yang valid.");
            }
        }

        private static Image Resize(Image original)
        {
            var ratio = Math.Min(1d, (double)MaxDimension / Math.Max(original.Width, original.Height));
            var width = Math.Max(1, (int)(original.Width * ratio));
            var height = Math.Max(1, (int)(original.Height * ratio));

            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(original, 0, 0, width, height);
            }

            return bitmap;
        }

        private string ResolveFullPath(string relativePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(
                _rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar)));
            var rootWithSeparator = _rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Path foto KTP tidak valid.");

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
