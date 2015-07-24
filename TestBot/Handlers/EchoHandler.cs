using System;
using System.Threading.Tasks;
using SimpleSlackBot;

namespace TestBot.Handlers
{
	class EchoHandler : Handler
	{
		const string prefix = "echo ";

		public override async Task OnMessage(Channel channel, User user, string text)
		{
			if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				return;

			await SendMessage(channel, text.Substring(prefix.Length));
		}
	}
}
