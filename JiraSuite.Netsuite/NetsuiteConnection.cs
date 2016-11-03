using JiraSuite.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace JiraSuite.Netsuite
{
    public class NetsuiteConnection
    {
        private readonly string _netsuiteUrlTrackedUpdate = "https://rest.netsuite.com/app/site/hosting/restlet.nl?script=35&deploy=1";
        private readonly string _netsuiteUrlTrackedNoUpdate = "https://rest.netsuite.com/app/site/hosting/restlet.nl?script=32&deploy=1";
        private readonly string _netsuiteUrlFixUpdate = "https://rest.netsuite.com/app/site/hosting/restlet.nl?script=36&deploy=1";
        private JavaScriptSerializer _serializer = new JavaScriptSerializer();


        private void AddRequestHeaders(WebRequest request)
        {
            request.Headers.Add(HttpRequestHeader.Authorization, "NLAuth nlauth_account=926273, nlauth_email=mwillis@motionsoft.net, nlauth_signature=@Power88cake, nlauth_role=3");
            request.ContentType = "application/json";
        }

        public List<NetsuiteApiResult> GetAllTickets()
        {
            List<NetsuiteApiResult> allTickets = new List<NetsuiteApiResult>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_netsuiteUrlTrackedUpdate);
            AddRequestHeaders(request);
            using (var stream = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                var responseObj = stream.ReadToEnd();
                allTickets.AddRange(DeserializeResult(responseObj));
            }
            return allTickets;
        }



        private List<NetsuiteApiResult> DeserializeResult(string responseString)
        {
            List<NetsuiteApiResult> parsedList = new List<NetsuiteApiResult>();
            string[] responseArray = responseString.Split('}');
            
            var loopCounter = 0;
            while (loopCounter < responseArray.Length/10)
            {
                try
                {
                    NetsuiteApiResult thisResult = new NetsuiteApiResult() {columns = new Columns()};

                    var thisarraySet = responseArray[(loopCounter*10)].Split('\\');
                    for (var i = 0; i < thisarraySet.Length - 2; i++)
                    {
                        var column = thisarraySet[i].Replace("\"", "");
                        var value = thisarraySet[i + 2].Replace("\"", "");
                        switch (column)
                        {
                            case "id":
                                thisResult.id = value;
                                i += 3;
                                break;
                            case "recordtype":
                                thisResult.recordtype = value;
                                i += 3;
                                break;
                            case "casenumber":
                                thisResult.columns.casenumber = value;
                                i += 3;
                                break;
                            case "title":
                                thisResult.columns.title = value;
                                i += 3;
                                break;
                            case "company":
                                thisResult.columns.company = new Company()
                                {
                                    name = thisarraySet[i + 4].Replace("\"", "")
                                };
                                i += 3;
                                break;
                        }
                    }

                    for (var i = 1; i < 9; i++)
                    {
                        string columnName = string.Empty, columnValue = string.Empty;
                        try
                        {
                            columnName = responseArray[i + (loopCounter*10)].Split('\\')[1].Replace("\"", "");
                            columnValue = responseArray[i + (loopCounter * 10)].Split('\\')[5].Replace("\"", "");
                        }
                        catch{}

                        switch (columnName)
                        {
                            case "casenumber":
                                thisResult.columns.casenumber = columnValue;
                                break;
                            case "contact":
                                thisResult.columns.contact = new Contact() {name = columnValue};
                                break;
                            case "stage":
                                thisResult.columns.stage = new Stage() {name = columnValue};
                                break;
                            case "status":
                                thisResult.columns.status = new Status() {name = columnValue};
                                break;
                            case "startdate":
                                thisResult.columns.startdate =
                                    responseArray[i + (loopCounter*10)].Split('\\')[3].Replace("\"", "");
                                break;
                            case "assigned":
                                thisResult.columns.assigned = new Assigned() {name = columnValue};
                                break;
                            case "priority":
                                thisResult.columns.priority = new Priority() {name = columnValue};
                                break;
                            case "helpdesk":
                                //ASSIGN CUSTOM FIELDS HERE!!!
                                var refinedFields = from str in responseArray[i + (loopCounter*10)].Split('\\')
                                    select str.Replace("\"", "");
                                for (var j = 0; j < refinedFields.ToArray().Length; j++)
                                {
                                    switch (refinedFields.ToArray()[j])
                                    {
                                        case "custeventsn_case_number":
                                            thisResult.columns.JiraIssues.Add(new JiraIssue()
                                            {
                                                IssueKey = refinedFields.ToArray()[j + 2]
                                            });
                                            break;
                                        case "custeventescalatedto":
                                            thisResult.columns.escalatedto = refinedFields.ToArray()[j + 2];
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    parsedList.Add(thisResult);
                    loopCounter++;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }
            }
            return parsedList;
        }
        
    }
}
