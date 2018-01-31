using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JiraSuite.DataAccess.EntityFramework;
using TechTalk.JiraRestClient;
using static JiraSuite.DataAccess.Models.JiraTypeHelper;
using System.Web.Script.Serialization;

namespace JiraSuite.DataAccess.Models
{
    public class JiraIssue
    {
        private readonly JiraSuiteDbContext _dbContext = DBContextManager.Instance.DbContext;
        private JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private List<FixVersion> _fixVersions = new List<FixVersion>();
        
        public string IssueId { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string IssueKey { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public virtual List<FixVersion> FixVersions { get { return _fixVersions; } set { _fixVersions = value; } }
        public List<JiraIssue> IssueLinks { get; set; }
        public string Reporter { get; set; }
        public string Assignee { get; set; }
        public string NetsuiteTicketNumber { get; set; }
        public virtual List<NetsuiteApiResult> NetsuiteApiResults { get; set; }
        public string Status { get; set; }
        public virtual List<string> Comments { get; set; }
        public SoftwareType SoftwareType { get; set; }
        public JiraIssueType IssueType { get; set; }
        public DateTime? LastRefreshTime { get; set; }
        public virtual List<NetsuiteApiResult> NetsuiteTickets { get; set; }


        #region Methods

        public void UpdateFromExisting(Issue issue)
        {
            this.FixVersions = FixVersions == null || FixVersions?.Count == 0 ? new List<FixVersion>() : FixVersions; 
            this.Status = UpdateIfDifferent<string>(Status, issue.fields.status.name)?.ToString();
            this.Summary = UpdateIfDifferent<string>(Summary, issue.fields.summary)?.ToString();
            this.Reporter = UpdateIfDifferent<string>(Reporter, issue.fields.reporter.displayName)?.ToString();
            this.Assignee = UpdateIfDifferent<string>(Assignee, issue.fields.assignee?.displayName)?.ToString();
            this.NetsuiteTicketNumber = UpdateIfDifferent<string>(NetsuiteTicketNumber, issue.fields?.customfield_10080)?.ToString();
            this.IssueId = UpdateIfDifferent<string>(IssueId, issue.id).ToString();
            this.LastRefreshTime = DateTime.Now;
            this.NetsuiteTicketNumber = issue.fields.customfield_10080;
            try
            {
                var newFixVersion = (_serializer.Deserialize<FixVersion[]>(issue.fields.fixVersions)).ToList();
                var contextualFixVersions = new List<FixVersion>();
                var fixVersionCopy = this.FixVersions.ToArray();
                if (newFixVersion.Count > 0)
                {
                    foreach (FixVersion fixVersion in newFixVersion)
                    {
                        if (!_dbContext.FixVersions.Any() || (!_dbContext.FixVersions.Any(x => x.id == fixVersion.id) && !fixVersionCopy.Contains(fixVersion)))
                        {
                            _dbContext.Entry(fixVersion).State = EntityState.Added;
                            contextualFixVersions.Add(fixVersion);
                        }
                        else
                        {
                            contextualFixVersions.Add(_dbContext.FixVersions.FirstOrDefault(x => x.name == fixVersion.name));
                        }
                        try
                        {
                            _dbContext.SaveChanges();
                        }
                        catch { }

                    }
                    FixVersions.AddRange(contextualFixVersions.ToList());
                    _dbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                this.FixVersions = new List<FixVersion>(); //= _dbContext.FixVersions.Create();
            }
            
            //manage linked netsuite tickets
            //List<NetsuiteApiResult> nsTickets = new List<NetsuiteApiResult>();
            //foreach (var ticket in issue.fields.customfield_10080.Split(',').ToList())
            //{
            //    if (NetsuiteApiResults.All(x => x.columns.casenumber != ticket))
            //    {
            //        if (!_dbContext.NetsuiteTickets.Any(x => x.columns.casenumber == ticket))
            //        {
            //            NetsuiteApiResult newNsApiResult = new NetsuiteApiResult();
            //            newNsApiResult.columns = new Columns();
            //            newNsApiResult.columns.casenumber = ticket;
            //            newNsApiResult.columns.JiraIssues.Add(this);
            //            newNsApiResult.id = new Random().Next(1, 99999).ToString();
            //            _dbContext.Entry(ticket).State = EntityState.Added;
            //            nsTickets.Add(newNsApiResult);
            //        }
            //        else
            //        {
            //            NetsuiteApiResult existingApiResult = _dbContext.NetsuiteTickets.FirstOrDefault(x => x.columns.casenumber == ticket);
            //            nsTickets.Add(existingApiResult);
            //        }
            //    }
            //}
            //NetsuiteApiResults.AddRange(nsTickets);


            //manage new comments
            List<String> commentsToAdd = new List<string>();
            foreach(var comment in issue.fields.comments)
                if (Comments.All(x => comment.body.ToString() != x))
                    commentsToAdd.Add(comment.body);
            Comments.AddRange(commentsToAdd);

        }

        private object UpdateIfDifferent<T>(object leftSide, object rightSide)
        {
            dynamic objOut = typeof(T).Name == "String" ? string.Empty : Activator.CreateInstance(typeof(T).ReflectedType);
            objOut = leftSide == rightSide ? leftSide : rightSide;
            return objOut;
        }

        #endregion

        #region Constructors

        public JiraIssue()
        {
            NetsuiteApiResults = new List<NetsuiteApiResult>();
            Comments = new List<string>();
        }
        
        public JiraIssue(Issue issue)
        {
            IssueId = issue.id;
            IssueKey = issue.key;
            Comments = new List<string>();
            IssueLinks = new List<JiraIssue>();
            NetsuiteTicketNumber = issue.fields.customfield_10080;
            NetsuiteTickets = new List<NetsuiteApiResult>();

            foreach (var prop in typeof(IssueFields).GetProperties())
            {
                IEnumerable<string> thisProperty = from property in typeof(JiraIssue).GetProperties()
                                                   where String.Equals(property.Name, prop.Name, StringComparison.CurrentCultureIgnoreCase)
                                                   select property.Name;
                if (thisProperty.Any())
                {
                    try
                    {
                        this.GetType()
                            .GetProperty(thisProperty.FirstOrDefault())
                            .SetValue(this, prop.GetValue(issue.fields));
                    }
                    catch(Exception ex)
                    {
                        try
                        {
                            switch (prop.Name)
                            {
                                case "status":
                                    this.Status = issue.fields.status.name;
                                    break;
                                case "reporter":
                                    this.Reporter = issue.fields.reporter.name;
                                    break;
                                case "comments":
                                    issue.fields.comments.ForEach(c => this.Comments.Add(c.body));
                                    break;
                                case "issuelinks":
                                    issue.fields.issuelinks.ForEach(i => this.IssueLinks.Add(new JiraIssue() { IssueId = i.id }));
                                    break;
                                case "fixVersions":
                                    this.FixVersions =
                                        _serializer.Deserialize<FixVersion[]>(issue.fields.fixVersions).ToList();
                                    break;
                                case "cucustomfield_10080":
                                    foreach(var ticket in issue.fields.customfield_10080.Split(','))
                                        Console.WriteLine(ticket);
                                    break;
                                //labels?

                            }

                        }
                        catch(Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
