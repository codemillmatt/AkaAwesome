using System;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Akavache;

using Xamarin.Forms;

namespace AkaAwesome
{
	public class AnswerDetailPage : ContentPage
	{
		WebView _theFullAnswer;
		AnswerInfo _theAnswer;
		int _questionId;

		public AnswerDetailPage (int questionId)
		{									
			_theAnswer = new AnswerInfo ();
			_questionId = questionId;

			Title = "Possible Answer";

			HtmlWebViewSource webSource = new HtmlWebViewSource ();

			webSource.BindingContext = _theAnswer;
			webSource.SetBinding (HtmlWebViewSource.HtmlProperty, new Binding("AnswerBody"));

			_theFullAnswer = new WebView { 
				Source = webSource,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			Content = new StackLayout { 
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					_theFullAnswer
				}
			};
							
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();

			await LoadAnswers (_questionId);
		}

		protected async Task LoadAnswers (int questionId)
		{	
			AnswerInfo currentAnswer = await BlobCache.LocalMachine.GetOrFetchObject<AnswerInfo>(
				_questionId.ToString(),
				async() => await new StackOverflowService().GetAnswerForQuestion(questionId),
				DateTime.Now.AddDays(1)
			);
				
			if (currentAnswer != null) {
				_theAnswer.AnswerID = currentAnswer.AnswerID;
				_theAnswer.QuestionID = currentAnswer.QuestionID;
				_theAnswer.AnswerBody = currentAnswer.AnswerBody;
			} else {
				// Nothing found on the web or in the cache - so invalidate the cache
				await BlobCache.LocalMachine.InvalidateObject<AnswerInfo> (_questionId.ToString ());

				_theAnswer.AnswerID = 0;
				_theAnswer.QuestionID = 0;
				_theAnswer.AnswerBody = "No answer found on StackOverflow or in cache";
			}				
		}

	}
}


