using System;
using System.Threading.Tasks;
using SimpleSlackBot;

namespace TestBot.Commands
{
	class EchoCommand : Command
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
