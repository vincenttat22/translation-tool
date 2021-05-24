using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadFIleController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationContext _applicationContext;
        private readonly string userId = "";
       
        public DownloadFIleController(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _applicationContext = applicationContext;
            userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "UserId")?.Value;      
        }

        [HttpPost]
        [Authorize]
        public IActionResult DownloadSRT(DownloadParametter paras)
        {
            var files = _applicationContext.FileManagement.Where(x => paras.filesIds.Contains(x.Id)).ToArray();
            using var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach(var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.OriginalFileName) + (file.FileType == "input" ? ( "_" + file.LanguageCode ) : "" ) + "_" + file.FileName;
                    var fileInArchive = zipArchive.CreateEntry(fileName, CompressionLevel.Optimal);
                    using var entryStream = fileInArchive.Open();
                    byte[] bytes = FileToByteArray(file.FilePath);
                    using var fileToCompressStream = new MemoryStream(bytes);
                    fileToCompressStream.CopyTo(entryStream);
                }
            }
            return File(memoryStream.ToArray(), "application/zip", "srt_files.zip");
        }

        public static byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null;

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                var binaryReader = new BinaryReader(fs);
                fileData = binaryReader.ReadBytes((int)fs.Length);
            }
            return fileData;
        }
    }
}
