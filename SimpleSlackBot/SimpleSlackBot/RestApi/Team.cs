﻿using System.Runtime.Serialization;

namespace SimpleSlackBot.RestApi
{
	[DataContract]
	public class Team : Entity
	{
		[DataMember(Name = "email_domain")]
		public string EmailDomain { get; private set; }
	}
}
