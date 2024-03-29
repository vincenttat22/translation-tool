﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class FileManagement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string OriginalFileName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string FileName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string FilePath { get; set; }

        [Column(TypeName = "nvarchar(6)")]
        public string FileType { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string LanguageCode { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime LastUpdated { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public FileManagement()
        {
            this.LastUpdated = DateTime.UtcNow;
        }
    }
}
