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

			var msg = await SendMessage(channel, "Please wait...");

			await Task.Delay(3000);

			await UpdateMessage(msg, text.Substring(prefix.Length));
		}
	}
}
