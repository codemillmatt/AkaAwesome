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

		public int UnixCreationDate {
			get;
			set;
		}

		public bool IsAnswered {
			get;
			set;
		}

		public string TitleWithLoadFrom {
			get {
				return this.Title + " " + this.InsertDate.ToString ("s");
			}
		}

		public bool Equals(QuestionInfo other)
		{
			return this.QuestionID == other.QuestionID;
		}
	}
}

