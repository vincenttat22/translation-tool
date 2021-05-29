using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class SupportLangagues
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        public SupportLangagues()
        {
            this.CreatedDate = DateTime.UtcNow;
        }
    }
    public class SupportLangagueVersion
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public string CurrentVersion { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        public SupportLangagueVersion()
        {
            this.CreatedDate = DateTime.UtcNow;
        }
    }
}
