using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translation.V2;
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
using System.Text;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ApplicationContext _applicationContext;
        private readonly string userId = "";
        private readonly ITranslator _translator;
        private readonly ISrtEditor _srtEditor;

        public TranslationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor, ApplicationContext applicationContext, ITranslator translator, ISrtEditor srtEditor)
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
        public async Task<ActionResult> StartTranslate(TranslationQueue queue)
        {
            var file = await _applicationContext.FileManagement.FindAsync(queue.FileId);
            SrtData mySrtModelList = _srtEditor.ParseSrt(file.FilePath);
            var folderName = Path.GetDirectoryName(file.FilePath);
            IList<TranslationResult> translatedText = _translator.TranslateListText(mySrtModelList.srtStrings, queue.ToCode);
            var dbPath = _srtEditor.WriteSrt(mySrtModelList.SrtModels, translatedText, folderName);
            var fileName = Path.GetFileName(dbPath);
            var OriginalFileName = Path.GetFileNameWithoutExtension(queue.OriginalFileName) + "_" + queue.ToCode + Path.GetExtension(queue.OriginalFileName);
            FileManagement fileManagement = new()
            {
                UserId = userId,
                OriginalFileName = OriginalFileName,
                FileName = fileName,
                FilePath = dbPath,
                FileType = "output",
                LanguageCode = queue.ToCode,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };
            _applicationContext.FileManagement.Add(fileManagement);
            await _applicationContext.SaveChangesAsync();
            return Ok(fileManagement);
        }
  

        [HttpGet]
        [Authorize]
        [Route("GetLanguages")]
        public IActionResult GetLanguages()
        {
            IList<Language> languages = _translator.GetLanguages();
            return Ok(languages);
        }

        [HttpGet]
        [Authorize]
        [Route("GetDefaultLanguages")]
        public IActionResult GetDefaultLanguages()
        {
            IList<string> languages = _applicationContext.DefaultLanguages.Where(x => x.UserId == userId).Select(x => x.LanguageCode).ToList();
            return Ok(languages);
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateDefaultLanguages")]
        public async Task<ActionResult> UpdateDefaultLanguages(DefaultLanguageParametters paras)
        {
            var languages = _applicationContext.DefaultLanguages.Where(x => x.UserId == userId);
            var languagesStringArr = languages.Select(x => x.LanguageCode).ToList();
            var ResponseTxt = "";
            if (!paras.langagues.SequenceEqual(languagesStringArr))
            {
                _applicationContext.DefaultLanguages.RemoveRange(languages);
                foreach(var lang in paras.langagues)
                {
                    _applicationContext.DefaultLanguages.Add(new DefaultLanguages()
                    {
                        UserId = userId,
                        LanguageCode = lang
                    });
                }
                await _applicationContext.SaveChangesAsync();
                ResponseTxt = "updated";
            }
            return Ok(new { ResponseTxt });
        }
    }
}
