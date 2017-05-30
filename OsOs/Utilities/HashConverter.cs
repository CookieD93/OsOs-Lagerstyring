using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace OsOs.Utilities
{
    class HashConverter
    {
        private IBuffer Buffer;
        private IBuffer Hasher;
        private HashAlgorithmProvider Provider;
        public string ConvertData(string data)
        {
            if (data == string.Empty)
                return "";

            Buffer = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);
            Provider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            Hasher = Provider.HashData(Buffer);
            return CryptographicBuffer.EncodeToBase64String(Hasher);
        }
    }
}
