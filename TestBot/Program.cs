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


			var fbUrl = Environment.GetEnvironmentVariable("SLACK_BOT_FB_URL", EnvironmentVariableTarget.User);
			var fbToken = Environment.GetEnvironmentVariable("SLACK_BOT_FB_TOKEN", EnvironmentVariableTarget.User);

			var handlers = new Handler[]
			{
				new EchoHandler(),
				new CountdownHandler(),
				new FogBugzCaseHandler(new Uri(fbUrl), fbToken),
				new SlowEchoHandler(),
			};

			RunSlackBotAsync(handlers).Wait();
			//RunConsoleBot(handlers);
		}

		//static void RunConsoleBot(Handler[] handlers)
		//{
		//	var bot = new ConsoleBot();

		//	foreach (var handler in handlers)
		//		bot.RegisterHandler(handler);

		//	bot.HandleInput();
		//}

		static async Task RunSlackBotAsync(Handler[] handlers)
		{
			Debug.Listeners.Add(new ConsoleTraceListener());
			var token = Environment.GetEnvironmentVariable("SLACK_BOT", EnvironmentVariableTarget.User);

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
