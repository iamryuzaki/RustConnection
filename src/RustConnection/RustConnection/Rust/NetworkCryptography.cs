using System.IO;
using Network;

namespace RustConnection.Rust
 {
	 public abstract class NetworkCryptography : INetworkCryptocraphy
	 {
		 public MemoryStream EncryptCopy(Connection connection, MemoryStream stream, int offset)
		 {
			 this.buffer.Position = 0L;
			 this.buffer.SetLength(0L);
			 this.buffer.Write(stream.GetBuffer(), 0, offset);
			 this.EncryptionHandler(connection, stream, offset, this.buffer, offset);
			 return this.buffer;
		 }

		 public MemoryStream DecryptCopy(Connection connection, MemoryStream stream, int offset)
		 {
			 this.buffer.Position = 0L;
			 this.buffer.SetLength(0L);
			 this.buffer.Write(stream.GetBuffer(), 0, offset);
			 this.DecryptionHandler(connection, stream, offset, this.buffer, offset);
			 return this.buffer;
		 }

		 public void Encrypt(Connection connection, MemoryStream stream, int offset)
		 {
			 this.EncryptionHandler(connection, stream, offset, stream, offset);
		 }

		 public void Decrypt(Connection connection, MemoryStream stream, int offset)
		 {
			 this.DecryptionHandler(connection, stream, offset, stream, offset);
		 }

		 public bool IsEnabledIncoming(Connection connection)
		 {
			 return connection != null && connection.encryptionLevel > 0u && connection.decryptIncoming;
		 }

		 public bool IsEnabledOutgoing(Connection connection)
		 {
			 return connection != null && connection.encryptionLevel > 0u && connection.encryptOutgoing;
		 }

		 protected abstract void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);

		 protected abstract void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);

		 private MemoryStream buffer = new MemoryStream();
	 }
 }