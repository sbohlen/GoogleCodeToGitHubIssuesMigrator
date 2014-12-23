using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IssuesImporter.Test
{
    [TestFixture]
    public class WhenCreatingNewGoogleIssuesReader
    {
        private GoogleIssuesReader _reader;
        private const string InputDataFile = @"TestData\TestInputDataFile.csv";

        [SetUp]
        public void SetUp()
        {
            _reader = new GoogleIssuesReader(InputDataFile);
        }


        [Test]
        public void CanAcceptDataFilePathInCtor()
        {
            Assert.That(_reader.InputDataFile, Is.EqualTo(InputDataFile));
        }

        [Test]
        public void CanValidateThatReadableFileIsValid()
        {
            Assert.That(_reader.IsInputDataFileValid(), Is.True);
        }

        [Test]
        public void CanValidateThatUnreadableFileIsInvalid()
        {
            var invalidReader = new GoogleIssuesReader("not-valid-file.csv");
            Assert.That(invalidReader.IsInputDataFileValid(), Is.False);
        }
        
    }
}
