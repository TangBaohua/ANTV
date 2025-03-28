using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppNet8
{
    internal static class GuessTest
    {
        internal static List<MediaChannel> test()
        {
            string example_ysp = "http://mobilelive-timeshift.ysp.cctv.cn/timeshift/ysp/2013693901/timeshift.m3u8?delay=0";
            /*
             CCTV6   :2013693901
             CETV1   :2022823801
             兵团卫视：2022606701
             */
            List<MediaChannel> channels = new List<MediaChannel>();
            string No = "";
            string tempUrl = "";
            //int lineCount = 0;
            for (int i = 10; i <= 25; i++)
            {
                //  No= i.ToString("D4");
                // Console.WriteLine(No);
                for (int j = 6000; j <= 9000; j++)
                {
                    No = "20" + i.ToString() + j.ToString() + "01";
                    tempUrl = "http://mobilelive-timeshift.ysp.cctv.cn/timeshift/ysp/" + No + "/timeshift.m3u8?delay=0";
                    MediaChannel tempChannel = new MediaChannel { MediaName = No, MediaUrl = tempUrl };
                    channels.Add(tempChannel);



                    //Console.WriteLine(tempUrl);
                    //lineCount++;
                    //if (lineCount > 10)
                    //{
                    //    lineCount = 0;
                    //    Console.WriteLine();
                    //}
                }
            }
            return channels;
        }

        internal static List<MediaChannel> test2()
        {
            string example_2 = "http://115.149.139.141:10001/tsfile/live/1022_1.m3u8";
            List<MediaChannel> channels = new List<MediaChannel>();
            string No = "";
            string tempUrl = "";
            //int lineCount = 0;
            for (int i = 2000; i <= 2100; i++)
            {
                tempUrl = "http://115.149.139.141:10001/tsfile/live/" + i.ToString() + "_1.m3u8";
                MediaChannel tv = new MediaChannel { MediaName = i.ToString(), MediaUrl = tempUrl };
                channels.Add(tv);
            }
            return channels;
        }
        //
        internal static List<MediaChannel> test3()
        {
            string example_2 = "http://218.76.32.193:9901/tsfile/live/1019_1.m3u8";
            List<MediaChannel> channels = new List<MediaChannel>();
            string No = "";
            string tempUrl = "";
            //int lineCount = 0;
            for (int i = 0; i <= 9999; i++)
            {
                tempUrl = "http://218.76.32.193:9901/tsfile/live/" + i.ToString("D4") + "_1.m3u8";
                MediaChannel tv = new MediaChannel { MediaName = i.ToString(), MediaUrl = tempUrl };
                channels.Add(tv);
            }
            return channels;
        }
        
            internal static List<MediaChannel> test4()
        {
            string example = "http://61.221.215.25:8800/hls/50/index.m3u8";
            List<MediaChannel> channels = new List<MediaChannel>();
            string No = "";
            string tempUrl = "";
            //int lineCount = 0;
            for (int i = 1; i <= 99; i++)
            {
                tempUrl = "http://61.221.215.25:8800/hls/" + i.ToString("D2") + "/index.m3u8";
                MediaChannel tv = new MediaChannel { MediaName = i.ToString(), MediaUrl = tempUrl };
                channels.Add(tv);
            }
            return channels;
        }
    }
}
