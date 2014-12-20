using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Octokit;

namespace IssuesImporter.Test
{
    [TestFixture]
    public class WhenWritingGithubIssues
    {
        private GoogleIssuesDataFileReader _reader;
        private const string InputDataFile = @"TestData\TestInputDataFile.csv";

        private GitHubClient _client;
        private Credentials _basicAuth;
        private IIssuesClient _issuesClient;
        private Repository _repository;

        [Test]
        public async void CanAddIssueToGitHub()
        {
            _client = new GitHubClient(new ProductHeaderValue("sbohlen-test-2"));
            _basicAuth = new Credentials("af152b92524012fbd5c79230fac6b4570f6451e1");

            _client.Credentials = _basicAuth;
            _issuesClient = _client.Issue;


            _reader = new GoogleIssuesDataFileReader(InputDataFile);

            var googleIssues = _reader.GetIssues();

            var gitHubIssues = new List<NewIssue>();

            //first step, create the issues...
            foreach (var googleIssue in googleIssues)
            {
                var newIssue = new NewIssue(googleIssue.Summary);
                newIssue.Body =
                    string.Format(
                    "This issue ported from the deprecated GoogleCode site (https://code.google.com/p/ndbunit)." +
                    "\n\nSee https://code.google.com/p/ndbunit/issues/detail?id={0} for more of the history and details for this issue.",
                    googleIssue.Id);

                gitHubIssues.Add(newIssue);
            }

            //next step, add them to the github instance ...
            foreach (var gitHubIssue in gitHubIssues)
            {
                await _issuesClient.Create("NDbUnit", "test-issues-import", gitHubIssue);
            }
        }
    }
}
