using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace JiraSuite.DataAccess.Models
{
    public class Company
    {
        private string _name;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string name {
            get { return _name ?? ""; }
            set { _name = value ?? ""; }
        }
        public string internalid { get; set; }
    }
}
