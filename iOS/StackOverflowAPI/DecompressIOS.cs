using System;
using System.IO;
using System.IO.Compression;

namespace AkaAwesome.iOS
{
	public class DecompressIOS : IDecompression
	{
		public DecompressIOS ()
		{
		}

		public Stream Decompress(Stream input)
		{
			return new GZipStream (input, CompressionMode.Decompress);
		}
	}
}

