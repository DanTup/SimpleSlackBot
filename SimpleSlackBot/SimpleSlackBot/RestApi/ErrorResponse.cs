using System.Runtime.Serialization;

namespace SimpleSlackBot.RestApi
{
	[DataContract]
	class ErrorResponse : Response
	{
		[DataMember(Name = "error")]
		public string Error { get; private set; }
	}
}
