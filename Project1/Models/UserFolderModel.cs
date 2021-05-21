using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class UserFolderModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserFolderModel> Children { get; set; }
        public List<UserFileModel> Files { get; set; }
    }

    public class UserFileModel
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string LanguageCode { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
