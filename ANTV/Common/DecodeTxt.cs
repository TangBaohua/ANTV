using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANTV.DataModel;

namespace ANTV.Common
{
    internal static class DecodeTxt
    {
        public static string TxtToJson(string path)
        {
            string[] TxtLines = ToStringLines(path);
            string[] TxtRes = [];

            TxtRes= TxtLines.Where(line => !String.IsNullOrWhiteSpace(line))
                    .Where(line => line.Contains("://"))
                    .Where(line => line.Contains(","))
                    .Where(line => !line.Contains(".mp4"))
                    .ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            for (int i = 0; i < TxtRes.Length; i++)
            {
                try
                {
                    sb.AppendLine("{");
                    sb.AppendLine($"\"MediaName\":\"{TxtRes[i].Split(',')[0].Trim()}\",");
                    sb.AppendLine($"\"MediaUrl\":\"{TxtRes[i].Split(',')[1].Trim()}\"");
                    sb.AppendLine("},");
                }
                catch (Exception)
                {
                    continue;
                }
            }
        
            if (sb.Length >= 2 && sb.ToString(sb.Length - 4, 2) == "},")
            {
                sb.Remove(sb.Length - 4, 4);
                sb.AppendLine("}");
            }
            sb.AppendLine("]");
            return sb.ToString();
        }

        public static List<MediaChannel> TxtToList(string path)
        {
            string[] TxtLines = ToStringLines(path);
            string[] TxtRes = [];

            TxtRes = TxtLines.Where(line => !String.IsNullOrWhiteSpace(line))
                    .Where(line => !line.Contains("://"))
                    .ToArray();

            List<MediaChannel> channels = new List<MediaChannel>();
            for (int i = 0; i < TxtRes.Length; i++)
            {
                channels.Add(new MediaChannel { MediaName = TxtRes[i].Split(',')[0], MediaUrl = TxtRes[i].Split(',')[1] });
            }
            return channels;
        }
        private static string[] ToStringLines(string path)
        {
            return System.IO.File.ReadAllLines(path, Encoding.UTF8);
        }
    }
}
