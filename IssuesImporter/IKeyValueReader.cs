using System.Collections;
using System.Collections.Generic;

namespace IssuesImporter
{
    public interface IKeyValueReader
    {
        string Read(string key);
        IEnumerable<string> AllKeys();
    }
}