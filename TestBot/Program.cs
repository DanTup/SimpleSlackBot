using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SimpleSlackBot;
using TestBot.Handlers;

namespace TestBot
{
	class Program
	{
		static void Main(string[] args)
		{
			/* REPLACE THESE VALUES WITH YOUR OWN */
			var useSlackBot = false; // Set to true to use real Slack bot, rather than console.
            var slackToken = Environment.GetEnvironmentVariable("SLACK_BOT", EnvironmentVariableTarget.User); // Slack bot access token.
			var fbUrl = Environment.GetEnvironmentVariable("SLACK_BOT_FB_URL", EnvironmentVariableTarget.User); // FogBugz installation url.
			var fbToken = Environment.GetEnvironmentVariable("SLACK_BOT_FB_TOKEN", EnvironmentVariableTarget.User); // FogBugz access token.

			var handlers = new Handler[]
			{
				new EchoHandler(),
				new CountdownHandler(),
				new FogBugzCaseHandler(new Uri(fbUrl), fbToken),
				new SlowEchoHandler(),
			};

			if (useSlackBot)
				RunSlackBotAsync(slackToken, handlers).Wait();
			else
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Using ConsoleBot by default. Please edit the values at the top of Program.cs to connect to a real Slack instance.");
				Console.WriteLine("Type 'quit' to quit.");
				Console.ResetColor();
				Console.WriteLine();
				RunConsoleBot(handlers);
			}
		}

		static void RunConsoleBot(Handler[] handlers)
		{
			var bot = new ConsoleBot();

			foreach (var handler in handlers)
				bot.RegisterHandler(handler);

			bot.HandleInput();
		}

		static async Task RunSlackBotAsync(string token, Handler[] handlers)
		{
			Debug.Listeners.Add(new ConsoleTraceListener());
			
			using (var bot = await SlackBot.Connect(token))
			{
				foreach (var handler in handlers)
					bot.RegisterHandler(handler);

				Console.WriteLine("Press a key to disconnect...");
				Console.ReadKey();
				Console.WriteLine();

				await bot.Disconnect();
			}
		}
	}
}
