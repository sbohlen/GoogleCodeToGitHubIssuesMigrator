using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssuesImporter;

namespace IssueImporterApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings(new AppConfigReader());
            settings.ReadConfig();

            var reader = new GoogleIssuesReader(@"InputGoogleCodeIssues.csv");
            var googleIssues = reader.GetIssues();

            var writer = new GitHubIssuesWriter(settings);

            //since its invalid to mark a console app's entry point as async,
            // we cannot use an await here so have to do the equivalent w/ the Task API directly...
            Task.Factory.StartNew(() => writer.WriteIssues(googleIssues)).Wait();

            Console.WriteLine("Import complete, press any key to exit...");
            Console.ReadKey();
        }
    }
}
