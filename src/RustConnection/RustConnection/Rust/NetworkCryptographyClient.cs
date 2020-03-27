using System;
using System.IO;
using Network;

namespace RustConnection.Rust
{
    public class NetworkCryptographyClient : NetworkCryptography
    {
        public static UInt32 EncryptionSeed = 0;
        
        protected override void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
        {
            Craptography.XOR(EncryptionSeed, src, srcOffset, dst, dstOffset);
            return;
        }

        protected override void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
        {
            Craptography.XOR(EncryptionSeed, src, srcOffset, dst, dstOffset);
            return;
        }
    }
}