using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Reactive.Linq;

using Xamarin.Forms;

using Akavache;

namespace AkaAwesome
{
	public class QuestionListPage : ContentPage
	{
		ListView _questionsListView;
		ObservableCollection<QuestionInfo> _displayQuestions;
		DateTime _dateToDisplay;

		bool _initialDisplay = true;

		public QuestionListPage (DateTime dateToDisplay)
		{
			_dateToDisplay = dateToDisplay;

			Title = "Xamarin Questions";

			_displayQuestions = new ObservableCollection<QuestionInfo> ();

			_questionsListView = new ListView {
				ItemsSource = _displayQuestions,
				RowHeight = 40,
				HasUnevenRows = true
			};

			_questionsListView.ItemTemplate = new DataTemplate (typeof(QuestionCell));

			_questionsListView.ItemTapped += async (object sender, ItemTappedEventArgs e) => {

				var questionInfo = e.Item as QuestionInfo;
				var newPage = new AnswerDetailPage (questionInfo.QuestionID);						
				await Navigation.PushAsync (newPage);
			};
				
			Content = new StackLayout { 
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {					
					_questionsListView
				}
			};
					
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			if (_initialDisplay) {
				LoadQuestions ();
				_initialDisplay = false;	
			}
		}

		protected void LoadQuestions ()
		{			
			BlobCache.LocalMachine.GetAndFetchLatest<IList<QuestionInfo>> (
				_dateToDisplay.Ticks.ToString (),
				async () => await new StackOverflowService ().GetQuestions (_dateToDisplay),
				null,
				_dateToDisplay.AddDays (7)
			).Catch (Observable.Return (new List<QuestionInfo> ())).Subscribe (
				returnedQuestions => {
					Device.BeginInvokeOnMainThread (() => DisplayQuestions (returnedQuestions));
				}
			);			
		}

		private void DisplayQuestions (IList<QuestionInfo> questions)
		{
			foreach (var item in questions) {
				if (!_displayQuestions.Contains (item)) {
					_displayQuestions.Insert (0, item);
				}
			}
		}

		protected virtual async Task HandleException ()
		{
		}
	}
}


