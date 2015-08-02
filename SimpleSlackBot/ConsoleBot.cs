using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSlackBot
{
	public class ConsoleBot : Bot
	{
		Channel consoleChannel = new Channel { Name = "Console" };
		User consoleUser = new User { Name = "Console" };
		CancellationTokenSource cancellationSource = new CancellationTokenSource();

		internal override Task SendMessage(Channel channel, string text, Attachment[] attachments)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(text);
			if (attachments != null && attachments.Any())
			{
				Console.WriteLine("+ ATTACHMENTS");
			}
			Console.ResetColor();

			// TODO: .CompletedTask (4.6).
			return Task.FromResult(true);
		}

		internal override Task SendTypingIndicator(Channel channel)
		{
			// TODO: .CompletedTask (4.6).
			return Task.FromResult(true);
		}

		public void HandleInput()
		{
			while (true)
			{
				var text = Console.ReadLine();
				Console.WriteLine();

				HandleRecievedMessage(consoleChannel, consoleUser, text);
			}
		}
	}
}
