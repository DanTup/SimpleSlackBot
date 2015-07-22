using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSlackBot
{
	public abstract class Command
	{
		Bot bot;

		internal void SetBot(Bot bot)
		{
			if (this.bot != null)
				throw new Exception("This command belongs to another :(");

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
			await bot.SendMessage(new MessageEvent(channel, text));
		}
	}
}
