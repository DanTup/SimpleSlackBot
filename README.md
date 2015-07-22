# Simple Slack Bot

A simple library for creating [Slack bots](https://api.slack.com/bot-users) in C#.

This is being developed for my own use and may or not every be finished/fixed/maintained.

## Installation

TODO: NuGet...

## Usage

Create a [bot integration](https://my.slack.com/services/new/bot) on Slack to get an API Key to connect a bot.

Create a Command that can receive and send messages:

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
	
Connect the bot and register your command:

	using (var bot = await Bot.Connect(token))
	{
		bot.RegisterCommand(new EchoCommand());
		bot.RegisterCommand(new CountdownCommand());

		Console.WriteLine("Press a key to disconnect...");
		Console.ReadKey();
	}
	
## Command Cancellation

It's possible to make your commands being cancelled. An optional `CancellationToken` can be used to signal cancellations when a user sends `abort`, `stop` or `cancel`:

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
