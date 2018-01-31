using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraSuite.DataAccess.Models
{
    public class Columns
    {
        private List<JiraIssue> _jiraIusIssues = new List<JiraIssue>();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string casenumber { get; set; }
        public string title { get; set; }
        public virtual Company company { get; set; }
        public virtual Stage stage { get; set; }
        public virtual Status status { get; set; }
        public virtual Profile profile { get; set; }
        public string startdate { get; set; }
        public string createddate { get; set; }
        public virtual Category category { get; set; }
        public virtual Assigned assigned { get; set; }
        public virtual Priority priority { get; set; }
        public bool helpdesk { get; set; }
        public bool custevent10 { get; set; }
        public bool custevent27 { get; set; }
        public virtual List<JiraIssue> JiraIssues { get { return _jiraIusIssues; } set { _jiraIusIssues = value; } }
        public virtual Contact contact { get; set; }
        public string escalatedto { get; set; }
    }
}