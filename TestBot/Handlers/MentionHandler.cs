using System.Threading.Tasks;
using SimpleSlackBot;

namespace TestBot.Handlers
{
	public class MentionHandler : Handler
	{
		public override async Task OnMessage(Channel channel, User user, string text, bool botIsMentioned)
		{
			if (botIsMentioned)
				await SendMessage(channel, string.Format("<@{0}> Hello!", user.ID));
		}
	}
}
