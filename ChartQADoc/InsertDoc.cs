using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using System.IO;

using services.varian.com.Patient.Documents;


namespace ChartQADoc
{
    //static utility class that can't be instantiated
    public static class InsertDoc
    {

        public static void InsertDocExecute(TextBox OutBox, string path, List<string> PatientInfo, string user)
        {
            OutBox.Visible = true;
            ConstructWebRequest(OutBox, path, PatientInfo, user);
            OutBox.AppendText(Environment.NewLine + Environment.NewLine + "Web request complete.");
        }

        private static void ConstructWebRequest(TextBox Outbox, string path, List<string> PatientInfo, string user)
        {
            //IMPORTANT - NOTES ON TROUBLESHOOTING ARIA WEB REQUESTS.
            //The problem with debugging web requests is you can have authentication and JSON format errors at the same time, but the web server will only tell you about one at a time.
            //then there is the added complication of the Varian APIKeys.
            //It is important to understand that the errors have an order of predence. So, you can have many errors at the same time, but the server will only send a message about the most important one.

            //First is an authentication error. This has to do with your username and password, NOT the API keys.
            //The server will respond with: "401 - Unauthorized: Access is denied due to invalid credentials".
            //you most likely need to include the domain before the username as well.

            //If the username and pasword are correct, then you can get a processing error if you JSON is formatted incorectly OR if you have the name of the request wrong.
            //The server will say "The server had an error processing your request". This is another basic HTML error like the authentication error.
            //It means that the server does not know what the hell you are talking about. It does not understand the request you made.
            //This could be formatting issues like backslashes, quotes, or curly braces. Or it could be that it does not recognize the name of the REST request.
            //You need to know the names of the specific requests availiable in the application you are querying. keep in mind that VS cannot help with this
            //like a SQL string, this is just a string in C# and you need to make sure yourself it is correct
            
            //If the username and password are correct, and the server recognizes the request (which means the name is correct and it can recognize wht it is. the properties of the request don't have to be correct)
            //then you can get an API key error. This is a clear error message from the application that you don't have the correct API key. At this point, our HTML request has gone through and we are now dealing with the application itself.

            //Finally, if you have the correct API Key, and everything else discussed earlier is good, now the system can actaully give you errors about the specific elements of the request.
            //for certain requests there are mandatory fields 

            //It is highly reccomended that you construct an object, using the Gateway file, representing the request you want to make, and then turn it into JSON using a JSON serializer, like I do here.
            //However, this is just for the elements of the request. You still need to write the beggining of the JSON string with the name of the request and make sure it correct.

            //This is Lahey's Aria Access API key. It can be used to interact with the REST requests outlined in the Aria Access Reference guide.
            //string ApiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx";  

            //This is Lahey's Oncoclogy Services API key. Again, as far as I can tell, it is used to interact with all the REST requests that are NOT in the Aria Access Reference guide.
            //There are a lot of other requests in the Gateway file that I generated from the REST service that I assume falls under this Oncology Services thing.
            string ApiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

            Outbox.AppendText("Constructing Aria Web Service Insert Document Request...");

            string bytesread = Convert.ToBase64String(File.ReadAllBytes(path));
            string patid = PatientInfo[5];
            int thefileformat = 10;  //10 is pdf
            string templatename = "Physics Chart QA Report";
            user = user.Remove(0,4);
            string theuser = "mr1\\\\" + user;
            string thesupervisor = "mr1\\\\Varian_Interface";
            //This format is important. First we calculate the so called UNIX time (milliseconds since midnight January 1, 1970 in UTC time) in UTC (Greenwhich Mean Time).
            long UNIXdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //However, Aria's timestamps are not in UTC, they are in our timezone, GMT-5.
            //So we subtract 5 hours (18000000 ms)
            UNIXdate = UNIXdate - 18000000;
            string date = "\\/Date(" + UNIXdate + ")\\/";

            //The "Patient Note" document type is specifically used because that is one of the few document types that Epic's MDM HL7 interface is set up to use.
            //The document is not marked as approved here (it could be, because we are authenticated through the API Key). But, when the physicist approves it in Aria
            //after running this script, it is automatically sent to Epic. Becuase the whole point is that there is a record of the Chart QAs that billing people can look at.
            //The Varian_Interface user is used as the supervisor becuase it is a generic login that has the permissions required to make new Aria documents.
            string request = "{\"__type\":\"InsertDocumentRequest:http://services.varian.com/Patient/Documents\"," +
                " \"PatientId\":{ \"ID1\":\"" + patid + "\"}," +
                " \"FileFormat\":" + thefileformat + "," +
                " \"IsMedOncDocument\":false," +
                " \"DocumentType\":{\"DocumentTypeDescription\":\"Patient Note\"}," +
                " \"TemplateName\":\"" + templatename + "\"," +
                " \"AuthoredByUser\":{\"SingleUserId\":\"" + theuser + "\"}," +
                " \"SupervisedByUser\":{\"SingleUserId\":\"" + thesupervisor + "\"}," +
                " \"EnteredByUser\": {\"SingleUserId\":\"" + theuser + "\"}," +
                " \"DateOfService\":\"" + date + "\"," +
                " \"DateEntered\":\"" + date + "\"," +
                " \"BinaryContent\":\"" + bytesread + "\"}";

            File.WriteAllText(@"\\wvariafssp01ss\VA_DATA$\ProgramData\Vision\PublishedScripts\Test.txt", request);
            Outbox.AppendText(Environment.NewLine + Environment.NewLine + "Sending Insert Document request to web server.");
            string response = SendWebRequest(request, true, ApiKey);

            //the raw HTTP response is abstruse, so we just do a basic check
            if(response.Contains("DocumentResponse:#VMS.OIS.ARIALocal.WebServices.Document.Contracts"))
            {
                Outbox.AppendText(Environment.NewLine + Environment.NewLine + "The Insert Document Request was successful!" + Environment.NewLine + Environment.NewLine + response);
            }
            else
            {
                Outbox.AppendText(Environment.NewLine + Environment.NewLine + "The Insert Document Request may have errors. Please check Eclipse to verify the document was made." + Environment.NewLine + Environment.NewLine + response);
            }
        }

        //wvariaplfp01ss is the name of Aria's license server, which also acts as the web server. 55051 is the specific port used to communicate with the Web API.
        //The rest of the url is for specifically directing the request to the REST service running on the web server (service.svc). The whole thing is required for it to work.
        //The end of the URL is different if you want to use SOAP, but I think REST and JSON are easier and the most widespread nowadays.
        private static string SendWebRequest(string request, bool bIsJson, string apiKey)
        {
            string sMediaTYpe = bIsJson ? "application/json" : "application/xml";
            string sResponse = null;
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                if (httpClient.DefaultRequestHeaders.Contains("ApiKey"))
                {
                    httpClient.DefaultRequestHeaders.Remove("ApiKey");
                }
                httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
                var task = httpClient.PostAsync("https://wvariaplfp01ss:55051/gateway/service.svc/interop/rest/process", new StringContent(request, Encoding.UTF8, sMediaTYpe));
                Task.WaitAll(task);
                Task<string> responseTask = task.Result.Content.ReadAsStringAsync();
                Task.WaitAll(responseTask);
                sResponse = responseTask.Result;
            }
            return sResponse;
        }
    }
}
