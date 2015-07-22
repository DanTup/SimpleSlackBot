using System.Runtime.Serialization;
using SimpleSlackBot.RestApi;

namespace SimpleSlackBot.WebSocketApi
{
	[DataContract]
	public class MessageEvent : Event
	{
		public const string MESSAGE = "message";
		public const string MESSAGE_CHANGED = "message_changed";

		public override string Type { get { return MESSAGE; } }

		[DataMember(Name = "channel")]
		public string ChannelID { get; private set; }

		[DataMember(Name = "user")]
		public string UserID { get; private set; }

		[DataMember(Name = "text")]
		public string Text { get; private set; }

		[DataMember(Name = "message")]
		public MessageEvent Message { get; private set; }

		[DataMember(Name = "hidden")]
		public bool Hidden { get; private set; }

		public MessageEvent(Channel channel, string text)
		{
			this.ChannelID = channel.ID;
			this.Text = text;
		}
	}
}
