using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace WitQuickStarts.Samples
{

    public class CreateEpic
    {
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _project;

        public CreateEpic()
        {
            _uri = "https://accountname.visualstudio.com";
            _personalAccessToken = "personal access token";
            _project = "project name";
        }

        /// <summary>
        /// Constructor. Manaully set values to match your account.
        /// </summary>
        public CreateEpic(string url, string pat, string project)
        {
            _uri = url;
            _personalAccessToken = pat;
            _project = project;
        }

        /// <summary>
        /// Create an Epic using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>    
        public WorkItem CreateEpicUsingClientLib()
        {
            Uri uri = new Uri(_uri);
            string personalAccessToken = _personalAccessToken;
            string project = _project;

            VssBasicCredential credentials = new VssBasicCredential("", _personalAccessToken);
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            //add fields and thier values to your patch document
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Test Epic 1"
                }
            );



            VssConnection connection = new VssConnection(uri, credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, project, "Epic").Result;

                Console.WriteLine("Epic Successfully Created: Epic #{0}", result.Id);

                return result;
            }
            catch (AggregateException ex)   
            {
                Console.WriteLine("Error creating Epic: {0}", ex.InnerException.Message);
                return null;
            }
        }

        /// <summary>
        /// Create an Epic using direct HTTP
        /// </summary>     
        public WorkItem CreateEpicUsingHTTP()
        {
            string uri = _uri;
            string personalAccessToken = _personalAccessToken;
            string project = _project;
            string credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            Object[] patchDocument = new Object[1];

            patchDocument[0] = new { op = "add", path = "/fields/System.Title", value = "Test Epic" };

            using (var client = new HttpClient())
            {
                //set our headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                //serialize the fields array into a json string
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json");

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, uri + "/" + project + "/_apis/wit/workitems/$Epic?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                //if the response is successfull, set the result to the workitem object
                if (response.IsSuccessStatusCode)
                {
                    var workItem = response.Content.ReadAsAsync<WorkItem>().Result;

                    Console.WriteLine("Epic Successfully Created: Epic #{0}", workItem.Id);
                    return workItem;
                }
                else
                {
                    Console.WriteLine("Error creating Epic: {0}", response.Content);
                    return null;
                }
            }
        }
    }

}
