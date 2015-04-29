using System;
using System.IO;

namespace AkaAwesome
{
	public interface IDecompression
	{
		Stream Decompress(Stream input);
	}
}

