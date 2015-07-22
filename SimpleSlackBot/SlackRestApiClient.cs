using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SimpleSlackBot.RestApi;

namespace SimpleSlackBot
{
	class SlackRestApi
	{
		readonly Uri endpoint = new Uri("https://slack.com/api/");
		readonly HttpClient http = new HttpClient();
		readonly string token;

		public SlackRestApi(string token)
		{
			this.token = token;

			// Make things faster.
			http.DefaultRequestHeaders.ExpectContinue = false;
		}

		public async Task<TResult> Post<TResult>(string method, IEnumerable<KeyValuePair<string, string>> args = null)
		{
			// Append the auth token to the args (required for all requests).
			var postArgs =
				(args ?? Enumerable.Empty<KeyValuePair<string, string>>())
				.Union(new[] {
					new KeyValuePair<string, string>("token", this.token)
				});

			var postData = new FormUrlEncodedContent(postArgs);
			var httpResponse = await http.PostAsync(new Uri(endpoint, method), postData);

			// Stash the repsonse in a memory stream.
			var json = await httpResponse.Content.ReadAsStringAsync();
			Debug.WriteLine("RCV: " + json);

			// Create serialisers for our error type (to check if we're valid) and the specific type
			// we've been asked to deserialise into.
			var errorResponse = Serialiser.Deserialise<ErrorResponse>(json);

			if (errorResponse.OK)
				return Serialiser.Deserialise<TResult>(json);
			else
				throw new Exception(errorResponse.Error);
		}
	}
}
