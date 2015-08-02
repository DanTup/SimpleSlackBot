using System.Runtime.Serialization;

namespace SimpleSlackBot
{
	[DataContract]
	public class Entity
	{
		[DataMember(Name = "id")]
		public string ID { get; internal set; }

		[DataMember(Name = "name")]
		public string Name { get; internal set; }
	}
}
