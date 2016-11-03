using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraSuite.DataAccess.EntityFramework;
using TechTalk.JiraRestClient;
using static JiraSuite.DataAccess.Models.JiraTypeHelper;

namespace JiraSuite.DataAccess.Models
{
    public class JiraIssue
    {
        
        public string IssueId { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string IssueKey { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public List<JiraIssue> IssueLinks { get; set; }
        public string Reporter { get; set; }
        public string Assignee { get; set; }
        public string NetsuiteTicketNumber { get; set; }
        public string Status { get; set; }
        public List<string> Comments { get; set; }
        public SoftwareType SoftwareType { get; set; }
        public JiraIssueType IssueType { get; set; }

        public DateTime? LastRefreshTime { get; set; }
        //{ }// return LastRefreshTime == DateTime.MinValue ? Convert.ToDateTime(SqlDateTime.MinValue) : LastRefreshTime; }
        //set; { }
        //}
        public virtual List<NetsuiteApiResult> NetsuiteTickets { get; set; }

        #region Constructors

        public JiraIssue() { }
        
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
                                                   where property.Name.ToLower() == prop.Name.ToLower()
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
