using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleAppNet8
{
    public class Publisher
    {
        public delegate void Notify(string message);
        public event Notify OnNotify;

        public void Publish(string message)
        {
            OnNotify?.Invoke(message);
        }
    }
    internal class Program
    {
        public void dalayExe(Action actionx,int mill) 
        {
            Task.Delay(mill).ContinueWith(x => actionx);
        }
        public delegate void Notifyxxx(string message);
        public event Notifyxxx OnNotify;

        public void Publish(string message)
        {
            OnNotify?.Invoke(message);
        }

        public class B { }
        public class D:B { }

        static async Task Main(string[] args)
        {

            object o2=new B();
            B b2 = new D();
            B b3 = new D();
            //B b4 = new Object();
            string strTemp = "abcdefg某某某";
            int i = System.Text.Encoding.Default.GetBytes(strTemp).Length;
            int j = strTemp.Length;
            //i:16  j:10
            float f = -123.567F; 
            int ix = (int)f;


            // 使用示例
            var publisher = new Publisher();
            publisher.OnNotify += msg => Console.WriteLine($"Subscriber 1: {msg}");
            publisher.OnNotify += msg => Console.WriteLine($"Subscriber 2: {msg}");
            publisher.Publish("New Article Published!");






            #region 多线程测试
            // List<Task> tss = new List<Task>();
            // tss.Add(Task.Run(() =>
            // {
            //     Thread.Sleep(5000);
            //     Console.WriteLine("B1-" + Environment.CurrentManagedThreadId + "#" + Thread.CurrentThread.ManagedThreadId);
            // }));
            // tss.Add(Task.Run(() =>
            // {
            //     Thread.Sleep(4000);
            //     Console.WriteLine("B2-" + Environment.CurrentManagedThreadId + "#" + Thread.CurrentThread.ManagedThreadId);
            // }));
            // tss.Add(Task.Run(() =>
            // {
            //     Thread.Sleep(3000);
            //     Console.WriteLine("B3-" + Environment.CurrentManagedThreadId + "#" + Thread.CurrentThread.ManagedThreadId);
            // }));
            // Task.WhenAll(tss).ContinueWith(x =>
            // {
            //     Console.WriteLine("中太中央万岁");
            // });

            // Task.Run(() =>
            //{

            //    Thread.Sleep(5000);
            //    Console.WriteLine("A1:" + Environment.CurrentManagedThreadId);
            //});


            // Thread.Sleep(100);
            // Console.WriteLine("A2:" + Environment.CurrentManagedThreadId);

            // Thread.Sleep(3000);
            // Console.WriteLine("A3:" + Environment.CurrentManagedThreadId);

            // Console.ReadKey();
            #endregion

            var people = new List<string> { "John", "Alice", "Bob" };
            people.Sort((x, y) => x.Length.CompareTo(y.Length));
            people.ForEach(Console.WriteLine);

            await Task.Delay(20000);


            int timeout = 5000; // 设置超时为5秒

            //string jsonPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data\\DataJson.json");
            //string jsonContent = File.ReadAllText(jsonPath);

            string m3uPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data\\playlist_2024-11-18_1457江西景德镇电信.m3u");
            string txtPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data\\iptv.txt");

            string jsonM3u = DecodeM3u.M3uToJson(m3uPath);
            //string jsonTxt = DecodeTxt.TxtToJson(txtPath);

            List<MediaChannel> ChannelList = JsonConvert.DeserializeObject<List<MediaChannel>>(jsonM3u);
            //List<MediaChannel> ChannelList = GuessTest.test4();//用于猜测

            /*1、按顺序输出，较慢
            foreach (var item in ChannelList)
            {
                int intOutTime = await CheckStreamAvailabilityAsync(item.MediaUrl, timeout);
                if (intOutTime >= 0)
                {

                    await ShowAsyncTxt($"{item.MediaName} 有效，延时{intOutTime}毫秒");
                   // Console.WriteLine($"{item.MediaName} 有效，延时{intOutTime}毫秒");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    await ShowAsyncTxt($"{item.MediaName} 无效！");
                    // Console.WriteLine($"{item.MediaName} 无效！");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }*/


            var semaphore = new SemaphoreSlim(1);//信号量
            //2、延时低先输出，较快
            /*   */
            var tasks = ChannelList.Select(async item => new
            {
                MadiaChannel = item,
                DelayTime = await CheckStreamAvailabilityAsync(item.MediaUrl, timeout, ChannelList.Count)
            }).ToList();
            int ChannelCount = tasks.Count;
            int AvailabilityCount = 0;
            Console.WriteLine();
            while (tasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);

                var result = await completedTask;
                if (result.DelayTime >= 0)
                {

                    AvailabilityCount++;
                    result.MadiaChannel.isAvailable = true;
                  
                    Console.WriteLine($"{result.MadiaChannel.MediaName} 有效，延时{result.DelayTime}毫秒");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{result.MadiaChannel.MediaName} 无效！");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            double AvailableRate = (double)AvailabilityCount / ChannelCount;
            Console.WriteLine($"共{ChannelCount}个频道，其中有效{AvailabilityCount}个，有效率{AvailableRate.ToString("P2")}");

            List<MediaChannel> AvailabilityChannels = ChannelList.Where(x => x.isAvailable).ToList();
            Console.WriteLine("有效频道：" + AvailabilityChannels.Count);

            //选择有效链接保存
            //if (AvailabilityChannels.Count > 0) { SaveAvailableM3u(AvailabilityChannels); }


            /*3、排序，延迟最低的最先输出
            // 创建一个任务列表，每个任务检查一个 MediaChannel 的可用性
            var tasks = ChannelList.Select(async item => new
            {
                MediaChannel = item,
                DelayTime = await CheckStreamAvailabilityAsync(item.MediaUrl, timeout: timeout)
            }).ToList();

            // 并行执行所有任务
            var results = await Task.WhenAll(tasks);

            // 按延时从低到高排序
            var sortedResults = results.OrderBy(r => r.DelayTime);

            // 输出排序后的结果
            foreach (var result in sortedResults)
            {
                if (result.DelayTime >= 0)
                {
                    Console.WriteLine($"{result.MediaChannel.MediaName} 有效，延时 {result.DelayTime} 毫秒");
                }
                else
                {
                    Console.WriteLine($"{result.MediaChannel.MediaName} 无效！");
                }
            }
            */





            //var responseTime = await CheckStreamAvailabilityAsync(url, timeout);
            //if (responseTime >= 0)
            //{
            //    string outputPath = Path.Combine(Path.GetTempPath(), $"Snapshot_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.jpg");
            //    object result = await CaptureSnapshotAsync(url, outputPath);
            //    dynamic dynmicResult = result;

            //    //string CaptureResult;
            //    //if (dynmicResult.code==1)
            //    //{
            //    //    CaptureResult = ($"截图成功，保存于{outputPath},分辨率为：{dynmicResult.text}");
            //    //}
            //    //else
            //    //{
            //    //    CaptureResult = ("截图失败！");
            //    //}
            //    //Console.WriteLine($"该流可用，超时: {responseTime} ms,{CaptureResult}");
            //}
            //else
            //{
            //    Console.WriteLine("该节目源超时，不可用。");
            //}


            //等待所有任务完成后再执行
            /*
            var semaphore = new SemaphoreSlim(5); // 限制并发任务数量为 5

            var tasks = ChannelList.Select(async item =>
            {
                await semaphore.WaitAsync(); // 等待信号量
                try
                {
                    return new
                    {
                        MadiaChannel = item,
                        DelayTime = await CheckStreamAvailabilityAsync(item.MediaUrl, timeout, ChannelList.Count)
                    };
                }
                finally
                {
                    semaphore.Release(); // 释放信号量
                }
            }).ToList();

            // 等待所有任务完成
            var results = await Task.WhenAll(tasks);

            int ChannelCount = results.Length;
            int AvailabilityCount = 0;
            Console.WriteLine();
            foreach (var result in results)
            {
                if (result.DelayTime >= 0)
                {
                    AvailabilityCount++;
                    result.MadiaChannel.isAvailable = true;

                    Console.WriteLine($"{result.MadiaChannel.MediaName} 有效，延时{result.DelayTime}毫秒");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{result.MadiaChannel.MediaName} 无效！");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            double AvailableRate = (double)AvailabilityCount / ChannelCount;
            Console.WriteLine($"共{ChannelCount}个频道，其中有效{AvailabilityCount}个，有效率{AvailableRate.ToString("P2")}");

            List<MediaChannel> AvailabilityChannels = ChannelList.Where(x => x.isAvailable).ToList();
            Console.WriteLine("有效频道：" + AvailabilityChannels.Count);
            */
            Console.ReadKey();
        }

        //把有效链接写入到新M3U文件
        private static void SaveAvailableM3u(List<MediaChannel> lists)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#EXTM3U\r\n");
            foreach (var item in lists)
            {
                if (item.Radio)
                {
                    sb.Append($"#EXTINF:-1 tvg-name=\"{item.TvgName}\" tvg-logo=\"{item.TvgLogo}\" group-title=\"{item.GroupTitle}\" radio=\"{item.Radio.ToString().ToLower()}\",{item.MediaName}\r\n");
                }
                else
                {
                    sb.Append($"#EXTINF:-1 tvg-name=\"{item.TvgName}\" tvg-logo=\"{item.TvgLogo}\" group-title=\"{item.GroupTitle}\",{item.MediaName}\r\n");
                }
                sb.Append($"{item.MediaUrl}\r\n");
            }
            string filePath = @"C:\Users\Sky\source\repos\ConsoleAppNet8\ConsoleAppNet8\Data\playlist.m3u";
            string newPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, $"Data\\playlist_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.m3u");
            File.WriteAllText(newPath, sb.ToString(), Encoding.UTF8);
        }

        static async Task ShowAsyncTxt(string res)
        {
            await Task.Run(() => Console.WriteLine(res));
        }

        static int i = 0;
        static async Task<int> CheckStreamAvailabilityAsync(string url, int timeout, int channelCount)
        {

            //Console.Write("*");
            //绘制进度条进度
            //Console.SetCursorPosition((int)Math.Round(i / ((double)channelCount / 50)), 0);//设置光标位置,参数为第几列和第几行
            //Console.BackgroundColor = ConsoleColor.Yellow;//设置进度条颜色
            //Console.Write(" ");//移动进度条
            //Console.BackgroundColor = ConsoleColor.Black;//恢复输出颜色

            ////更新进度百分比,原理同上.
            //Console.SetCursorPosition(53, 0);
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.Write(((double)i / channelCount).ToString("P2"));
            //Console.ForegroundColor = ConsoleColor.Gray;



            #region 绘制进度条进度
            //绘制进度条进度
            Console.BackgroundColor = ConsoleColor.Yellow;//设置进度条颜色
            if (channelCount < 50)
            {
                int x = (int)Math.Round((double)50 / channelCount);//用于小于进度条方块数单个长度的计算
                string Bar = " ";
                for (int u = 0; u < x; u++)
                {
                    Bar += " ";
                }
                for (int k = 0; k < i; k++)
                {
                    Console.SetCursorPosition((int)Math.Ceiling(k / ((double)channelCount / 50)), 0);//设置光标位置,参数为第几列和第几行
                    Console.Write(Bar);//移动进度条
                }
            }
            else
            {
                Console.SetCursorPosition((int)Math.Round(i / ((double)channelCount / 50)), 0);//设置光标位置,参数为第几列和第几行
                Console.Write(" ");//移动进度条
            }

            i++;
            Console.BackgroundColor = ConsoleColor.Black;//恢复输出颜色
            //更新进度百分比,原理同上.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(52, 0);
            Console.Write(((double)i / channelCount).ToString("P2"));
            Console.ForegroundColor = ConsoleColor.Gray;

            #endregion


            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using (var httpClient = new HttpClient(handler))
            {
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                try
                {
                    var stopwatch = Stopwatch.StartNew();// 开始计时                   
                    var response = await httpClient.GetAsync(url);// 发送请求                   
                    stopwatch.Stop();// 停止计时
                    if (response.IsSuccessStatusCode)
                    {

                        string outputPath = Path.Combine(Environment.CurrentDirectory, $"Snapshot_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.jpg");
                        //object ImgResult = await CaptureSnapshotAsync(url, outputPath);

                        return (int)stopwatch.ElapsedMilliseconds;// 返回响应时间（毫秒）
                    }
                    else
                    {
                        return -1; // 返回 -1 表示请求失败
                    }
                }
                catch (TaskCanceledException)
                {
                    // 超时处理                   
                    return -1;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Error: {ex.Message}");
                    return -1;
                }
            }
        }

        static async Task<object> CaptureSnapshotAsync(string url, string outputPath)
        {
            var ffmpegPath = "ffmpeg"; // 确保 ffmpeg 已添加到环境变量

            // FFmpeg 命令
            var arguments = $"-i \"{url}\" -frames:v 1 -q:v 2 \"{outputPath}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                string error = process.StandardError.ReadToEnd(); // 读取错误输出
                await process.WaitForExitAsync();
                string resulution;
                // 使用正则表达式匹配分辨率信息
                Match match = Regex.Match(error, @"(\d{2,4})x(\d{2,4})");
                if (match.Success)
                {
                    resulution = $"{match.Groups[1].Value}x{match.Groups[2].Value}";
                }
                else
                {
                    resulution = "";
                }

                // 检查是否成功创建了图像文件
                int CaptureResult = File.Exists(outputPath) ? 1 : 0;
                return new { code = CaptureResult, text = resulution };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
