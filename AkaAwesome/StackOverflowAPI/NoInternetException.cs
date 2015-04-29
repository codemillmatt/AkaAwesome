using System;

namespace AkaAwesome
{
	public class NoInternetException : Exception
	{
		public NoInternetException () : base ("Internet not reachable")
		{
		}
	}
}

