using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class ExportFiles
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string OriginalFileName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string ExportedFileName { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string TranslatedFromCode { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string TranslatedToCode { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        public ExportFiles()
        {
            this.CreatedDate = DateTime.UtcNow;
        }
    }
    public class DownloadParametter
    {
        public List<int> filesIds { get; set; }
    }
}
