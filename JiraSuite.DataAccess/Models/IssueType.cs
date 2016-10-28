namespace JiraSuite.DataAccess.Models
{
    public class IssueType
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Subtask { get; set; }
    }
}