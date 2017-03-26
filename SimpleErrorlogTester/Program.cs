using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.Security;
using SPOSimpleErrorlog;

namespace SimpleErrorlogTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "https://mod681489.sharepoint.com/sites/olaf-tester";
            var user = "admin@mod681489.onmicrosoft.com";
            var pw = "pass@word1";

            var password = new SecureString();
            foreach (char c in pw.ToCharArray()) password.AppendChar(c);

            using (var ctx = new ClientContext(url))
            {
                ctx.Credentials = new SharePointOnlineCredentials(user, password);

                Web web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQueryRetry();

                SimpleErrorlog errorLog = new SimpleErrorlog(ctx);
                errorLog.EnsureList();

                errorLog.SetCorrelation();

                errorLog.WriteInformation("SimpleErrorlogTester", "Information message");
                errorLog.WriteWarning("SimpleErrorlogTester", "Warning message");
                errorLog.WriteError("SimpleErrorlogTester", "Error message");

                try
                {
                    int i = 10;
                    int j = 0;
                    int k = i / j;
                }
                catch (Exception ex)
                {
                    errorLog.WriteError("ExceptionTester", ex);
                }
            }
        }
    }
}
