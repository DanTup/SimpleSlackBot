using System.Runtime.Serialization;

namespace SimpleSlackBot
{
	[DataContract]
	public class Entity
	{
		[DataMember(Name = "id")]
		public string ID { get; private set; }

		[DataMember(Name = "name")]
		public string Name { get; private set; }
	}
}
