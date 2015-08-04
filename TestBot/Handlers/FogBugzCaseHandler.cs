using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimpleSlackBot;

namespace TestBot.Handlers
{
	/// <summary>
	/// Listens for "Case x" in chat and provides links and some details about the FogBugz case.
	/// </summary>
	class FogBugzCaseHandler : Handler
	{
		readonly Uri url;
		readonly string token;

		/// <summary>
		/// Creates a SimpleSlackBot handler that listens for "Case x" in chat and provides links and some details
		/// about the FogBugz case.
		/// </summary>
		/// <param name="url">The base url of the FogBugz installation (eg. https://yourname.fogbugz.com/).</param>
		/// <param name="token">The API token to use when connectingto FogBugz.</param>
		public FogBugzCaseHandler(Uri url, string token)
		{
			this.url = url;
			this.token = token;
		}

		public async override Task OnMessage(Channel channel, User user, string text, bool botIsMentioned)
		{
			// Check the message for a case reference.
			var match = Regex.Match(text, @"(?:fogbugz|fb|case|bug|feature|issue) (\d+)", RegexOptions.IgnoreCase);

			// Extract the case number.
			int caseNumber;
			if (!match.Success || !int.TryParse(match.Groups[1].Captures[0].Value, out caseNumber))
				return;

			// Make it clear we're doing something.
			await SendTypingIndicator(channel);

			// Attempt to fetch the case from the FogBugz API.
			var c = GetCase(caseNumber);
			if (c == null)
				await SendMessage(channel, $"_(Unable to retrieve info from FogBugz for Case {caseNumber})_");
			else
			{
				var att = new Attachment
				{
					Pretext = c.ParentBug != null ? $"Child case of {c.ParentBug}" : null,
					Fallback = $"{caseNumber}: {c.Title}",
					Title = $"{caseNumber}: {c.Title}",
					TitleLink = c.Url.AbsoluteUri,
					Text = c.LatestText,
					AuthorName = c.IsOpen ? c.AssignedTo : null,
					AuthorIcon = new Uri(url, $"default.asp?ixPerson={c.ixAssignedTo}&pg=pgAvatar&pxSize=16").AbsoluteUri,
					Colour = c.ixPriority <= 2 ? "danger" : "warning",
					Fields = new[]
					{
						new Field(c.Status, c.Category),
						new Field($"{c.Project} {c.Area}", "FixFor: " + c.Milestone)
					}
				};

				await SendMessage(channel, null, new[] { att });
			}
		}

		/// <summary>
		/// Fetch case information from the FogBugz API.
		/// </summary>
		Case GetCase(int caseNumber)
		{
			var http = new HttpClient();
			http.DefaultRequestHeaders.ExpectContinue = false;

			// TODO: Error handling.
			var xml = XDocument.Load(new Uri(url, $"api.asp?token={UrlEncode(token)}&cmd=search&q={UrlEncode(caseNumber)}&cols=sTitle,sStatus,ixPersonAssignedTo,sPersonAssignedTo,sLatestTextSummary,sProject,sArea,ixBugOriginal,sFixFor,sCategory,fOpen,ixPriority").AbsoluteUri);

			var caseXml = xml.Descendants("case").SingleOrDefault();

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
				ixPriority = int.Parse(caseXml.Element("ixPriority").Value),
				IsOpen = bool.Parse(caseXml.Element("fOpen").Value),
				Category = caseXml.Element("sCategory").Value,
				Project = caseXml.Element("sProject").Value,
				Area = caseXml.Element("sArea").Value,
				Milestone = caseXml.Element("sFixFor").Value,
				ixAssignedTo = int.Parse(caseXml.Element("ixPersonAssignedTo").Value),
				AssignedTo = caseXml.Element("sPersonAssignedTo").Value,
				LatestText = caseXml.Element("sLatestTextSummary").Value,
			};
		}

		class Case
		{
			public int CaseNumber { get; set; }
			public Uri Url { get; set; }
			public int? ParentBug { get; set; }
			public string Title { get; set; }
			public string Status { get; set; }
			public int ixPriority { get; set; }
			public bool IsOpen { get; set; }
			public string Category { get; set; }
			public string Project { get; set; }
			public string Area { get; set; }
			public string Milestone { get; set; }
			public int ixAssignedTo { get; set; }
			public string AssignedTo { get; set; }
			public string LatestText { get; set; }
		}
	}
}
