using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

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


        public IEnumerable<GoogleIssue> GetIssues()
        {
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


        private string[] SplitStringIntoSeparateElements(string input, string delimiter = ",")
        {
            return input.Split(delimiter.ToCharArray());
        }
    }
}
