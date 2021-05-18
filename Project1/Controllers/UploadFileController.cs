using Google.Cloud.Translation.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ITranslator _translator;
        private readonly ISrtEditor _srtEditor;
        public UploadFileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor, ApplicationContext applicationContext, ITranslator translator, ISrtEditor srtEditor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
            _applicationContext = applicationContext;
            userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "UserId")?.Value;
            _translator = translator;
            _srtEditor = srtEditor;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UploadSRT()
        {
           try
            {
                var formCollection = await Request.ReadFormAsync();
                var files = formCollection.Files;
                var folderName = Path.Combine("Uploads", userId);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                List<FileManagement> fileDetails = new();
                List<string> texts = new();
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
                    stream.Close();
                    string text = _srtEditor.GetFirstLine(dbPath);
                    texts.Add(text);
                    fileDetails.Add(new()
                    {
                        UserId = userId,
                        OriginalFileName = ofileName,
                        FileName = fileName,
                        FilePath = dbPath,
                        FileType = "input",
                        CreatedDate = DateTime.UtcNow
                    });
                }
                IList<Detection> detectedLanguges = _translator.DetectLanguages(texts);
                foreach (var srt in fileDetails.Select((val, i) => (val, i)))
                {
                    srt.val.LanguageCode = detectedLanguges[srt.i].Language;
                    _applicationContext.FileManagement.Add(srt.val);
                }
                await _applicationContext.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        [Route("GetInputFiles")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FileManagement>>> GetInputFiles()
        {          
            return await _applicationContext.FileManagement.Where(x => x.FileType == "input").ToListAsync();
        }
    }
}
