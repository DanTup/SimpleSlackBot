using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleSlackBot;
using SimpleSlackBot.RestApi;

namespace TestBot.Commands
{
	class CountdownCommand : Command
	{
		public override async Task OnMessage(Channel channel, User user, string text, CancellationToken cancellationToken)
		{
			if (!string.Equals(text, "countdown", StringComparison.OrdinalIgnoreCase))
				return;

			for (var i = 10; i > 0 && !cancellationToken.IsCancellationRequested; i--)
			{
				await SendMessage(channel, $"{i}...");
				await Task.Delay(1000);
			}

			if (!cancellationToken.IsCancellationRequested)
				await SendMessage(channel, "Thunderbirds are go!");
			else
				await SendMessage(channel, "Countdown was aborted. Thunderbirds are cancelled, kids :(");
		}
	}
}
