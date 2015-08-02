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
				foreach (var attachment in attachments)
				{
					if (!string.IsNullOrEmpty(attachment.Pretext))
						Console.WriteLine(attachment.Pretext);
					if (!string.IsNullOrEmpty(attachment.AuthorName))
						Console.WriteLine(attachment.AuthorName);
					Console.WriteLine($"{attachment.Title} <{attachment.TitleLink}>");
					if (!string.IsNullOrEmpty(attachment.Text))
						Console.WriteLine(attachment.Text);
					foreach (var field in attachment.Fields)
						Console.WriteLine($"{field.Title}: {field.Value}");
					Console.WriteLine();
				}
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
			SayHello(consoleChannel).Wait();

			while (true)
			{
				var text = Console.ReadLine();
				Console.WriteLine();

				if (string.Equals(text, "quit"))
				{
					SayGoodbye(consoleChannel).Wait();
					break;
				}

				HandleRecievedMessage(consoleChannel, consoleUser, text);
			}
		}
	}
}
