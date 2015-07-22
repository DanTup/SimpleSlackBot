using System.Runtime.Serialization;

namespace SimpleSlackBot.RestApi
{
	[DataContract]
	public class Channel : Entity
	{
		[DataMember(Name = "is_im")]
		public bool IsPrivate { get; private set; }

		[DataMember(Name = "is_member")]
		public bool IsMember { get; private set; }
	}
}
