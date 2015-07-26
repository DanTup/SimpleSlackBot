using System.Runtime.Serialization;

namespace SimpleSlackBot
{
	[DataContract]
	public class PostMessageResponse : Response
	{
		[DataMember(Name = "ts")]
		public double Timestamp{ get; private set; }
	}
}
