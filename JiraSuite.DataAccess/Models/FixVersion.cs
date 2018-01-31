using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraSuite.DataAccess.Models
{
    public class FixVersion
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string InternalId { get; set; }
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public bool archived { get; set; }
        public bool released { get; set; }
        public DateTime? releaseDate { get; set; }
    }
}