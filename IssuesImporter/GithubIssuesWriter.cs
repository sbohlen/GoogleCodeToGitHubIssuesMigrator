using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace IssuesImporter
{
    public class GitHubIssuesWriter
    {
        private Settings _settings;
        private readonly IIssuesClient _issuesClient;

        public GitHubIssuesWriter(Settings settings)
        {
            _settings = settings;

            var client = new GitHubClient(new ProductHeaderValue(_settings.GitHubApiProductHeaderValue))
            {
                Credentials = new Credentials(_settings.GitHubApiAccessToken)
            };

            _issuesClient = client.Issue;
        }

        public async Task WriteIssues(IEnumerable<GoogleIssue> googleIssues)
        {
            //convert to list to avoid re-enumeration of IEnumerable
            googleIssues = googleIssues.ToList();

            var gitHubIssuesToCreate = AssembleListOfGitHubIssuesToCreate(googleIssues);

            var gitHubIssuesCreated = await CreateGitHubIssues(gitHubIssuesToCreate);

            await UpdateGitHubIssues(gitHubIssuesCreated, googleIssues);
        }

        private async Task UpdateGitHubIssues(IEnumerable<Issue> gitHubIssuesCreated, IEnumerable<GoogleIssue> googleIssues)
        {
            const string gitHubIssueNotFoundStringFormatTemplate = "ERROR: Unable to retrieve Issue #{0} from GitHub.";

            //convert to list to avoid re-enumeration of IEnumerable
            googleIssues = googleIssues.ToList();

            //final step, update each issue to reflect correct status, etc.
            foreach (var gitHubIssue in gitHubIssuesCreated)
            {
                var correspondingGoogleIssue = googleIssues.Single(s => s.Id == gitHubIssue.Number);

                //santity-check: ensure that the target issue actually exists on GitHub, report error if not found
                try
                {
                    var gitHubIssueToUpdate = await _issuesClient.Get(_settings.GitHubRepositoryOwner, _settings.GitHubRepositoryName, gitHubIssue.Number);
                    if (null == gitHubIssueToUpdate)
                    {
                        Debug.WriteLine(gitHubIssueNotFoundStringFormatTemplate, correspondingGoogleIssue.Id);
                        continue;
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine(gitHubIssueNotFoundStringFormatTemplate, correspondingGoogleIssue.Id);
                    continue;
                }

                //assuming we got this far, process the update...
                Debug.WriteLine("Processing Update to Issue #{0}", correspondingGoogleIssue.Id);

                var issueUpdate = ComposeIssueUpdate(correspondingGoogleIssue);

                await _issuesClient.Update(_settings.GitHubRepositoryOwner, _settings.GitHubRepositoryName, gitHubIssue.Number, issueUpdate);
            }
        }

        private async Task<IEnumerable<Issue>> CreateGitHubIssues(IEnumerable<NewIssue> gitHubIssuesToCreate)
        {
            var gitHubIssuesCreated = new List<Issue>();

            var issueCounter = 1.0d;


            //next step, add them to the github instance ...
            foreach (var gitHubIssue in gitHubIssuesToCreate)
            {

                if (Math.Abs(issueCounter % _settings.GitHubApiThrottleOnCreateInvocationCount) < 0.1)
                {
                    Debug.WriteLine("Sleeping for {0} milliseconds to avoid GitHub anti-DoS policies on CREATE API calls...", _settings.GitHubApiThrottleOnCreatePauseDurationMilliseconds);
                    Thread.Sleep(_settings.GitHubApiThrottleOnCreatePauseDurationMilliseconds);
                }

                var issue = await _issuesClient.Create(_settings.GitHubRepositoryOwner, _settings.GitHubRepositoryName, gitHubIssue);

                Debug.WriteLine("Added Issue #{0} to GitHub.", issue.Number);

                gitHubIssuesCreated.Add(issue);

                issueCounter++;
            }

            return gitHubIssuesCreated;
        }


        private IEnumerable<NewIssue> AssembleListOfGitHubIssuesToCreate(IEnumerable<GoogleIssue> googleIssues)
        {
            var gitHubIssuesToCreate = new List<NewIssue>();

            foreach (var googleIssue in googleIssues)
            {
                var newIssue = new NewIssue(googleIssue.Summary);
                var googleCodeIssueUrl = ComposeGoogleCodeIssueUrl(googleIssue);
                newIssue.Body =
                    string.Format(
                        "This issue ported from the deprecated GoogleCode {0} site ({1})." +
                        "\n\nSee {2} for more history and details for this specific issue.",
                        _settings.GoogleCodeProjectName, _settings.GoogleCodeProjectUrl, googleCodeIssueUrl);

                gitHubIssuesToCreate.Add(newIssue);
            }

            return gitHubIssuesToCreate;
        }

        private IssueUpdate ComposeIssueUpdate(GoogleIssue googleIssue)
        {
            var issueUpdate = new IssueUpdate();
            AddLabelsToIssueUpdate(googleIssue, issueUpdate);

            //any other work to manipulate the update would go here...

            return issueUpdate;
        }

        private void AddLabelsToIssueUpdate(GoogleIssue googleIssue, IssueUpdate issueUpdate)
        {

            //if there is no IssueUpdate.Labels collection, we need to create a new one...
            if (null == issueUpdate.Labels)
            {
                issueUpdate.Labels = new List<string>();
            }

            if (!googleIssue.IsOpen)
            {
                issueUpdate.State = ItemState.Closed;
            }

            if (googleIssue.IsDefect)
            {
                issueUpdate.Labels.Add("bug");
            }

            if (googleIssue.IsEnhancement)
            {
                issueUpdate.Labels.Add("enhancement");
            }

            if (googleIssue.IsWontFix)
            {
                issueUpdate.Labels.Add("wontfix");
            }

            if (googleIssue.IsInvalid)
            {
                issueUpdate.Labels.Add("invalid");
            }

        }

        private string ComposeGoogleCodeIssueUrl(GoogleIssue googleIssue)
        {
            return string.Format(_settings.GoogleCodeIssueUrlStringFormatTemplate, googleIssue.Id);
        }
    }
}
