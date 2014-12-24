using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Common.Logging.Simple;
using IssuesImporter;

namespace IssueImporterApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //not required; configured via app.config instead...
            //LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter();

            var logger = LogManager.GetLogger<Program>();

            try
            {
                var settings = new Settings(new AppConfigReader());
                settings.ReadConfig();

                var reader = new GoogleIssuesReader(@"InputGoogleCodeIssues.csv");
                var googleIssues = reader.GetIssues();

                var writer = new GitHubIssuesWriter(settings);

                //since its invalid to mark a console app's entry point as async,
                // we cannot use an await here so have to do the equivalent w/ the Task API directly...
                Task.Factory.StartNew(() => writer.WriteIssues(googleIssues)).Wait();

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.Error("Uh-Oh...something has gone horribly wrong! :-/", ex);
            }
        }
    }
}
