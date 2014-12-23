using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IssuesImporter.Test
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test()
        {
            var settings = new Settings(new ConfigurationManagerKeyValueReader());
            settings.ReadConfig();
        }
    }
}
