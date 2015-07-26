using System;
using System.Threading.Tasks;
using SimpleSlackBot;

namespace TestBot.Handlers
{
	class SlowEchoHandler : Handler
	{
		const string prefix = "slow echo ";

		public override async Task OnMessage(Channel channel, User user, string text)
		{
			if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				return;

			await SendTypingIndicator(channel);

			await Task.Delay(3000);

			await SendMessage(channel, text.Substring(prefix.Length));
		}
	}
}
