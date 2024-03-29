﻿using Google.Cloud.Translation.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public interface ITranslator
    {
        string TranslateText(string text, string language);
        string TranslateHtml(string text, string language);
        IList<TranslationResult> TranslateListText(List<string>texts, string language);
        IList<Detection> DetectLanguages(List<string> text);
        Detection DetectLanguage(string text);
        IList<Language> GetLanguages();
    }
    public class ApiLanguage
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class Translator : ITranslator
    {
        private readonly TranslationClient client;
        public Translator()
        {
            client = TranslationClient.Create();
        }

        public IList<Language> GetLanguages() {            
            IList<Language> languages = client.ListLanguages(LanguageCodes.English);
            return languages;
        }

        public string TranslateHtml(string text, string language)
        {
            var response = client.TranslateHtml(text, language);
            return response.TranslatedText;
        }
        public string TranslateText(string text, string language)
        {
            var response = client.TranslateText(text, language);
            return response.TranslatedText;
        }

        public IList<TranslationResult> TranslateListText(List<string> text, string language)
        {
            IList<TranslationResult> results = client.TranslateText(text, language);
            return results;
        }

        public IList<Detection> DetectLanguages(List<string> text)
        {
            IList<Detection> results = client.DetectLanguages(text);
            return results;
        }

        public Detection DetectLanguage(string text)
        {
            Detection results = client.DetectLanguage(text);
            return results;
        }
    }

    public class TranslationQueue
    {
        public int FileId { get; set; }
        public string OriginalFileName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ToCode { get; set; }
        public string State { get; set; }
    }
}
