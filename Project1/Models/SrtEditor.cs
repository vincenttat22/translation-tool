using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Models
{
    public interface ISrtEditor
    {
        SrtData ParseSrt(string path);
        string GetFirstLine(string srtPath);
        string WriteSrt(List<SrtModel> srtContent, IList<TranslationResult> translationResults, string srtPath);
    }

    public class SrtModel
    {
        public string Index { get; set; }
        public string SrtTime { get; set; }
        public string SrtString { get; set; }
    }

    public class SrtData {
        public List<SrtModel> SrtModels { get; set; }
        public List<string> srtStrings { get; set; }
    }

    public class SrtEditor : ISrtEditor
    {
        
        public SrtData ParseSrt(string srtPath)
        {
            List<SrtModel> mySrtModelList = new List<SrtModel>();
            List<string> srtStrings = new();
            SrtData srtData = new();
            string line;
            using (FileStream fs = new FileStream(srtPath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    StringBuilder sb = new StringBuilder();
                    while ((line = sr.ReadLine()) != null)
                    {

                        if (!line.Equals(""))
                        {
                            sb.Append(line).Append("@");
                            continue;
                        }

                        string[] parseStrs = sb.ToString().Split('@');
                        if (parseStrs.Length < 3)
                        {
                            sb.Remove(0, sb.Length);// Clear, otherwise it will affect the analysis of the next subtitle element</i>  
                            continue;
                        }

                        SrtModel srt = new();
                        srt.Index = parseStrs[0];
                        srt.SrtTime = parseStrs[1];

                        string strBody = null;
                        for (int i = 2; i < parseStrs.Length; i++)
                        {
                            strBody += parseStrs[i];
                        }
                        srt.SrtString = strBody;
                        mySrtModelList.Add(srt);
                        srtStrings.Add(strBody);
                        sb.Remove(0, sb.Length);
                    }
                    srtData.SrtModels = mySrtModelList;
                    srtData.srtStrings = srtStrings;

                }

            }
            return srtData;
        }

        public string GetFirstLine(string srtPath)
        {
            string line;
            string strBody = null;
            using (FileStream fs = new FileStream(srtPath, FileMode.Open))
            {
                using StreamReader sr = new StreamReader(fs, Encoding.Default);       
                StringBuilder sb = new StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.Equals(""))
                    {
                        sb.Append(line).Append("@");
                    }
                    else
                    {
                        string[] parseStrs = sb.ToString().Split('@');
                        if (parseStrs.Length < 3)
                        {
                            sb.Remove(0, sb.Length);// Clear, otherwise it will affect the analysis of the next subtitle element</i>  
                        }
                        else
                        {
                            for (int i = 2; i < parseStrs.Length; i++)
                            {
                                strBody += parseStrs[i];
                            }
                            break;
                        }
                    }
                }
            }
        
            return strBody;
        }
        public string WriteSrt(List<SrtModel> srtContent, IList<TranslationResult> translationResults, string srtPath)
        {
            var fileName = Path.Combine(srtPath,DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + ".srt");

            using (StreamWriter outputFile = new StreamWriter(fileName, true))
            {
                foreach (var srt in srtContent.Select((val, i) => (val, i)))
                {
                    outputFile.WriteLine(srt.val.Index);
                    outputFile.WriteLine(srt.val.SrtTime);
                    outputFile.WriteLine(translationResults[srt.i].TranslatedText);
                    outputFile.WriteLine(Environment.NewLine);
                }
            }
            return fileName;
        }
    }
}
