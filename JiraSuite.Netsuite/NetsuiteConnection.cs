using JiraSuite.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using JiraSuite.DataAccess.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JiraSuite.Netsuite
{
    public class NetsuiteConnection
    {
        private readonly string _netsuiteUrlTrackedUpdate = "https://rest.netsuite.com/app/site/hosting/restlet.nl?script=35&deploy=1";
        private readonly string _netsuiteUrlTrackedNoUpdate = "https://rest.netsuite.com/app/site/hosting/restlet.nl?script=32&deploy=1";
        private readonly string _netsuiteUrlFixUpdate = "https://rest.netsuite.com/app/site/hosting/restlet.nl?script=36&deploy=1";
        private readonly string _netsuiteUrlGetTicketByNumber = "https://rest.na2.netsuite.com/app/site/hosting/restlet.nl?script=37&deploy=1";
        private JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private JiraSuiteDbContext _dbContext = DBContextManager.Instance.DbContext;


        private void AddRequestHeaders(WebRequest request)
        {
            request.Headers.Add(HttpRequestHeader.Authorization, "NLAuth nlauth_account=926273, nlauth_email=mwillis@motionsoft.net, nlauth_signature=@Power88cake, nlauth_role=3");
            request.ContentType = "application/json";
        }


        public void UpdateTicketWithoutLocalNsTicket(string ticket, JiraIssue issue)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_netsuiteUrlGetTicketByNumber);
            AddRequestHeaders(request);
            request.Method = WebRequestMethods.Http.Post;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                var ser = new JavaScriptSerializer();
                var payload = ser.Serialize(new
                {
                    caseNumber = ticket
                });
                writer.Write(payload);
                writer.Flush();
                writer.Close();
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd();
                    Assert.IsTrue(Convert.ToBoolean(result));
                    NetsuiteApiResult newResult = DeserializeResult(result, _dbContext).FirstOrDefault();
                    UpdateExistingTicketWithJiraStatus(newResult, issue);
                }
            }
            catch (Exception ex) { }
        }

        public void UpdateExistingTicketWithJiraStatus(NetsuiteApiResult nsTicket, JiraIssue jIssue)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_netsuiteUrlTrackedUpdate);
            AddRequestHeaders(request);
            request.Method = WebRequestMethods.Http.Post;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                var ser = new JavaScriptSerializer();
                var payload = ser.Serialize(new
                {
                    id = nsTicket.id,
                    jiraStatus = jIssue.Status,
                    fixVersion = (jIssue.FixVersion != null && jIssue.FixVersion.Length > 0 ? string.Join(",", (object) jIssue.FixVersion) : ""),
                    priority = "",
                    components = "",
                    type = jIssue.IssueType
                });
                writer.Write(payload);
                writer.Flush();
                writer.Close();
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd();
                    Assert.IsTrue(Convert.ToBoolean(result));
                }
            }
            catch (Exception ex) { }
        }



        public List<NetsuiteApiResult> GetAllTickets(JiraSuiteDbContext dbContext)
        {
            List<NetsuiteApiResult> allTickets = new List<NetsuiteApiResult>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_netsuiteUrlTrackedUpdate);
            AddRequestHeaders(request);
            using (var stream = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                var responseObj = stream.ReadToEnd();
                allTickets.AddRange(DeserializeResult(responseObj, dbContext));
            }
            return allTickets;
        }



        private List<NetsuiteApiResult> DeserializeResult(string responseString, JiraSuiteDbContext dbContext)
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
                                if (dbContext.NetsuiteCompanies.Find(thisarraySet[i + 4].Replace("\"", "")) != null)
                                    thisResult.columns.company =
                                        dbContext.NetsuiteCompanies.Find(thisarraySet[i + 4].Replace("\"", ""));
                                else
                                {
                                    thisResult.columns.company = dbContext.NetsuiteCompanies.Create();
                                    thisResult.columns.company.name = thisarraySet[i + 4].Replace("\"", "");
                                    dbContext.Entry(thisResult.columns.company).State = EntityState.Added; ;
                                    dbContext.SaveChanges();
                                }
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
                                if (dbContext.NetuiteContacts.Find(columnValue) != null)
                                    thisResult.columns.contact = dbContext.NetuiteContacts.Find(columnValue);
                                else
                                {
                                    thisResult.columns.contact = dbContext.NetuiteContacts.Create();
                                    thisResult.columns.contact.name = columnValue;
                                    dbContext.Entry(thisResult.columns.contact).State = EntityState.Added; ;
                                    dbContext.SaveChanges();
                                }
                                break;
                            case "stage":
                                if (dbContext.NetsuiteStages.Find(columnValue) != null)
                                    thisResult.columns.stage = dbContext.NetsuiteStages.Find(columnValue);
                                else
                                {
                                    thisResult.columns.stage = dbContext.NetsuiteStages.Create();
                                    thisResult.columns.stage.name = columnValue;
                                    dbContext.Entry(thisResult.columns.stage).State = EntityState.Added; ;
                                    dbContext.SaveChanges();
                                }
                                break;
                            case "status":
                                if (dbContext.NetsuiteStatuses.Find(columnValue) != null)
                                    thisResult.columns.status = dbContext.NetsuiteStatuses.Find(columnValue);
                                else
                                {
                                    thisResult.columns.status = dbContext.NetsuiteStatuses.Create();
                                    thisResult.columns.status.name = columnValue;
                                    dbContext.Entry(thisResult.columns.status).State = EntityState.Added; ;
                                    dbContext.SaveChanges();
                                }

                                break;
                            case "startdate":
                                thisResult.columns.startdate =
                                    responseArray[i + (loopCounter*10)].Split('\\')[3].Replace("\"", "");
                                if (
                                    dbContext.NetsuiteCategories.Find(
                                        responseArray[i + (loopCounter*10)].Split('\\')[13].Replace("\"", "")) != null)
                                    thisResult.columns.category =
                                        dbContext.NetsuiteCategories.Find(
                                            responseArray[i + (loopCounter*10)].Split('\\')[13].Replace("\"", ""));
                                else
                                {
                                    thisResult.columns.category = dbContext.NetsuiteCategories.Create();
                                    thisResult.columns.category.name =
                                        responseArray[i + (loopCounter*10)].Split('\\')[13].Replace("\"", "");
                                    dbContext.Entry(thisResult.columns.category).State = EntityState.Added;;
                                    dbContext.SaveChanges();
                                }
                                break;
                            case "assigned":
                                if (dbContext.NetsuiteAssigndes.Find(columnValue) != null)
                                    thisResult.columns.assigned = dbContext.NetsuiteAssigndes.Find(columnValue);
                                else
                                {
                                    thisResult.columns.assigned = dbContext.NetsuiteAssigndes.Create();
                                    thisResult.columns.assigned.name = columnValue;
                                    dbContext.Entry(thisResult.columns.assigned).State = EntityState.Added; ;
                                    dbContext.SaveChanges();
                                }
                                break;
                            case "priority":
                                if (dbContext.NetsuitePriorities.Find(columnValue) != null)
                                    thisResult.columns.priority = dbContext.NetsuitePriorities.Find(columnValue);
                                else
                                {
                                    thisResult.columns.priority = dbContext.NetsuitePriorities.Create();
                                    thisResult.columns.priority.name = columnValue;
                                    dbContext.Entry(thisResult.columns.priority).State = EntityState.Added; ;
                                    dbContext.SaveChanges();
                                }
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
                                            columnValue = refinedFields.ToArray()[j + 2];
                                            foreach (var ticket in columnValue.Split(','))
                                                thisResult.columns.JiraIssues.Add(
                                                    dbContext.JiraIssues.Find(ticket) ?? 
                                                    new JiraIssue()
                                                {
                                                    IssueKey = ticket
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
                    FillEmptyReferences(thisResult);
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

        public void FillEmptyReferences(NetsuiteApiResult thisResult)
        {
            var properties = typeof(Columns).GetProperties();
            foreach (var prop in properties)
            {
                try
                {
                    if (prop.GetValue(thisResult.columns) == null)
                        prop.SetValue(thisResult.columns, Activator.CreateInstance(prop.PropertyType));
                }
                catch { }
            }
        }


    }
}
