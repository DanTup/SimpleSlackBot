using System;
using System.Diagnostics;

namespace TestBot
{
	class ConsoleTraceListener : TraceListener
	{
		public override void Write(string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(message);
			Console.ResetColor();
		}

		public override void WriteLine(string message)
		{
			Write(message);
			Write("\r\n");
		}
	}
}
