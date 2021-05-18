using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ApplicationContext _applicationContext;
        private readonly string userId = "";
        public UploadFileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
            _applicationContext = applicationContext;
            userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "UserId")?.Value;
        }

        [HttpPost]
        [Route("Subtitile")]
        [Authorize]
        public async Task<IActionResult> UploadSRT()
        {
           try
            {
                var formCollection = await Request.ReadFormAsync();
                var files = formCollection.Files;
                var folderName = Path.Combine("Uploads", userId);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                List<FilePath> filePaths = new();
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                if (files.Any(f => f.Length == 0))
                {
                    return BadRequest();
                }
                foreach (var file in files)
                {
                    var ofileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string ext = Path.GetExtension(ofileName);
                    var fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()+ext;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);
                    FileManagement fileDetail = new()
                    {
                        UserId = userId,
                        OriginalFileName = ofileName,
                        FileName = fileName,
                        FilePath = dbPath,
                        CreatedDate = DateTime.UtcNow
                    };
                    _applicationContext.FileManagement.Add(fileDetail);
                    filePaths.Add(new FilePath(ofileName, fileName, dbPath));
                }
                await _applicationContext.SaveChangesAsync();
                return Ok(new { filePaths });
            } catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
