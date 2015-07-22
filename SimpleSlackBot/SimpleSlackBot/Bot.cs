using System;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using SimpleSlackBot.RestApi;

namespace SimpleSlackBot
{
	public class Bot : IDisposable
	{
		readonly SlackRestApi api;
		readonly ClientWebSocket ws = new ClientWebSocket();
		
		#region Construction

		private Bot(string token)
		{
			api = new SlackRestApi(token);
		}

		public static async Task<Bot> Connect(string apiToken)
		{
			// Can't do async constructors, so do connection here. This makes it easy to tie the lifetime of the
			// websocket to this class.
			var bot = new Bot(apiToken);
			await bot.Connect();
			return bot;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				ws.Dispose();
		}

		#endregion

		#region REST Methods

		Task<AuthTestResponse> AuthTest() => api.Post<AuthTestResponse>("auth.test");
		Task<RtmStartResponse> RtmStart() => api.Post<RtmStartResponse>("rtm.start");

		#endregion

		public async Task Connect()
		{
			if (ws.State == WebSocketState.Connecting || ws.State == WebSocketState.Open)
				throw new InvalidOperationException("Bot is already connected");

			// First check we can authenticate.
			var authResponse = await this.AuthTest();
			Debug.WriteLine("Authorised as " + authResponse.User);

			// Issue a request to start a real time messaging session.
			var rtmResponse = await this.RtmStart();

			// Connect the WebSocket to the URL we were given back.
			await ws.ConnectAsync(rtmResponse.Url, CancellationToken.None);
			Debug.WriteLine("Connected...");

			// Start the receive message loop.
			var _ = Task.Run(ListenForMessages);
		}

		async Task ListenForMessages()
		{
			var buffer = new byte[1024];
			var segment = new ArraySegment<byte>(buffer);
			while (ws.State == WebSocketState.Open)
			{
				var fullMessage = new StringBuilder();

				while (true)
				{
					var msg = await ws.ReceiveAsync(segment, CancellationToken.None);

					fullMessage.Append(Encoding.UTF8.GetString(buffer, 0, msg.Count));
					if (msg.EndOfMessage)
						break;
				}

				HandleMessage(fullMessage.ToString());
			}
		}
		
		#region Message Handling

		internal async Task SendMessage<TEvent>(TEvent message)
		{
			var json = Serialiser.Serialise(message);
			Debug.WriteLine("SEND: " + json);
			await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, CancellationToken.None);
		}

		void HandleMessage(string message)
		{
			Debug.WriteLine("RCV: " + message);			
		}

		#endregion
	}
}
