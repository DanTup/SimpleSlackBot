using System.Runtime.Serialization;

namespace SimpleSlackBot.RestApi
{
	[DataContract]
	abstract class Response
	{
		[DataMember(Name = "ok")]
		public bool OK { get; private set; }
	}
}
