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
    public class ExperimentationWithOctoKitApi
    {
        private GitHubClient _client;
        private Credentials _basicAuth;
        private IIssuesClient _issuesClient;
        private Repository _repository;

        [SetUp]
        public async void SetUp()
        {
            _client = new GitHubClient(new ProductHeaderValue("sbohlen-test-2"));
            _basicAuth = new Credentials("af152b92524012fbd5c79230fac6b4570f6451e1");

            _client.Credentials = _basicAuth;
            _issuesClient = _client.Issue;

        }

        [Test]
        public async void CanAuthenticate()
        {
            var user = await _client.User.Current();
        }

        [Test]
        public async void CanListIssues()
        {
            _repository = await _client.Repository.Get("net-commons", "common-logging");

            var issues =
                await _issuesClient.GetForRepository("net-commons", "common-logging");

            Assert.That(issues.Count, Is.GreaterThan(0));

        }
    }
}
