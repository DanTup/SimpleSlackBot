using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			// TODO: Connect bot...
			Debug.WriteLine("Test...");

			await Task.Delay(5000);

			Console.WriteLine("Press a key to disconnect...");
			Console.ReadKey();
			Console.WriteLine();
		}
	}
}
