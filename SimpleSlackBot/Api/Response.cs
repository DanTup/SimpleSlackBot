using System.Runtime.Serialization;

namespace SimpleSlackBot.RestApi
{
	[DataContract]
	public abstract class Response
	{
		[DataMember(Name = "ok")]
		public bool OK { get; private set; }
	}
}
