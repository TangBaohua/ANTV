using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTV.DataModel
{
    public class ChannelInfo
    {
        public string ChannelName { get; set; }
        public string ScreenshotPath { get; set; } // 图片路径
        public int Latency { get; set; } // 延迟，单位毫秒
    }
}
