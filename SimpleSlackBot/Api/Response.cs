using System.Runtime.Serialization;

namespace SimpleSlackBot
{
	[DataContract]
	public abstract class Response
	{
		[DataMember(Name = "ok")]
		public bool OK { get; private set; }
	}
}
