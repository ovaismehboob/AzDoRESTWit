using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitQuickStarts
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return 0;
            }

            string connectionUrl, token, project = "";

            try
            {
                CheckArguments(args, out connectionUrl, out token, out project);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);

                ShowUsage();

                return -1;
            }

            try
            {
                Console.WriteLine("Executing quick start samples...");
                Console.WriteLine("");

                //todo: may need to adjust this to scale better for more samples.

                #region Bug
                //Instantiating CreateBug object
                Samples.CreateBug objBug = new Samples.CreateBug(connectionUrl, token, project);
                //execute the client lib code. If you want to run the direct http calls then adjust (see below)
                objBug.CreateBugUsingClientLib();
                objBug = null;
                #endregion

                #region Epic
                //Instantiating CreateEpic object
                Samples.CreateEpic epic = new Samples.CreateEpic(connectionUrl, token, project);
                epic.CreateEpicUsingClientLib();
                epic= null;
                #endregion

                #region Feature
                //Instantiating CreateFeature object 
                Samples.CreateFeature feature = new Samples.CreateFeature(connectionUrl, token, project);
                feature.CreateFeatureUsingClientLib();
                feature = null;
                #endregion

                #region Execute Query 
                //Instantiating ExecuteQuery object  
                Samples.ExecuteQuery objQuery = new Samples.ExecuteQuery(connectionUrl, token, project);
                objQuery.RunGetBugsQueryUsingClientLib();
                objQuery = null;
                #endregion
                  
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to run the sample: " + ex.Message);
                return 1;
            }

            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Runs the WIT Quick Start samples on a Team Services account or Team Foundation Server instance.");
            Console.WriteLine("");
            Console.WriteLine("These samples are to provide you the building blocks of using the REST API's in Work Item Tracking.");
            Console.WriteLine("Examples are written using the .NET client library and using direct HTTP calls. We recommend, that");
            Console.WriteLine("whenever possible, you use the .NET client library.");
            Console.WriteLine("");
            Console.WriteLine("!!WARNING!! Some samples are destructive. Always run on a test account or collection.");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /url:fabrikam.visualstudio.com /token:personalaccesstoken /project:projectname");
            Console.WriteLine("");

            Console.ReadKey();
        }

        private static void CheckArguments(string[] args, out string connectionUrl, out string token, out string project)
        {
            connectionUrl = null;
            token = null;
            project = null;
            
            Dictionary<string, string> argsMap = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                if (arg[0] == '/' && arg.IndexOf(':') > 1)
                {
                    string key = arg.Substring(1, arg.IndexOf(':') - 1);
                    string value = arg.Substring(arg.IndexOf(':') + 1);

                    switch (key)
                    {
                        case "url":
                            connectionUrl = value;
                            break;

                        case "token":
                            token = value;                          
                            break;

                        case "project":
                            project = value;
                            break;
                        default:
                            throw new ArgumentException("Unknown argument", key);
                    }
                }
            }

            if (connectionUrl == null || token == null)
            {
                throw new ArgumentException("Missing required arguments");
            }
        }


    }
}
