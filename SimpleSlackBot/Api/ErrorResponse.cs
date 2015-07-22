using System.Runtime.Serialization;

namespace SimpleSlackBot.RestApi
{
	[DataContract]
	public class ErrorResponse : Response
	{
		[DataMember(Name = "error")]
		public string Error { get; private set; }
	}
}
