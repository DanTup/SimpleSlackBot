using System.Runtime.Serialization;

namespace SimpleSlackBot
{
	[DataContract]
	public class Event
	{
		[DataMember(Name = "type")]
		public virtual string Type { get; protected set; }

		[DataMember(Name = "subtype")]
		public string SubType { get; protected set; }
	}
}
