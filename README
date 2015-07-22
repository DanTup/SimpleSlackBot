# Simple Slack Bot

A simple library for creating [Slack bots](https://api.slack.com/bot-users) in C#.

This is being developed for my own private use and may or not every be finished/fixed/maintained.

## Usage

Create a [bot integration](https://my.slack.com/services/new/bot) to get an API Key to connect a bot.

	using (var bot = await Bot.Connect(token))
	{
		bot.RegisterCommand(new EchoCommand());
		bot.RegisterCommand(new CountdownCommand());

		Console.WriteLine("Press a key to disconnect...");
		Console.ReadKey();
	}