using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using CsvHelper;
using Octokit;

namespace IssuesImporter
{
    public class GoogleIssuesReader
    {
        private readonly string _inputDataFile;
        public ILog Logger { private get; set; }

        public GoogleIssuesReader(string inputDataFile)
        {
            _inputDataFile = inputDataFile;
            Logger = LogManager.GetLogger(this.GetType());
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


        public IEnumerable<GoogleIssue> GetIssues()
        {
            if (!IsInputDataFileValid())
            {
                var message = string.Format("Cannot access inoput file [{0}].  Ensure that the file exists and can be opened for READ access.", _inputDataFile);
                Logger.Error(message);
                throw new InvalidOperationException(message);
            }

            var csv = new CsvParser(new StringReader(File.ReadAllText(InputDataFile)));

            var issues = new List<GoogleIssue>();

            while (true)
            {
                var elements = csv.Read();
                if (elements == null)
                {
                    break;
                }

                issues.Add(
                    new GoogleIssue()
                    {
                        Id = Convert.ToInt16(elements[0]),
                        IssueType = elements[1],
                        Status = elements[2],
                        Priority = elements[3],
                        Milestone = elements[4],
                        Owner = elements[5],
                        Summary = elements[6],
                        Labels = SplitStringIntoSeparateElements(elements[7]).ToList()
                    });

            }

            return issues;

        }


        private IEnumerable<string> SplitStringIntoSeparateElements(string input, string delimiter = ",")
        {
            return input.Split(delimiter.ToCharArray());
        }
    }
}
