using System;
using System.IO;
using System.IO.Compression;

namespace AkaAwesome.Droid
{
	public class DecompressDroid : IDecompression
	{
		public DecompressDroid ()
		{
		}

		public Stream Decompress(Stream input)
		{
			return new GZipStream (input, CompressionMode.Decompress);
		}
	}
}

