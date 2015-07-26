using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSlackBot
{
	public abstract class Handler
	{
		Bot bot;

		internal void SetBot(Bot bot)
		{
			if (this.bot != null)
				throw new Exception("This handler belongs to another :(");

			this.bot = bot;
		}

		public virtual Task OnMessage(Channel channel, User user, string text)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnMessage(Channel channel, User user, string text, CancellationToken cancellationToken)
		{
			return OnMessage(channel, user, text);
		}

		protected async Task SendMessage(Channel channel, string text)
		{
			await bot.SendMessage(channel, text);
		}

		protected async Task SendTypingIndicator(Channel channel)
		{
			await bot.SendTypingIndicator(channel);
		}

		/// <summary>
		/// Escapes message text for Slack messages.
		/// </summary>
		protected string Escape(object input)
		{
			// https://api.slack.com/docs/formatting
			return (input?.ToString() ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace("&", "&gt;");
        }

		protected string UrlEncode(object input)
		{
			return WebUtility.UrlEncode(input?.ToString() ?? "");
		}
	}
}
