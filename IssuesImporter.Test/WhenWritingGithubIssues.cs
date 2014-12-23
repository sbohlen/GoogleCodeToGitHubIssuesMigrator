using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        private const string AccessToken = "03f37ffab000ed46dfa912a554f03d2cc8bf2d67";
        private const string GitHubRepositoryOwner = "NDbUnit";
        private const string GitHubRepositoryName = "NDbUnit";

        private GitHubClient _client;
        private Credentials _basicAuth;
        private IIssuesClient _issuesClient;

        [Test]
        public async void CanAddIssueToGitHub()
        {
            _client = new GitHubClient(new ProductHeaderValue("google%20code%20to%20github%20issue%20migrator"));
            _basicAuth = new Credentials(AccessToken);

            _client.Credentials = _basicAuth;
            _issuesClient = _client.Issue;

            _reader = new GoogleIssuesDataFileReader(InputDataFile);

            var googleIssues = _reader.GetIssues().ToList();

            var gitHubIssuesToCreate = new List<NewIssue>();

            //first step, create the issues...
            foreach (var googleIssue in googleIssues)
            {
                var newIssue = new NewIssue(googleIssue.Summary);
                newIssue.Body =
                    string.Format(
                    "This issue ported from the deprecated GoogleCode NDbUnit site (https://code.google.com/p/ndbunit)." +
                    "\n\nSee https://code.google.com/p/ndbunit/issues/detail?id={0} for more history and details for this specific issue.",
                    googleIssue.Id);

                gitHubIssuesToCreate.Add(newIssue);
            }

            var gitHubIssuesCreated = new List<Issue>();

            var issueCounter = 1.0d;


            //next step, add them to the github instance ...
            foreach (var gitHubIssue in gitHubIssuesToCreate)
            {

                if (Math.Abs(issueCounter % 20.0) < 0.1)
                {
                    Debug.WriteLine("Sleeping for 20s...");
                    Thread.Sleep(70000);
                }

                Issue issue = await _issuesClient.Create(GitHubRepositoryOwner, GitHubRepositoryName, gitHubIssue);

                Debug.WriteLine("Added Issue #{0} to GitHub.", issue.Number);

                gitHubIssuesCreated.Add(issue);

                issueCounter++;
            }

            //final step, update each issue to reflect correct status, etc.
            foreach (var gitHubIssue in gitHubIssuesCreated)
            {
                var gitHubIssueToUpdate = await _issuesClient.Get(GitHubRepositoryOwner, GitHubRepositoryName, gitHubIssue.Number);

                if (null == gitHubIssueToUpdate)
                {
                    Debugger.Break();
                }

                if (!googleIssues.Any(googleIssue => googleIssue.Id == gitHubIssue.Number))
                {
                    Debugger.Break();
                }


                var correspondingGoogleIssue = googleIssues.Single(s => s.Id == gitHubIssue.Number);

                Debug.WriteLine("Processing Issue #{0}", correspondingGoogleIssue.Id);

                var issueUpdate = new IssueUpdate();
                var newLabels = new List<string>();

                if (!correspondingGoogleIssue.IsOpen)
                {
                    issueUpdate.State = ItemState.Closed;
                }

                if (correspondingGoogleIssue.IsDefect)
                {
                    newLabels.Add("bug");
                }

                if (correspondingGoogleIssue.IsEnhancement)
                {
                    newLabels.Add("enhancement");
                }

                if (correspondingGoogleIssue.IsWontFix)
                {
                    newLabels.Add("wontfix");
                }

                if (correspondingGoogleIssue.IsInvalid)
                {
                    newLabels.Add("invalid");
                }

                issueUpdate.Labels = newLabels;

                await _issuesClient.Update(GitHubRepositoryOwner, GitHubRepositoryName, gitHubIssue.Number, issueUpdate);
            }
        }
    }
}
