using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimpleSlackBot;

namespace TestBot.Handlers
{
	class FogBugzCaseHandler : Handler
	{
		readonly Uri url;
		readonly string token;

		public FogBugzCaseHandler(Uri url, string token)
		{
			this.url = url;
			this.token = token;
		}

		public async override Task OnMessage(Channel channel, User user, string text)
		{
			var match = Regex.Match(text, @"case (\d+)", RegexOptions.IgnoreCase);

			int caseNumber;
			if (!match.Success || !int.TryParse(match.Groups[1].Captures[0].Value, out caseNumber))
				return;

			var c = GetCase(caseNumber);
			if (c == null)
				return;

			var message = FormatMessage(c);

			await SendMessage(channel, message);
		}

		Case GetCase(int caseNumber)
		{
			var http = new HttpClient();
			http.DefaultRequestHeaders.ExpectContinue = false;

			var xml = XDocument.Load(new Uri(url, $"api.asp?token={UrlEncode(token)}&cmd=search&q={UrlEncode(caseNumber)}&cols=sTitle,sStatus,sPersonAssignedTo,sLatestTextSummary,sProject,sArea,ixBugOriginal,sFixFor,sCategory").AbsoluteUri);

			var caseXml = xml.Descendants("case").Single();

			if (caseXml == null)
				return null;

			var parent = int.Parse(caseXml.Element("ixBugOriginal").Value);
			return new Case
			{
				CaseNumber = caseNumber,
				Url = new Uri(url, $"?{caseNumber}"),
				ParentBug = parent != 0 ? parent : (int?)null,
				Title = caseXml.Element("sTitle").Value,
				Status = caseXml.Element("sStatus").Value,
				Category = caseXml.Element("sCategory").Value,
				Project = caseXml.Element("sProject").Value,
				Area = caseXml.Element("sArea").Value,
				Milestone = caseXml.Element("sFixFor").Value,
				AssignedTo = caseXml.Element("sPersonAssignedTo").Value,
				LatestText = caseXml.Element("sLatestTextSummary").Value,
			};
		}

		string FormatMessage(Case c)
		{
			var message = new StringBuilder();
			message.AppendLine($"*<{Escape(c.Url)}|{c.CaseNumber}: {Escape(c.Title)}>*");
			message.AppendLine($"_{Escape(c.Category)}_ | _{Escape(c.Project)}_ | _{Escape(c.Area)}_");
			message.Append($"*Status:* {Escape(c.Status)} | ");
			if (!string.Equals(c.AssignedTo, "closed", StringComparison.OrdinalIgnoreCase)) // TODO: Use ixAssignedTo
				message.Append($"*Assigned:* {Escape(c.AssignedTo)} | ");
			message.AppendLine($"*FixFor:* {Escape(c.Milestone)}");

			if (c.ParentBug != null)
				message.AppendLine($"*Parent:* Case {Escape(c.ParentBug)}");
			message.AppendLine(Escape(c.LatestText));

			return message.ToString();
		}

		class Case
		{
			public int CaseNumber { get; set; }
			public Uri Url { get; set; }
			public int? ParentBug { get; set; }
			public string Title { get; set; }
			public string Status { get; set; }
			public string Category { get; set; }
			public string Project { get; set; }
			public string Area { get; set; }
			public string Milestone { get; set; }
			public string AssignedTo { get; set; }
			public string LatestText { get; set; }
		}
	}
}
