using System;

namespace AkaAwesome
{
	public class QuestionInfo : IEquatable<QuestionInfo>
	{
		public QuestionInfo ()
		{
		}
			
		public int QuestionID {
			get;
			set;
		}

		public string Title {
			get;
			set;
		}

		public DateTime InsertDate {
			get;
			set;
		}

		public bool LoadedFromWeb {
			get;
			set;
		}	

		public string TitleWithLoadFrom {
			get {
				if (LoadedFromWeb)
					return "W - " + this.Title;
				else
					return "D - " + this.Title;
			}
		}

		public bool Equals(QuestionInfo other)
		{
			return this.QuestionID == other.QuestionID;
		}
	}
}

