using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IssuesImporter.Test
{
    [TestFixture]
    public class WhenReadingData
    {
        private GoogleIssuesDataFileReader _reader;
        private const string InputDataFile = @"..\..\TestData\TestInputDataFile.csv";

        [SetUp]
        public void SetUp()
        {
            _reader = new GoogleIssuesDataFileReader(InputDataFile);
        }

        [Test]
        public void CanHydrateGoogleIssues()
        {
            var issues = _reader.GetIssues();

        }
    }
}
