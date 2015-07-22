using System.Runtime.Serialization;

namespace SimpleSlackBot
{
	[DataContract]
	public class ErrorResponse : Response
	{
		[DataMember(Name = "error")]
		public string Error { get; private set; }
	}
}
