using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JiraSuite.DataAccess.Models
{
    public class NetsuiteTicket
    {
        [Key]
        public int NetsuiteTicketId { get; set; }
        public string Title { get; set; }

        public virtual List<JiraIssue> JiraIssues { get; set; }
    }
}