using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleSlackBot;

namespace TestBot.Handlers
{
	class CountdownHandler : Handler
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
