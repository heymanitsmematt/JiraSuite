using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraSuite.DataAccess.Models
{
    public class NetsuiteApiResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string id { get; set; }
        public string recordtype { get; set; }
        public virtual Columns columns { get; set; }
        
    }
}