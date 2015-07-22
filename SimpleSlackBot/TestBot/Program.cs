using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SimpleSlackBot;

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
			// Grab a token we can use for testing from outside the repo, to avoid accidentally committing!
			var token = Environment.GetEnvironmentVariable("SLACK_BOT", EnvironmentVariableTarget.User);

			var bot = new Bot(token);

			// TODO: Commands.
			//bot.RegisterCommand(new EchoCommand());
			//bot.RegisterCommand(new CountdownCommand());

			Console.WriteLine("Press a key to disconnect...");
			Console.ReadKey();
			Console.WriteLine();
		}
	}
}
