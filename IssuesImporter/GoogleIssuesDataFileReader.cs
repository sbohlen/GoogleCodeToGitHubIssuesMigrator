using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        public IEnumerable<GoogleIssue> GetIssues()
        {
            var issues = new List<GoogleIssue>();

            var inputRows = File.ReadLines(_inputDataFile);

            foreach (var row in inputRows)
            {
                var elements = SplitRowIntoDataElements(row);

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
                        Labels = SplitRowIntoDataElements(elements[7]).ToList()
                    });
            }

            return issues;

        }

        private string[] SplitRowIntoDataElements(string row)
        {
            return row.Split(",".ToCharArray());
        }
    }

    public class GoogleIssue
    {
        public int Id { get; set; }
        public string IssueType { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Milestone { get; set; }
        public string Owner { get; set; }
        public string Summary { get; set; }
        public IEnumerable<string> Labels { get; set; }
    }
}
