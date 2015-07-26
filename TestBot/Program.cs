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
			Debug.Listeners.Add(new ConsoleTraceListener());

			MainAsync(args).Wait();
		}

		static async Task MainAsync(string[] args)
		{
			var token = Environment.GetEnvironmentVariable("SLACK_BOT", EnvironmentVariableTarget.User);
			var fbUrl = Environment.GetEnvironmentVariable("SLACK_BOT_FB_URL", EnvironmentVariableTarget.User);
			var fbToken = Environment.GetEnvironmentVariable("SLACK_BOT_FB_TOKEN", EnvironmentVariableTarget.User);

			using (var bot = await Bot.Connect(token))
			{
				bot.RegisterHandler(new EchoHandler());
				bot.RegisterHandler(new CountdownHandler());
				bot.RegisterHandler(new FogBugzCaseHandler(new Uri(fbUrl), fbToken));

				Console.WriteLine("Press a key to disconnect...");
				Console.ReadKey();
				Console.WriteLine();

				await bot.Disconnect();
			}
		}
	}
}
