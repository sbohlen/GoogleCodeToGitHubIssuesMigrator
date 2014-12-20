using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssuesImporter
{
    public class GoogleIssuesDataFileReader
    {
        private readonly string _inputDataFile;

        public GoogleIssuesDataFileReader(string inputDataFile)
        {
            _inputDataFile = inputDataFile;
        }

        public string InputDataFile
        {
            get { return _inputDataFile; }
        }

        public bool IsInputDataFileValid()
        {
            try
            {
                File.OpenRead(_inputDataFile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
