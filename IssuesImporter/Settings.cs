using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssuesImporter
{
    public class Settings
    {
        public string GitHubApiAccessToken { get; private set; }
        public string GitHubApiProductHeaderValue { get; private set; }
        public string GitHubRepositoryOwner { get; private set; }
        public string GitHubRepositoryName { get; private set; }
        public string GoogleCodeProjectName { get; private set; }
        public string GoogleCodeProjectUrl { get; private set; }
        public string GoogleCodeIssueUrlStringFormatTemplate { get; private set; }
        public int GitHubApiThrottleOnCreateInvocationCount { get; private set; }
        public int GitHubApiThrottleOnCreatePauseDurationMilliseconds { get; private set; }
    }
}
