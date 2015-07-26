using System.Runtime.Serialization;

namespace SimpleSlackBot
{
	[DataContract]
	public class ChannelJoinedEvent : Event
	{
		public const string TYPE = "channel_joined";

		[DataMember(Name = "channel")]
		public Channel Channel { get; private set; }
	}
}
