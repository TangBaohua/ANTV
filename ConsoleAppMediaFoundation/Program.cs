//using System.Drawing;
//using System.Runtime.InteropServices;
using LibVLCSharp.Shared;
//using LibVLCSharp;
//using System.Numerics;


namespace ConsoleAppMediaFoundation
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            Core.Initialize();
            string m3u8Url = "http://115.149.139.141:10001/tsfile/live/1023_1.m3u8";
            string snapshotPath = $"C:\\Users\\Sky\\source\\repos\\ConsoleAppNet8\\Snapshot_{DateTime.Now.ToString("yyMMddHHmmss")}.jpg";
            
             using var libVLC = new LibVLC();
            //using var libVLC = new LibVLC("--vout=dummy", "--no-audio");//不弹出视频不启用音频
            
            using var mediaPlayer = new MediaPlayer(libVLC);         
            //mediaPlayer.Media = new Media(libVLC, m3u8Url, FromType.FromLocation);
            using var media = new Media(libVLC, m3u8Url, FromType.FromLocation);



            // 增加网络缓存，单位毫秒
            media.AddOption(":network-caching=1000");

            // 增加实时流缓存，单位毫秒
            media.AddOption(":live-caching=3000");

            // 禁用时钟抖动和同步，降低流播放要求
            media.AddOption(":clock-jitter=0");
            media.AddOption(":clock-synchro=0");

            // 设置错误容忍度
            media.AddOption(":demux-filter=+ac3"); // 强制使用 AC3 解码




            mediaPlayer.Media = media;
            media.Parse(MediaParseOptions.ParseNetwork);           

            // 播放媒体
            mediaPlayer.Play();

            libVLC.Log += (sender, e) =>
            {
                Console.WriteLine($"[{e.Level}] {e.Module}: {e.Message}");
            };
            // 等待加载一帧视频
            System.Threading.Thread.Sleep(4000); // 根据流的加载速度调整时间

            // 获取视频的原生分辨率
             //int videoWidth  = mediaPlayer.Video.Width;
            //int videoHeight =  mediaPlayer.Video.Height;
            // 截取当前帧
          
            bool success = mediaPlayer.TakeSnapshot(0, snapshotPath, 640, 360); // 0 = 视频轨道，指定分辨率
            
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"截图成功！保存于{snapshotPath}");
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("截图失败！");
                Console.ForegroundColor = ConsoleColor.Black;
            }
           
             System.Threading.Thread.Sleep(200000);
            mediaPlayer.Stop();
            Console.ReadKey();
        }
 
    }
}