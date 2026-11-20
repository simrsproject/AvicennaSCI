using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaKtpFileStore
    {
        string Save(string relatedId, byte[] content, string contentType, DateTime now);
        byte[] Load(string relativePath);
    }
}
