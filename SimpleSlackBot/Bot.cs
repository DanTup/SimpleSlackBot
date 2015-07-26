using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSlackBot
{
	public class Bot : IDisposable
	{
		readonly SlackRestApi api;
		readonly ClientWebSocket ws = new ClientWebSocket();
		readonly HashSet<Handler> handlers = new HashSet<Handler>();

		readonly Dictionary<string, User> users = new Dictionary<string, User>(); // TODO: Handle new users joining/leaving
		readonly Dictionary<string, Channel> channels = new Dictionary<string, Channel>(); // TODO: Handle new channels/deleted

		readonly string[] cancellationTerms = new[] { "cancel", "abort", "stop" };

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
		Task<PostMessageResponse> PostMessage(string channelID, string text) =>
			api.Post<PostMessageResponse>("chat.postMessage", new Dictionary<string, string> {
				{ "as_user", "true" },
				{ "channel", channelID },
				{ "text", text }
			});

		#endregion

		public async Task Connect()
		{
			if (ws.State == WebSocketState.Connecting || ws.State == WebSocketState.Open)
				throw new InvalidOperationException("Not is already connected");

			// First check we can authenticate.
			var authResponse = await this.AuthTest();
			Debug.WriteLine("Authorised as " + authResponse.User);

			// Issue a request to start a real time messaging session.
			var rtmResponse = await this.RtmStart();

			// Store users and channels so we can look them up by ID.
			foreach (var user in rtmResponse.Users)
				users.Add(user.ID, user);
			foreach (var channel in rtmResponse.Channels.Union(rtmResponse.IMs))
				channels.Add(channel.ID, channel);

			// Connect the WebSocket to the URL we were given back.
			await ws.ConnectAsync(rtmResponse.Url, CancellationToken.None);
			Debug.WriteLine("Connected...");

			// Start the receive message loop.
			var _ = Task.Run(ListenForMessages);

			// Say hello in each of the channels the bot is a member of.
			foreach (var channel in channels.Values.Where(c => !c.IsPrivate && c.IsMember))
				await SendMessage(channel, RandomMessages.Hello());
		}

		public async Task Disconnect()
		{
			// Say goodbye to each of the channels the bot is a member of.
			foreach (var channel in channels.Values.Where(c => !c.IsPrivate && c.IsMember))
				await SendMessage(channel, RandomMessages.Goodbye());
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

				await HandleMessage(fullMessage.ToString());
			}
		}

		public void RegisterHandler(Handler handler)
		{
			handler.SetBot(this);
			handlers.Add(handler);
		}

		#region Message Handling

		internal async Task SendMessage(Channel channel, string text)
		{
			await PostMessage(channel.ID, text);
        }

		async Task HandleMessage(string message)
		{
			Debug.WriteLine("RCV: " + message);

			var eventType = Serialiser.Deserialise<Event>(message).Type;

			switch (eventType)
			{
				case "hello":
				case "presence_change":
				case "user_typing":
				case null: // Acknowledgement of sent message
					break;

				case MessageEvent.MESSAGE:
					await Handle(Serialiser.Deserialise<MessageEvent>(message));
					break;

				default:
					Debug.WriteLine("Unknown message type: " + eventType);
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.WriteLine(message);
					Console.ResetColor();
					break;

			}
		}

		CancellationTokenSource cancellationSource = new CancellationTokenSource();
		async Task Handle(MessageEvent message)
		{
			var channelID = message.Message?.ChannelID ?? message.ChannelID;
			var userID = message.Message?.UserID ?? message.UserID;
			var text = message.Message?.Text ?? message.Text;

			// If the text is cancellation, then send a cancellation message instead.
			if (cancellationTerms.Contains(text, StringComparer.OrdinalIgnoreCase))
			{
				cancellationSource.Cancel();
				cancellationSource = new CancellationTokenSource();
				return;
			}

			foreach (var handler in handlers)
			{
				try
				{
					await handler.OnMessage(channels[channelID], users[userID], text, cancellationSource.Token);
				}
				catch (Exception ex)
				{
					await SendMessage(channels[channelID], ex.ToString());
				}
			}
		}

		#endregion
	}
}
