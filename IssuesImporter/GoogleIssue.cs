using System.Collections.Generic;

namespace IssuesImporter
{
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