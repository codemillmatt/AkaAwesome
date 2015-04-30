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
				var newPage = new QuestionDetailPage (questionInfo.QuestionID);						
				await Navigation.PushAsync (newPage);
			};
				
			Content = new StackLayout { 
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {					
					_questionsListView
				}
			};
					
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();

			if (_initialDisplay) {
				await LoadQuestions ();
				_initialDisplay = false;	
			}
		}



		protected async Task LoadQuestions ()
		{
			//				questions = await BlobCache.LocalMachine.GetOrFetchObject<IList<QuestionInfo>> (
			//					_dateToDisplay.Ticks.ToString(),
			//					async () => await overflowAPI.GetQuestions(_dateToDisplay),
			//					_dateToDisplay.AddDays(7)
			//				);

			_displayQuestions.Clear ();

			bool loadQuestionsFromWeb = false;

			IList<QuestionInfo> questions = new List<QuestionInfo>();

			try
			{				
				// 1 - try to load from the cache
				questions = await BlobCache.LocalMachine.GetObject<IList<QuestionInfo>> (_dateToDisplay.Ticks.ToString ());

				loadQuestionsFromWeb = questions.Count == 0;

			} catch (KeyNotFoundException) {
				loadQuestionsFromWeb = true;
			}

			// 2 - if nothing there, load from the web
			if (loadQuestionsFromWeb) {
				try {				
					var overflowAPI = new StackOverflowService ();
					questions = await overflowAPI.GetQuestions (_dateToDisplay);

					// 3 - save records to the cache
					await BlobCache.LocalMachine.InsertObject<IList<QuestionInfo>>(
						_dateToDisplay.Ticks.ToString(),
						questions,
						_dateToDisplay.AddDays(7)
					);
												
				} catch (NoInternetException) {
					await HandleException ();
				}
			}

			foreach (var item in questions) {					
				_displayQuestions.Insert (0, item);
			}
		}

		protected virtual async Task HandleException ()
		{
		}
	}
}


