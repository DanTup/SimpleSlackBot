using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSlackBot
{
	public abstract class Bot
	{
		readonly HashSet<Handler> handlers = new HashSet<Handler>();
		readonly ConcurrentBag<Task> handlerTasks = new ConcurrentBag<Task>();
		CancellationTokenSource cancellationSource = new CancellationTokenSource();
		readonly string[] cancellationTerms = new[] { "cancel", "abort", "stop" };
		
		/// <summary>
		/// Registers a handler with the bot.
		/// </summary>
		public void RegisterHandler(Handler handler)
		{
			handler.SetBot(this);
			handlers.Add(handler);
		}

		/// <summary>
		/// Used by the bot to send a message.
		/// </summary>
		abstract internal Task SendMessage(Channel channel, string text, Attachment[] attachments = null);

		/// <summary>
		/// Used by the bot to send a typing notification.
		/// </summary>
		abstract internal Task SendTypingIndicator(Channel channel);

		/// <summary>
		/// Called by the bot implementation when a message is received, to be passed to handlers.
		/// </summary>
		protected void HandleRecievedMessage(Channel channel, User user, string text)
		{
			// If the text is cancellation, then send a cancellation message instead.
			if (cancellationTerms.Contains(text, StringComparer.OrdinalIgnoreCase))
			{
				cancellationSource.Cancel();
				cancellationSource = new CancellationTokenSource();
				return;
			}

			foreach (var handler in handlers)
				handlerTasks.Add(SendMessageToHandlerAsync(channel, user, text, handler));
		}

		/// <summary>
		/// Used privately to pass a message on to each handler.
		/// </summary>
		async Task SendMessageToHandlerAsync(Channel channel, User user, string text, Handler handler)
		{
			try
			{
				await handler.OnMessage(channel, user, text, cancellationSource.Token);
			}
			catch (Exception ex)
			{
				await SendMessage(channel, ex.ToString());
			}
		}

		/// <summary>
		/// Called by the bot implementation when it should cancel all tasks (eg. disconnecting).
		/// </summary>
		protected async Task CancelAllTasks()
		{
			cancellationSource.Cancel();

			// Wait for all tasks to finish cancelling.
			await Task.WhenAll(handlerTasks);
		}

		protected async Task SayHello(Channel channel)
		{
			await SendMessage(channel, RandomMessages.Hello());
		}

		protected async Task SayGoodbye(Channel channel)
		{
			await SendMessage(channel, RandomMessages.Goodbye());
		}
	}
}
