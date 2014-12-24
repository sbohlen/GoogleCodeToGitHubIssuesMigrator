##GoogleCode To GitHub Issues Migrator##
================================
Down-and-Dirty utility to facilatate the migration of issues from Google Code projects to GitHub projects.

###CAUTION: Intented to run _only_ on GitHub repos that do NOT already have issues added to them###
This is intended to facilitate one-time migration of issues from Google Code into a _new_ GitHub repo; it is _not_ intended for repeated syncronization of issues over time.

###How To Use###
1. Clone this repo and compile/build it.
1. Since Google Code has deprecated its API that would permit you to access the issues programmatically, we must instead manually export the Google Code Issues to a CSV for subsequent processing to GitHub.  Navigate to your Google Code issues list (e.g., https://code.google.com/p/ndbunit/issues/list).
1. You can only export issues from Google Code if they are shown on the issues list.  This utility will only work if ALL Google Code issues are exported at once.  Since Google Code defaults to showing just your OPEN issues, you **must** perform a search for ALL ISSUES before you move to the next step.
1. In the lower-right-hand corner of the Google Code issues list is a link labeled "CSV".  Click this to download a CSV file of the presently-displayed issues on your Google Code site.
1. Name this file `InputGoogleCodeIssues.csv` and save it in the same directory as the console app binary (e.g., `IssueImporterApplication.exe`) you built in step 1.
1. Edit this CSV file to remove the first row (e.g. the header row); this app assumes that there is no header row in the source CSV file it will read and will throw an exception if the CSV file contains anything other than valid input data.
1. Locate the `IssueImporterApplication.exe.config` file in the same folder as the console app binary and edit it per the following table to reflect the values relevant to your scenario.
1. Run the `IssueImporterApplication.exe` console app to read the values from the CSV, insert each into your your GitHub repo as a new issue, and then update each issue to reflect the proper state (e.g. OPEN vs. CLOSED) and labels (e.g., bug, enhancement, wontfix) based on these values for the issues on Google Code.

###App.Config Settings###
All behavior is controlled via a series of app.config settings as follows:

| Key | Mandatory? | Sample Value | Description |
| --- | --- | --- | --- |
| `GitHubApiAccessToken` | YES | 03f37ffab000ed46dfa912a554f03d2cc8bf2d67 | your GitHub API Personal Access Token acquire from https://github.com/settings/applications |
| `GitHubRepositoryOwner` | YES | sbohlen | the name of the owner of the GitHub repo (e.g., from the https://github.com/OWNER/repository url structure) |
| `GitHubRepositoryName` | YES | MyRepo | the name of the GitHub repo (e.g., from the https://github.com/owner/REPOSITORY url structure) |
| `GoogleCodeProjectName` | YES | MyGoogleProject | the name of the project on Google Code (e.g., from the http://code.google.com/p/PROJECTNAME url structure) |
| `GoogleCodeProjectUrl` | NO | https://code.google.com/p/MyGoogleProject | the url of the project on Google Code (will be inferred based on the value of GoogleCodeProjectName if not provided explicitly) |
| `GitHubApiProductHeaderValue` | NO | my-api-access-project | identifies your API access to GitHub; can be any arbitrary http-encoded string |
| `GitHubApiThrottleOnCreateInvocationCount` | NO | 20 | number of CREATE calls to the GitHub API to perform before pausing for the length of `GitHubApiThrottleOnCreatePauseDurationMilliseconds`; defaults to 20, the requisite value to avoid colliding with GitHub API throtting when operating on public repositories |
| `GitHubApiThrottleOnCreatePauseDurationMilliseconds` | NO | 70000 | milliseconds to pause CREATE calls to the API each time the `GitHubApiThrottleOnCreateInvocationCount` interval is reached | 

Sample App.Config file is as follows (also found in the source repository here on GitHub):

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>

  <appSettings>
    <add key="GitHubApiAccessToken" value="03f37ffab000ed46dfa912a554f03d2cc8bf2d67"/>
    <add key="GitHubRepositoryOwner" value="NDbUnit"/>
    <add key="GitHubRepositoryName" value="test-issues-import"/>
    <add key="GoogleCodeProjectName" value="NDbUnit"/>
    <add key="GoogleCodeProjectUrl" value="https://code.google.com/p/ndbunit"/>
    <add key="GoogleCodeIssueUrlStringFormatTemplate" value="https://code.google.com/p/ndbunit/issues/detail?id={0}"/>
    <add key="GitHubApiProductHeaderValue" value="google%20code%20to%20github%20issue%20migrator"/>
    <add key="GitHubApiThrottleOnCreateInvocationCount" value="20"/>
    <add key="GitHubApiThrottleOnCreatePauseDurationMilliseconds" value="70000"/>
  </appSettings>
</configuration>
```
###Issues, Disclaimer###
Let me know if you unconver any issues in using this utility; its a bit down-and-dirty (some error handling, but not completely robust) so while it worked just fine for my needs, I cannot validate that it will handle all situations properly.  For this reason, I _strongly_ recommend creating a temporary "test repository" on GitHub and attempting to use this utility to import the issues into that first to ensure that the behavior is what you expect before running this against your actual target repository.

Also, please note that since this utility expects that the GitHub Issue Number will correlate to the GoogleCode Issue Number, this utility can only be run _once_ per GitHub repo (because that's the only way to ensure that the Issues numbering starts at 1 during the import).  This further argues for testing this utility against a fresh 'test repo' on GitHub so that you can ensure proper behavior (since you will only be able to run this utility exactly ONCE against your actual final target repo).  **YOU HAVE BEEN WARNED!**
