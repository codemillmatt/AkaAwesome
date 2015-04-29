using System;

using Xamarin.Forms;

namespace AkaAwesome
{
	public class App : Application
	{
		public static IDecompression PlatformDecompression {
			get;
			set;
		}
				
		public App ()
		{
			MainPage = new NavigationPage (new QuestionListPage ());
		}
			
	}
}

