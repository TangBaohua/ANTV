using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppNet8
{
    //#EXTINF:-1 tvg-id="10179" tvg-chno="140" tvg-name="ESPN US HD" tvg-logo="http://*/*/misc/logos/240/549.png" group-title="Eng;Sports",ESPN US HD

    internal class MediaChannel
    {
        public string? TvgName;
        public string? TvgLogo;
        public string? GroupTitle;
        public bool Radio;
        public required string MediaName;
        public required string MediaUrl;
        public int Timeout = -1;
        public bool isAvailable = false;
        public string? Snapshot;
    }
}
