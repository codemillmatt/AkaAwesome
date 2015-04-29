using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace AkaAwesome
{
	public class QuestionListPage : ContentPage
	{
		ListView _questionsListView;
		ObservableCollection<QuestionInfo> _displayQuestions;

		bool _initialDisplay = true;

		public QuestionListPage ()
		{
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
			_displayQuestions.Clear ();

			try {
				var overflowAPI = new StackOverflowService();
				var questions = await overflowAPI.GetQuestions();

				foreach (var item in questions) {
					_displayQuestions.Insert(0, item);
				}

			} catch (NoInternetException) {
				await HandleException ();
			}
		}

		protected virtual async Task HandleException ()
		{
		}
	}
}


