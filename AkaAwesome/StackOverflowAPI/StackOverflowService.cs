﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.IO.Compression;
using System.Text;

using Newtonsoft.Json;
using Connectivity.Plugin;

namespace AkaAwesome
{
	public class StackOverflowService
	{
		public StackOverflowService ()
		{
		}

		private const string QUESTION_URL = "https://api.stackexchange.com/2.2/search?order=desc&sort=activity&tagged=xamarin&site=stackoverflow";
		private const string ANSWER_URL = "https://api.stackexchange.com/2.2/questions/{0}/answers?order=desc&sort=activity&site=stackoverflow&filter=!-04siDnHf2zL";

		public async Task<IList<QuestionInfo>> GetQuestions ()
		{
			if (!CrossConnectivity.Current.IsConnected)
				throw new NoInternetException ();

			var decompressedContent = await this.CallAndDecompress (QUESTION_URL);
			var deserializedContent = JsonConvert.DeserializeObject<QuestionResponse> (decompressedContent);

			var questions = new List<QuestionInfo> ();

			foreach (var item in deserializedContent.items) {
				var qi = new QuestionInfo { QuestionID = item.question_id, Title = item.title, LoadedFromWeb = true };

				questions.Add (qi);
			}

			return questions;

		}

		public async Task<AnswerInfo> GetAnswerForQuestion(int questionId)
		{
			if (!CrossConnectivity.Current.IsConnected)
				throw new NoInternetException ();

			var decompressedContent = await this.CallAndDecompress (string.Format (ANSWER_URL, questionId));
			var deserializedContent = JsonConvert.DeserializeObject<AnswerResponse> (decompressedContent);

			AnswerInfo theAnswer = null;

			if (deserializedContent != null && deserializedContent.items != null && deserializedContent.items.Count > 0) {
				var currAnswer = deserializedContent.items [0];

				theAnswer = new AnswerInfo {
					AnswerID = currAnswer.answer_id,
					QuestionID = currAnswer.question_id,
					AnswerBody = currAnswer.body,
					LoadedFromWeb = true
				};						
			}

			return theAnswer;
		}

		public async Task<IList<AnswerInfo>>GetAnswersForManyQuestions(List<int> questionIds)
		{
			if (!CrossConnectivity.Current.IsConnected)
				throw new NoInternetException();

			StringBuilder semiDelimQuestionIds = new StringBuilder();

			foreach (var item in questionIds) {
				semiDelimQuestionIds.AppendFormat("{0};",item);
			}

			semiDelimQuestionIds.Remove (semiDelimQuestionIds.Length - 1, 1);

			var decompressedContent = await this.CallAndDecompress (string.Format (ANSWER_URL, semiDelimQuestionIds));
			var deserializedContent = JsonConvert.DeserializeObject<AnswerResponse> (decompressedContent);

			List<AnswerInfo> allAnswers = new List<AnswerInfo>();

			if (deserializedContent != null && deserializedContent.items != null && deserializedContent.items.Count > 0) {
				foreach (var currAnswer in deserializedContent.items) {
					allAnswers.Add( new AnswerInfo { 
						AnswerID = currAnswer.answer_id,
						QuestionID = currAnswer.question_id,
						AnswerBody = currAnswer.body
					});
				}
			}

			return allAnswers;
		}

		private async Task<string> CallAndDecompress (string url)
		{
			using (var client = new HttpClient ()) {                
				client.DefaultRequestHeaders.Accept.Clear ();
				client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
				client.DefaultRequestHeaders.AcceptEncoding.Add (new StringWithQualityHeaderValue ("GZIP"));
				client.DefaultRequestHeaders.AcceptEncoding.Add (new StringWithQualityHeaderValue ("DEFLATE"));

				var response = await client.GetAsync (url);

				response.EnsureSuccessStatusCode ();

				string content;

				using (var responseStream = await response.Content.ReadAsStreamAsync ()) {
					var decompressedStream = App.PlatformDecompression.Decompress (responseStream);
					using (var streamReader = new StreamReader (decompressedStream)) {
						content = streamReader.ReadToEnd ();
					}
				}
			
				return content;
			}
		}
	}
}

