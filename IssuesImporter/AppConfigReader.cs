using System.Collections.Generic;
using System.Configuration;

namespace IssuesImporter
{
    public class AppConfigReader : IKeyValueReader
    {
        public string Read(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public IEnumerable<string> AllKeys()
        {
            return ConfigurationManager.AppSettings.AllKeys;
        }
    }
}