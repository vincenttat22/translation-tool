using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class FilePath
    {
        public FilePath(string OriginaFilelName, string FileName, string FilePathLocation)
        {
            this.OriginaFilelName = OriginaFilelName;
            this.FileName = FileName;
            this.FilePathLocation = FilePathLocation;
        }
        public string OriginaFilelName { get; set; }
        public string FileName { get; set; }
        public string FilePathLocation { get; set; }
    }
}
