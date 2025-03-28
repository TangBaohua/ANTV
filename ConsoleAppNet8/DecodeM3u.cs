using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleAppNet8
{
    internal static class DecodeM3u
    {
        public static string M3uToJson(string path)
        {
            string[] TxtLines = ToStringLines(path);
            string[] TxtRes = [];
            //for (int i = 0; i < TxtLines.Length; i++)//
            //{
            //    if (TxtLines[i].Length > 1 && (TxtLines[i].TrimStart().Substring(0, 7) != "#EXTINF" && TxtLines[i].TrimStart().Substring(0, 1) == "#"))
            //    {
            //        TxtRes.Append(TxtLines[i]);
            //    }
            //}
            TxtRes = TxtLines.Where(line => !string.IsNullOrWhiteSpace(line)) // 排除空行
                             .Where(line => !(line.StartsWith("#") && !line.StartsWith("#EXTINF"))) // 排除以 # 开头但不是 #EXTINF 的注释行
                             .ToArray();

            List<RadioStation> stations = new List<RadioStation>();
            for (int i = 0; i < TxtRes.Length; i++)
            {
                if (TxtRes[i].StartsWith("#EXTINF"))
                {
                    string metadataline = TxtRes[i];
                    string streamUrl = (i + 1 < TxtRes.Length && !TxtRes[i + 1].StartsWith("#EXTINF")) ? TxtRes[i + 1] : null;

                    RadioStation station = ParseExtInf(metadataline, streamUrl);
                    if (station != null) { stations.Add(station); }
                }
            }

            //foreach (var station in stations)
            //{
            //    Console.WriteLine($"Name: {station.Name}");
            //    Console.WriteLine($"Logo: {station.Logo}");
            //    Console.WriteLine($"Group: {station.Group}");
            //    Console.WriteLine($"Radio: {station.Radio}");
            //    Console.WriteLine($"DisplayName: {station.DisplayName}");
            //    Console.WriteLine($"Stream URL: {station.StreamUrl}");
            //    Console.WriteLine("-------------------------");
            //}
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            foreach (var station in stations)
            {
                sb.AppendLine("{");
                sb.AppendLine($"\"TvgName\":\"{station.Name.Trim()}\",");
                sb.AppendLine($"\"MediaName\":\"{station.DisplayName.Trim()}\",");
                sb.AppendLine($"\"MediaUrl\":\"{station.StreamUrl}\",");
                sb.AppendLine($"\"TvgLogo\":\"{station.Logo}\",");
                sb.AppendLine($"\"GroupTitle\":\"{station.Group}\",");
                sb.AppendLine($"\"Radio\":\"{station.Radio}\"");
                sb.AppendLine("},");
            }
            if (sb.Length >= 2 && sb.ToString(sb.Length - 4, 2) == "},")
            {
                sb.Remove(sb.Length - 4, 4);
                sb.AppendLine("}");
            }
            sb.AppendLine("]");
            return sb.ToString();
        }

        public static List<MediaChannel> M3uToList(string path)
        {
            string[] TxtLines = ToStringLines(path);
            string[] TxtRes = [];
            TxtRes = TxtLines.Where(line => !string.IsNullOrWhiteSpace(line)) // 排除空行
                             .Where(line => !(line.StartsWith("#") && !line.StartsWith("#EXTINF"))) // 排除以 # 开头但不是 #EXTINF 的注释行
                             .ToArray();

            List<MediaChannel> channels = new List<MediaChannel>();
            for (int i = 0; i < TxtRes.Length; i++)
            {
                if (TxtRes[i].StartsWith("#EXTINF"))
                {
                    string metadataline = TxtRes[i];
                    string streamUrl = (i + 1 < TxtRes.Length && !TxtRes[i + 1].StartsWith("#EXTINF")) ? TxtRes[i + 1] : null;

                    MediaChannel channel = ParseChnExtInf(metadataline, streamUrl);
                    if (channel != null)
                    {
                        channels.Add(channel);
                    }
                }
            }

            return channels;
        }

        private static string[] ToStringLines(string path)
        {
            return File.ReadAllLines(path, Encoding.UTF8);
        }


        static RadioStation ParseExtInf(string metadataLine, string streamUrl)
        {
            try
            {
                string name = ExtractValue(metadataLine, "tvg-name");
                string logo = ExtractValue(metadataLine, "tvg-logo");
                string group = ExtractValue(metadataLine, "group-title");
                string radio_temp = ExtractValue(metadataLine, "radio");
                string radio = "";
                if (radio_temp.ToLower()== "true" || radio_temp.ToLower()== "false")
                {
                    radio = radio_temp;
                }

                string displayName = metadataLine.Split(',').Last().Trim();

                return new RadioStation
                {
                    Name = name,
                    Logo = logo,
                    Group = group,
                    Radio = string.IsNullOrEmpty(radio) ? false : bool.Parse(radio),
                    DisplayName = displayName,
                    StreamUrl = streamUrl
                };
            }
            catch
            {
                return null; // 如果解析失败，返回 null
            }
        }

        static MediaChannel ParseChnExtInf(string metadataLine, string streamUrl)
        {
            try
            {
                string name = ExtractValue(metadataLine, "tvg-name");
                string logo = ExtractValue(metadataLine, "tvg-logo");
                string group = ExtractValue(metadataLine, "group-title");
                string radio = ExtractValue(metadataLine, "radio");
                string displayName = metadataLine.Split(',').Last().Trim();
                /*
                        public string? TvgName;
                        public string? TvgLogo;
                        public string? GroupTitle;
                        public bool Radio;
                        public required string MediaName;
                        public required string MediaUrl;
                        public int Timeout = -1;
                        public bool isAvailable = false;
                        public string? Snapshot;
                 */
                return new MediaChannel
                {
                    MediaName = displayName,
                    MediaUrl = streamUrl
                };
            }
            catch
            {
                return null; // 如果解析失败，返回 null
            }
        }

        static string ExtractValue(string line, string key)
        {
            string pattern = $"{key}=\"";
            int startIndex = line.IndexOf(pattern);
            if (startIndex == -1) return "";

            startIndex += pattern.Length;
            int endIndex = line.IndexOf("\"", startIndex);
            return line.Substring(startIndex, endIndex - startIndex);
        }

        // 定义数据结构
        class RadioStation
        {
            public string? Name { get; set; } = "";
            public string? Logo { get; set; } = "";
            public string? Group { get; set; } = "";
            public bool Radio { get; set; } = false;
            public string? DisplayName { get; set; } = "";
            public string StreamUrl { get; set; }
        }

    }
}
