using System;

namespace SimpleSlackBot
{
	static class RandomMessages
	{
		static Random rnd = new Random();
		static bool Choose() => rnd.Next(2) == 1;
        static string RandomOf(params string[] items)
		{
			var msg = items[rnd.Next(items.Length)];

			if (Choose())
				msg = msg.ToLower();

			if (Choose())
				msg += "!";

			return msg;
		}

		public static string Hello() => RandomOf("Hello", "I'm back!", "Hi", "'sup?", "Yo");

		public static string Goodbye() => RandomOf("Goodbye", "Bye", "I'll be back", "Farewell", "So long");
	}
}
