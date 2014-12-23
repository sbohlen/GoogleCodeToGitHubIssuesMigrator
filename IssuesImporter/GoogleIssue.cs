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

        public bool IsOpen
        {
            get
            {
                var candidate = Status.ToUpper();
                return candidate == "NEW" || candidate == "ACCEPTED" || candidate == "STARTED";
            }
        }

        public bool IsDefect
        {
            get { return IssueType.ToUpper() == "DEFECT"; }
        }

        public bool IsEnhancement
        {
            get { return IssueType.ToUpper() == "ENHANCEMENT"; }
        }

        public bool IsWontFix
        {
            get { return Status.ToUpper() == "WONTFIX"; }
        } 
        
        public bool IsInvalid
        {
            get { return Status.ToUpper() == "INVALID"; }
        }
    }
}