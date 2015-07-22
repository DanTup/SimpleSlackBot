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
			var token = Environment.GetEnvironmentVariable("SLACK_BOT", EnvironmentVariableTarget.User);

			using (var bot = await Bot.Connect(token))
			{
				// TODO: Commands.
				//bot.RegisterCommand(new EchoCommand());
				//bot.RegisterCommand(new CountdownCommand());

				Console.WriteLine("Press a key to disconnect...");
				Console.ReadKey();
				Console.WriteLine();
			}
		}
	}
}
