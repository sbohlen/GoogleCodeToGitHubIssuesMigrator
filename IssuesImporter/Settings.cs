using System;
using System.Collections.Generic;
using System.Configuration;
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

        private IKeyValueReader _keyValueReader;

        public Settings(IKeyValueReader keyValueReader)
        {
            _keyValueReader = keyValueReader;
            
            //set some reasonable (usually) functional defaults for the optional app.config values
            GoogleCodeProjectUrl = string.Format("https://code.google.com/p/{0}", GoogleCodeProjectName);
            GoogleCodeIssueUrlStringFormatTemplate = string.Format("{0}/issues/detail?id={{0}}", GoogleCodeProjectUrl);
            GitHubApiThrottleOnCreateInvocationCount = 20;
            GitHubApiThrottleOnCreatePauseDurationMilliseconds = 70000;
            GitHubApiProductHeaderValue = "google%20code%20to%20github%20issue%20migrator";
        }

        public void ReadConfig()
        {
            var allConfigKeys = _keyValueReader.AllKeys().ToArray();

            ValidateAllMandatoryValuesProvided(allConfigKeys);

            ReadMandatoryValues();
            
            ReadOptionalValuesIfPresent(allConfigKeys);
        }

        private void ValidateAllMandatoryValuesProvided(IEnumerable<string> allConfigKeys)
        {
            var missingMandatoryKeys = GetMissingMandatoryKeys(allConfigKeys).ToArray();

            if (missingMandatoryKeys.Any())
            {
                var builder = new StringBuilder("Missing required Configuration key(s): ");

                foreach (var missingMandatoryValue in missingMandatoryKeys)
                {
                    builder.Append(missingMandatoryValue);
                    if (Array.IndexOf(missingMandatoryKeys, missingMandatoryValue) < missingMandatoryKeys.Length - 1)
                    {
                        builder.Append(", ");
                    }
                }

                throw new ConfigurationErrorsException(builder.ToString());
            }
        }

        private void ReadOptionalValuesIfPresent(string[] allConfigKeys)
        {
            //optional values...
            if (allConfigKeys.Contains("GitHubApiProductHeaderValue"))
            {
                GitHubApiProductHeaderValue = _keyValueReader.Read("GitHubApiProductHeaderValue");
            }

            if (allConfigKeys.Contains("GitHubApiThrottleOnCreateInvocationCount"))
            {
                GitHubApiThrottleOnCreateInvocationCount = int.Parse(_keyValueReader.Read("GitHubApiThrottleOnCreateInvocationCount"));
            }

            if (allConfigKeys.Contains("GitHubApiThrottleOnCreatePauseDurationMilliseconds"))
            {
                GitHubApiThrottleOnCreatePauseDurationMilliseconds = int.Parse(_keyValueReader.Read("GitHubApiThrottleOnCreatePauseDurationMilliseconds"));
            }

            if (allConfigKeys.Contains("GoogleCodeIssueUrlStringFormatTemplate"))
            {
                GoogleCodeIssueUrlStringFormatTemplate = _keyValueReader.Read("GoogleCodeIssueUrlStringFormatTemplate");
            }

            if (allConfigKeys.Contains("GoogleCodeProjectUrl"))
            {
                GoogleCodeProjectUrl = _keyValueReader.Read("GoogleCodeProjectUrl");
            }
        }

        private void ReadMandatoryValues()
        {
            GitHubApiAccessToken = _keyValueReader.Read("GitHubApiAccessToken");
            GitHubRepositoryOwner = _keyValueReader.Read("GitHubRepositoryOwner");
            GitHubRepositoryName = _keyValueReader.Read("GitHubRepositoryName");
            GoogleCodeProjectName = _keyValueReader.Read("GoogleCodeProjectName");
        }

        private IEnumerable<string> GetMissingMandatoryKeys(IEnumerable<string> allConfigKeys)
        {
            allConfigKeys = allConfigKeys.ToArray();

            var missing = new List<string>();

            if (!allConfigKeys.Contains("GitHubApiAccessToken"))
            {
                missing.Add("GitHubApiAccessToken");
            }

            if (!allConfigKeys.Contains("GitHubRepositoryOwner"))
            {
                missing.Add("GitHubRepositoryOwner");
            }

            if (!allConfigKeys.Contains("GitHubRepositoryName"))
            {
                missing.Add("GitHubRepositoryName");
            }

            if (!allConfigKeys.Contains("GoogleCodeProjectName"))
            {
                missing.Add("GoogleCodeProjectName");
            }

            return missing;
        }
    }
}
