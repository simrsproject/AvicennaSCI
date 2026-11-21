using System;
using System.IO;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaCasemixFileReader
    {
        byte[] Load(string filePath);
    }

    /// <summary>
    /// Membaca PDF Casemix dari lokasi yang sudah disediakan pihak lain (read-only).
    /// </summary>
    public class TilakaCasemixFileReader : ITilakaCasemixFileReader
    {
        private const long MaxSizeBytes = 50L * 1024 * 1024;

        private readonly string _rootPath;

        public TilakaCasemixFileReader(string rootPath)
        {
            _rootPath = string.IsNullOrWhiteSpace(rootPath)
                ? string.Empty
                : Path.GetFullPath(rootPath);
        }

        public byte[] Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("FilePath dokumen Casemix kosong.");

            var fullPath = Path.IsPathRooted(filePath)
                ? filePath
                : (string.IsNullOrEmpty(_rootPath)
                    ? null
                    : Path.Combine(_rootPath, filePath));

            if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
                throw new InvalidOperationException("File Casemix tidak ditemukan: " + filePath);

            var info = new FileInfo(fullPath);
            if (info.Length > MaxSizeBytes)
                throw new InvalidOperationException("Ukuran file Casemix melebihi 50 MB.");

            var content = File.ReadAllBytes(fullPath);
            if (!IsPdf(content))
                throw new InvalidOperationException("File Casemix harus berupa PDF.");

            return content;
        }

        public static bool IsPdf(byte[] content)
        {
            if (content == null || content.Length < 5) return false;
            return content[0] == 0x25 // %
                   && content[1] == 0x50 // P
                   && content[2] == 0x44 // D
                   && content[3] == 0x46 // F
                   && content[4] == 0x2D; // -
        }
    }
}
