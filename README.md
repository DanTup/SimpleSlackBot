NOTE: This software is not being actively developed or maintained.

# Simple Slack Bot

A simple library for creating [Slack bots](https://api.slack.com/bot-users) in C#.

This is being developed for my own use and may or not every be finished/fixed/maintained.

## Installation

Install from [NuGet](https://www.nuget.org/packages/SimpleSlackBot/):

```powershell
PM> Install-Package SimpleSlackBot
```

## Usage

Create a [bot integration](https://my.slack.com/services/new/bot) on Slack to get an API Key to connect a bot.

Create a Handle that can receive and send messages:

```csharp
class EchoHandler : Handler
{
	const string prefix = "echo ";

	public override async Task OnMessage(Channel channel, User user, string text, bool botIsMentioned)
	{
		if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			return;

		await SendMessage(channel, text.Substring(prefix.Length));
	}
}
```
	
Connect the bot and register your handler:

```csharp
using (var bot = await SlackBot.Connect(token))
{
	bot.RegisterHandler(new EchoHandler());
	bot.RegisterHandler(new CountdownHandler());

	Console.WriteLine("Press a key to disconnect...");
	Console.ReadKey();
}
```
	
## Command Cancellation

It's possible to make your handler support having commands cancelled. An optional `CancellationToken` can be used to signal cancellations when a user sends `abort`, `stop` or `cancel`:

```csharp
class CountdownHandler : Handler
{
	public override async Task OnMessage(Channel channel, User user, string text, bool botIsMentioned, CancellationToken cancellationToken)
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
```

## Testing Bots without Slack

You can use the ConsoleBot to test your handlers without needing to go via Slack.

	var bot = new ConsoleBot();

	bot.RegisterHandler(new EchoHandler());
	bot.RegisterHandler(new CountdownHandler());

	bot.HandleInput();
	
Upon calling `HandleInput`, the bot will block, consuming input from the console (via `Console.ReadLine`). The test project uses this version of the bot by default. You should change the `useSlackBot` boolean in `Program.cs` to switch to the real `SlackBot`.

## Sample Bot Handlers

There are a couple of sample bot handlers in the TestBot project that might help illustrate how to build things.

- [Echo Handler](https://github.com/DanTup/SimpleSlackBot/blob/master/TestBot/Handlers/EchoHandler.cs)
- [Slow Echo Handler (shows editing messages)](https://github.com/DanTup/SimpleSlackBot/blob/master/TestBot/Handlers/CountdownHandler.cs)
- [Countdown Handler (shows 'typing...')](https://github.com/DanTup/SimpleSlackBot/blob/master/TestBot/Handlers/SlowEchoHandler.cs)
- [FogBugz Case Handler](https://github.com/DanTup/SimpleSlackBot/blob/master/TestBot/Handlers/FogBugzCaseHandler.cs)
- [Mention Handler (responds only when mentioned)](https://github.com/DanTup/SimpleSlackBot/blob/master/TestBot/Handlers/MentionHandler.cs)
