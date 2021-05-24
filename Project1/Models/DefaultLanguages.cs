using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class DefaultLanguages
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string LanguageCode { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class DefaultLanguageParametters
    {
        public List<string> langagues { get; set; }
    }
}
