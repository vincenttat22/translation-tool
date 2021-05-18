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
        public IActionResult StartTranslate()
        {
            var folderName = Path.Combine("Uploads", userId);
            var pathFolder = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var path = Path.Combine(pathFolder, "test.srt");
            SrtData mySrtModelList = _srtEditor.ParseSrt(path);
            var language = "zh-CN";
            IList<TranslationResult> translatedText  = _translator.TranslateListText(mySrtModelList.srtStrings, language);
            _srtEditor.WriteSrt(mySrtModelList.SrtModels, translatedText, pathFolder);
            return Ok(new { translatedText });
        }

       

        [HttpGet]
        [Authorize]
        public IActionResult GetLanguages()
        {
            IList<Language> languages = _translator.GetLanguages();
            return Ok(languages);
        }
    }
}
