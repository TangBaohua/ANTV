using System.Collections.ObjectModel;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using ANTV.DataModel;
using System.IO;
using Microsoft.Win32;
using ANTV.Common;
using Newtonsoft.Json;
using System.Threading.Channels;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System;
using System.Xaml;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;


namespace ANTV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region 阴影
        //// Constants
        //private const int DWMWA_NCRENDERING_POLICY = 2;
        //private const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;
        //private const int DWMWA_BORDER_COLOR = 34;
        //private const int DWMWA_CAPTION_COLOR = 35;
        //private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        //// Enums for DWM rendering policy
        //private enum DWMNCRENDERINGPOLICY
        //{
        //    DWMNCRP_USEWINDOWSTYLE = 0,
        //    DWMNCRP_DISABLED = 1,
        //    DWMNCRP_ENABLED = 2
        //}

        //[DllImport("dwmapi.dll", PreserveSig = true)]
        //private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        //[DllImport("dwmapi.dll")]
        //private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct Margins
        //{
        //    public Margins(int value)
        //    {
        //        Left = value;
        //        Right = value;
        //        Top = value;
        //        Bottom = value;
        //    }
        //    public int Left;
        //    public int Right;
        //    public int Top;
        //    public int Bottom;
        //}



        ////
        ////private void MainWindow_SourceInitialized(object sender, EventArgs e)
        ////{
        ////    IntPtr hwnd = new WindowInteropHelper(this).Handle;

        ////    // Enable DWM Shadow
        ////    EnableShadow(hwnd);
        ////}

        //private void EnableShadow(IntPtr hwnd)
        //{
        //    int enabled = 1;

        //    // Enable non-client area rendering for shadow
        //    DwmSetWindowAttribute(hwnd, DWMWA_NCRENDERING_POLICY, ref enabled, Marshal.SizeOf(enabled));

        //    // Optionally extend frame into client area for custom content alignment
        //    Margins margins = new Margins { Left = 1, Right = 1, Top = 1, Bottom = 1 };
        //    DwmExtendFrameIntoClientArea(hwnd, ref margins);
        //}
        ////
        #endregion

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern IntPtr GetWindowLongPtr(HandleRef hWnd, int nIndex);
        public IntPtr myHWND;
        public const int GWL_STYLE = -16;

        public static class WS
        {
            public static readonly long
            WS_BORDER = 0x00800000L,
            WS_CAPTION = 0x00C00000L,
            WS_CHILD = 0x40000000L,
            WS_CHILDWINDOW = 0x40000000L,
            WS_CLIPCHILDREN = 0x02000000L,
            WS_CLIPSIBLINGS = 0x04000000L,
            WS_DISABLED = 0x08000000L,
            WS_DLGFRAME = 0x00400000L,
            WS_GROUP = 0x00020000L,
            WS_HSCROLL = 0x00100000L,
            WS_ICONIC = 0x20000000L,
            WS_MAXIMIZE = 0x01000000L,
            WS_MAXIMIZEBOX = 0x00010000L,
            WS_MINIMIZE = 0x20000000L,
            WS_MINIMIZEBOX = 0x00020000L,
            WS_OVERLAPPED = 0x00000000L,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000L,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEBOX = 0x00040000L,
            WS_SYSMENU = 0x00080000L,
            WS_TABSTOP = 0x00010000L,
            WS_THICKFRAME = 0x00040000L,
            WS_TILED = 0x00000000L,
            WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_VISIBLE = 0x10000000L,
            WS_VSCROLL = 0x00200000L;
        }

        public IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // 获取原有样式
            // IntPtr originalStyle = GetWindowLongPtr(new HandleRef(null, hwnd), GWL_STYLE);

            // 添加新的样式
            IntPtr newStyle = new IntPtr(
                                         //originalStyle.ToInt64() |
                                         WS.WS_CAPTION |
                                         WS.WS_CLIPCHILDREN |
                                         WS.WS_MINIMIZEBOX |
                                         WS.WS_MAXIMIZEBOX |
                                         WS.WS_SYSMENU |
                                         WS.WS_SIZEBOX);

            // 设置窗口样式
            SetWindowLongPtr(new HandleRef(null, hwnd), GWL_STYLE, newStyle);




        }




        //VLC控件官方版
        private LibVLC _libVLC;
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;
        private bool _isFullScreen = false; // 用于记录全屏状态
        private WindowState _previousWindowState;
        private WindowStyle _previousWindowStyle;



        public string DefaultImagePath { get; set; } = "C:\\Users\\Sky\\source\\repos\\ConsoleAppNet8\\ANTV\\Image\\Image\\placeholder.jpg";

        public ObservableCollection<WifiInfo> WifiList { get; set; }
        // public ObservableCollection<MediaChannel> Channels { get; set; }
        private ObservableCollection<MediaChannel> _channels;
        private ObservableCollection<MediaChannel> _channels_forSearch; // 仅存储筛选后的频道
        public ObservableCollection<MediaChannel> Channels
        {
            get => _channels_forSearch ?? _channels;
            set
            {
                if (_channels != value)
                {
                    if (_channels != null)
                        _channels.CollectionChanged -= Channels_CollectionChanged;

                    _channels = value;

                    if (_channels != null)
                        _channels.CollectionChanged += Channels_CollectionChanged;


                    OnPropertyChanged(nameof(Channels)); // 通知 UI 更新
                    OnPropertyChanged(nameof(TotalChannels)); // 更新 TotalChannels
                    OnPropertyChanged(nameof(OnLineChannels));
                    OnPropertyChanged(nameof(OffLineChannels));
                    //OnPropertyChanged(nameof(CurrentFileName));
                    OnPropertyChanged(nameof(EffectiveRate));
                    OnPropertyChanged(nameof(CheckRate));

                }
            }
        }




        public ICommand CopyToClipboardCommand { get; set; }
        public string CurrentFileName { get; set; }


        private bool _isSnapshotButtonEnabled = false;
        public bool IsSnapshotButtonEnabled
        {
            get { return _isSnapshotButtonEnabled; }
            set
            {
                _isSnapshotButtonEnabled = value;
                OnPropertyChanged(nameof(IsSnapshotButtonEnabled)); // 触发 UI 更新
            }
        }
        private void SnapshotsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // 选中时 快照按钮可用
            IsSnapshotButtonEnabled = true;
        }
        private void SnapshotsRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // 未被选中时 快照按钮不可用
            IsSnapshotButtonEnabled = false;
        }

        //搜索关键词清空按钮是否隐藏
        private Visibility _isSearchCloseButtonVisibility = Visibility.Hidden;
        public Visibility IsSearchCloseButtonVisibility
        {
            get { return _isSearchCloseButtonVisibility; }
            set
            {
                _isSearchCloseButtonVisibility = value;
                OnPropertyChanged(nameof(IsSearchCloseButtonVisibility)); // 触发 UI 更新
            }
        }

        //顶部按钮面板是否隐藏
        private Visibility _isTopButtonPanelVisibility = Visibility.Visible;
        public Visibility IsTopButtonPanelVisibility
        {
            get { return _isTopButtonPanelVisibility; }
            set
            {
                _isTopButtonPanelVisibility = value;
                OnPropertyChanged(nameof(IsTopButtonPanelVisibility)); // 触发 UI 更新
            }
        }
        //顶部按钮面板高度
        private GridLength _topButtonPanelRowHeight = new GridLength(60);
        public GridLength TopButtonPanelRowHeight
        {
            get { return _topButtonPanelRowHeight; }
            set
            {
                _topButtonPanelRowHeight = value;
                OnPropertyChanged(nameof(TopButtonPanelRowHeight)); // 触发 UI 更新
            }
        }
        //左侧按钮条 TopMargin
        private Thickness _topMargin = new Thickness(0, 0, 0, 0);
        public Thickness TopMargin
        {
            get { return _topMargin; }
            set
            {
                _topMargin = value;
                OnPropertyChanged(nameof(TopMargin)); // 通知 UI 更新
            }
        }



        // 播放按钮未选中时 顶部按钮面板显示
        private void PlayRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            IsTopButtonPanelVisibility = Visibility.Visible;
            TopButtonPanelRowHeight = new GridLength(60);
            TopMargin = new Thickness(0, 0, 0, 0);
        }
        // 播放按钮选中时 顶部按钮面板隐藏
        private void PlayRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsTopButtonPanelVisibility = Visibility.Collapsed;
            TopButtonPanelRowHeight = new GridLength(0);
            TopMargin = new Thickness(0, 60, 0, 0);
        }


        //频道总数
        public int TotalChannels => Channels?.Count ?? 0;

        private int _offLineChannels;
        public int OffLineChannels
        {
            get => _offLineChannels;
            set
            {
                if (_offLineChannels != value)
                {
                    _offLineChannels = value;
                    OnPropertyChanged(nameof(OffLineChannels));
                }
            }
        }
        //有效频道数量
        public int _onLineChannels;
        public int OnLineChannels
        {
            get => _onLineChannels;
            set
            {
                if (_onLineChannels != value)
                {
                    _onLineChannels = value;
                    OnPropertyChanged(nameof(OnLineChannels));
                }
            }
        }

        // 有效率
        public double _effectiveRate;// => TotalChannels == 0 ? 0 : (double)OnLineChannels / TotalChannels;
        public double EffectiveRate
        {
            get => _effectiveRate;
            set
            {
                if (_effectiveRate != value)
                {
                    _effectiveRate = value;
                    OnPropertyChanged(nameof(EffectiveRate));
                }
            }
        }
        //检测率
        public double _checkRate;
        public double CheckRate
        {
            get => _checkRate;
            set
            {
                if (_checkRate != value)
                {
                    _checkRate = value;
                    OnPropertyChanged(nameof(CheckRate));
                }
            }
        }

        //当前播放频道
        private MediaChannel _currentPlayChannel;
        public MediaChannel CurrentPlayChannel
        {
            get => _currentPlayChannel;
            set
            {
                if (_currentPlayChannel != value)
                {
                    _currentPlayChannel = value;
                    OnPropertyChanged(nameof(CurrentPlayChannel));
                }
            }
        }

        //Console.WriteLine($"共{ChannelCount}个频道，其中有效{AvailabilityCount}个，有效率{AvailableRate.ToString("P2")}");

        private readonly System.Timers.Timer _updateTimer;

        public MainWindow()
        {


            OnLineChannels = 0; // 初始值

            InitializeComponent();
            Loaded += MainWindow_Loaded;
            StateChanged += MainWindowStateChangeRaised;
            //Loaded += MainWindow_SourceInitialized;//阴影
            // string prePath = "C:\\Users\\Sky\\source\\repos\\ConsoleAppNet8\\ANTV\\Symbols\\";
             //string prePath2 = @"D:\MySpace\项目1_基本\ANTV\ANTV\Symbols\";
            string currentDirectory = System.Environment.CurrentDirectory;
            string prePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Symbols\\");
            string parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            string prePathx = System.IO.Path.Combine(System.IO.Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName, "ANTV\\Symbols\\");

            WifiList = new ObservableCollection<WifiInfo>
            {
                new WifiInfo { Name = "CCTV电视指南", GroupName="央视", Latency = 50 , Color="#008000" ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"CCTV电视指南.png"},
                new WifiInfo { Name = "CCTV1", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV1.png"},
                new WifiInfo { Name = "CCTV2", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV2.png"},
                new WifiInfo { Name = "CCTV3", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV3.png"},
                new WifiInfo { Name = "CCTV4", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV4.png"},
                new WifiInfo { Name = "CCTV5", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV5.png"},
                new WifiInfo { Name = "CCTV5+", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV5+.png"},
                new WifiInfo { Name = "CCTV6", GroupName="央视", Latency = 300, Color="#FFD700"  ,IsRadio=true,Operators="China Telecom",ScreenshotPath=prePath+"CCTV6.png"},
                new WifiInfo { Name = "CCTV7", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV7.png"},
                new WifiInfo { Name = "CCTV8", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV8.png"},
                new WifiInfo { Name = "CCTV9", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV9.png"},
                new WifiInfo { Name = "CCTV10", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV10.png"},
                new WifiInfo { Name = "CCTV11", GroupName="央视", Latency = 300, Color="#FFD700"  ,IsRadio=true,Operators="China Telecom",ScreenshotPath=prePath+"CCTV11.png"},
                new WifiInfo { Name = "CCTV12", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV12.png"},
                new WifiInfo { Name = "CCTV13", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV13.png"},
                new WifiInfo { Name = "CCTV14", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV14.png"},
                new WifiInfo { Name = "CCTV15", GroupName="央视", Latency = 150, Color="#008000"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CCTV15.png"},
                new WifiInfo { Name = "CCTV16", GroupName="央视", Latency = 300, Color="#FFD700"  ,IsRadio=true,Operators="China Telecom",ScreenshotPath=prePath+"CCTV16.png"},
                new WifiInfo { Name = "CCTV17", GroupName="央视", Latency = 300, Color="#FFD700"  ,IsRadio=true,Operators="China Telecom",ScreenshotPath=prePath+"CCTV17.png"},
                new WifiInfo { Name = "CGTN", GroupName="央视", Latency = 300, Color="#FFD700"  ,IsRadio=true,Operators="China Telecom",ScreenshotPath=prePath+"CGTN.png"},
                new WifiInfo { Name = "CGTNDocumentary", GroupName="央视", Latency = 300, Color="#FFD700"  ,IsRadio=true,Operators="China Telecom",ScreenshotPath=prePath+"CGTNDocumentary.png"},
                new WifiInfo { Name = "CETV1", GroupName="央视", Latency = 300, Color="#FFD700"  ,IsRadio=true,Operators="China Telecom",ScreenshotPath=prePath+"CETV1.png"},

                new WifiInfo { Name = "湖南卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"湖南卫视.png" },
                new WifiInfo { Name = "北京卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"北京卫视.png" },
                new WifiInfo { Name = "东方卫视", GroupName="省级", Latency = 150, Color="#FF8C00"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"东方卫视.png"},
                new WifiInfo { Name = "上海电视台", GroupName="省级", Latency = 150, Color="#FF8C00"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"上海电视台.png"},
                new WifiInfo { Name = "天津卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"天津卫视.png" },
                new WifiInfo { Name = "重庆卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"重庆卫视.png" },
                new WifiInfo { Name = "浙江卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"浙江卫视.png" },
                new WifiInfo { Name = "江苏卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"江苏卫视.png" },
                new WifiInfo { Name = "安徽卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"安徽卫视.png" },
                new WifiInfo { Name = "山东卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"山东卫视.png" },
                new WifiInfo { Name = "广东卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"广东卫视.png" },
                new WifiInfo { Name = "湖北卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"湖北卫视.png" },
                new WifiInfo { Name = "河北卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"河北卫视.png" },
                new WifiInfo { Name = "河南卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"河南卫视.png" },

                new WifiInfo { Name = "江西卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"江西卫视.png" },
                new WifiInfo { Name = "山西卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"山西卫视.png" },
                new WifiInfo { Name = "陕西卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"陕西卫视.png" },
                new WifiInfo { Name = "贵州卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"贵州卫视.png" },
                new WifiInfo { Name = "云南卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"云南卫视.png" },
                new WifiInfo { Name = "四川卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"四川卫视.png" },
                new WifiInfo { Name = "广西卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"广西卫视.png" },
                new WifiInfo { Name = "黑龙江卫视", GroupName="省级", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"黑龙江卫视.png" },

                new WifiInfo { Name = "CHC影迷电影", GroupName="数字频道", Latency = 500, Color="#FFD700"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CHC影迷电影.png"},
                new WifiInfo { Name = "CHC动作电影", GroupName="数字频道", Latency = 500, Color="#FF8C00"  ,IsRadio=false,Operators="China Telecom",ScreenshotPath=prePath+"CHC动作电影.png"},
                new WifiInfo { Name = "CHC家庭影院", GroupName="数字频道", Latency = 500, Color="#FF8C00"  ,IsRadio=false,Operators="China Telecom",ScreenshotPath=prePath+"CHC家庭影院.png"},

                new WifiInfo { Name = "凤凰卫视中文台", GroupName="港澳台", Latency = 500, Color="#FF8C00"  ,IsRadio=false,Operators="China Telecom",ScreenshotPath=prePath+"凤凰卫视.png"},
                new WifiInfo { Name = "凤凰卫视资讯台", GroupName="港澳台", Latency = 500, Color="#FFD700"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"凤凰卫视资讯台.png"},
                new WifiInfo { Name = "香港卫视", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"香港卫视.png" },
                new WifiInfo { Name = "澳门卫视", GroupName="央视", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"澳门卫视.png" },
                new WifiInfo { Name = "TVB", GroupName="港澳台", Latency = 500, Color="#FF8C00"  ,IsRadio=false,Operators="China Telecom",ScreenshotPath=prePath+"TVB.png"},
                new WifiInfo { Name = "TVB无线新闻台", GroupName="港澳台", Latency = 300, Color=" #B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"TVB无线新闻台.png"},
                new WifiInfo { Name = "明珠台", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"TVB_Pearl.png" },
                new WifiInfo { Name = "翡翠台", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"翡翠台.png" },

                new WifiInfo { Name = "台视新闻", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"台视.png" },
                new WifiInfo { Name = "中视新闻", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"中视.png" },
                new WifiInfo { Name = "华视新闻", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"华视.png" },
                new WifiInfo { Name = "公视", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"公视.png" },
                new WifiInfo { Name = "民视", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"民视.png" },
                new WifiInfo { Name = "TVBS", GroupName="港澳台", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"TVBS.png" },

                new WifiInfo { Name = "CNN", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Mobile",ScreenshotPath=prePath+"CNNWORLD.png"},
                new WifiInfo { Name = "FOX", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"FOXNEWS.png" },
                new WifiInfo { Name = "ABC", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"ABC.png" },
                new WifiInfo { Name = "CNBC", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"CNBC.png" },
                new WifiInfo { Name = "PBS", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"PBS.png" },
                new WifiInfo { Name = "NASA TV", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"NASATV.png" },
                new WifiInfo { Name = "BBC Wold News", GroupName="境外", Latency = 50 , Color="#FF8C00" ,IsRadio=false,Operators="China Telecom",ScreenshotPath=prePath+"BBC_World_News.png"},
                new WifiInfo { Name = "RT", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"RT.png" },
                new WifiInfo { Name = "半岛电视台", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"半岛电视台.png" },
                new WifiInfo { Name = "新加坡亚洲新闻台", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"新加坡亚洲新闻台.png" },

                new WifiInfo { Name = "国家地理频道", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"国家地理频道.png" },
                new WifiInfo { Name = "国家地理野生频道", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"Logo_Nat_Geo_Wild.png" },
                new WifiInfo { Name = "探索频道", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"Discovery.png" },
                new WifiInfo { Name = "动物星球频道", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"AnimalPlanetChannel.png" },
                new WifiInfo { Name = "探索亚洲", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"Discovery_Asia_logo.png" },
                new WifiInfo { Name = "史密森尼频道", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"SmithsonianChannel.png" },
                new WifiInfo { Name = "历史频道", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"HistoryChannel.png" },
                new WifiInfo { Name = "法国时尚", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"FashionTV.png" },
                new WifiInfo { Name = "HBO", GroupName="境外", Latency = 500, Color="#B22222"  ,IsRadio=false,Operators="China Unicom",ScreenshotPath=prePath+"HBO.png" },

            };
            Channels = new ObservableCollection<MediaChannel>() {
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CGTN",TvgLogo=returnTVLOGO("CGTN.png"),MediaUrl="https://english-livetx.cgtn.com/hls/yypdyyctzb_hd.m3u8", Timeout=555,isAvailable=1},
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CGTN Documentary",TvgLogo=returnTVLOGO("CGTNDocumentary.png"),MediaUrl="https://english-livetx.cgtn.com/hls/yypdjlctzb_hd.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="境外台" ,MediaName="探索亚洲频道",TvgLogo=returnTVLOGO("Discovery_Asia_logo.png"),MediaUrl="http://61.221.215.25:8800/hls/50/index.m3u8", Timeout=168, isAvailable=1},

                new MediaChannel{GroupTitle="港澳台" ,MediaName="凤凰卫视中文台",TvgLogo=returnTVLOGO("凤凰卫视中文台.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1001_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="港澳台" ,MediaName="凤凰卫视资讯台",TvgLogo=returnTVLOGO("凤凰卫视资讯台.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1038_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CCTV-1 综合",TvgLogo=returnTVLOGO("CCTV1.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1004_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CCTV-3 综艺",TvgLogo=returnTVLOGO("CCTV3.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1002_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CCTV-5 体育",TvgLogo=returnTVLOGO("CCTV5.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1006_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CCTV-6 电影",TvgLogo=returnTVLOGO("CCTV6.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1007_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CCTV-8 电视剧",TvgLogo=returnTVLOGO("CCTV8.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1009_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="央视频道" ,MediaName="CCTV-13 新闻",TvgLogo=returnTVLOGO("CCTV13.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1014_1.m3u8", Timeout=168, isAvailable=1},


                new MediaChannel{GroupTitle="省级卫视" ,MediaName="东方卫视",TvgLogo=returnTVLOGO("东方卫视2.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1031_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="省级卫视" ,MediaName="湖南卫视",TvgLogo=returnTVLOGO("湖南卫视.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1023_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="省级卫视" ,MediaName="浙江卫视",TvgLogo=returnTVLOGO("浙江卫视.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1024_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="省级卫视" ,MediaName="江苏卫视",TvgLogo=returnTVLOGO("江苏卫视.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1026_1.m3u8", Timeout=168, isAvailable=1},
                new MediaChannel{GroupTitle="省级卫视" ,MediaName="安徽卫视",TvgLogo=returnTVLOGO("安徽卫视.png"),MediaUrl="http://115.149.139.141:10001/tsfile/live/1025_1.m3u8", Timeout=168, isAvailable=1},

            };



            //官方版
            // 初始化LibVLC
            Core.Initialize();

            // 配置选项
            var libVlcOptions = new[]
            {
                "--aout=directsound", // 设置音频输出模块为 DirectSound（适用于 Windows）
                "--audio-resampler=soxr", // 使用高质量音频重采样器
                //"--no-hw-decoder",  禁用硬件解码，强制使用软件解码
                "--audio-time-stretch" // 启用音频时间伸缩（防止音频失真）
            };
            _libVLC = new LibVLC(libVlcOptions);

            _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC);


            // 将播放器与VideoView绑定
            VideoView.MediaPlayer = _mediaPlayer;

            // 加载媒体
            _mediaPlayer.Play(new Media(_libVLC, "http://61.221.215.25:8800/hls/50/index.m3u8x", FromType.FromLocation));
            DataContext = this;

            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);

            // 模拟动态更新
            // 模拟动态更新
            // 使用 System.Timers.Timer
            //_updateTimer = new System.Timers.Timer(1000);
            //_updateTimer.Elapsed += UpdateLatency;
            //_updateTimer.Start();
        }
        private string returnTVLOGO(string ImgName)
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory + "\\Symbols\\", ImgName);
        }
        private void CopyToClipboard(string mediaUrl)
        {

            Debug.WriteLine($"CopyToClipboard triggered with URL: {mediaUrl}");
            if (!string.IsNullOrEmpty(mediaUrl))
            {
                Clipboard.SetText(mediaUrl);
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterChannels(); // 调用筛选方法
            }
        }



        //private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is ListViewItem item)
        //    {
        //        item.IsSelected = true;
        //    }
        //}

        private void ListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取点击的元素
            var clickedElement = e.OriginalSource as DependencyObject;

            // 向上查找是否是 ListViewItem
            var listViewItem = VisualUpwardSearch<ListViewItem>(clickedElement);

            if (listViewItem != null)
            {
                var contextMenu = (ContextMenu)((ListView)sender).Resources["ItemContextMenu"];
                ((ListView)sender).SetValue(FrameworkElement.ContextMenuProperty, contextMenu);

                // 获取点击项的索引
                //var listView = (ListView)sender;
                //int index = listView.ItemContainerGenerator.IndexFromContainer(listViewItem);
            }
            else
            {
                //e.Handled = true;
                // 点击空白区域，清除 ContextMenu
                ((ListView)sender).ClearValue(FrameworkElement.ContextMenuProperty);
            }

            // 注意：不要设置 e.Handled = true，保留默认行为
        }

        // 工具方法：向上查找元素的父级链，直到找到指定类型
        private static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            while (source != null && !(source is T))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source as T;
            //while (source != null)
            //{
            //    if (source is T target)
            //    {
            //        return target;
            //    }

            //    // 优先从 VisualTreeHelper 获取父级
            //    DependencyObject parent = VisualTreeHelper.GetParent(source);

            //    // 如果 VisualTreeHelper 返回 null，则尝试 LogicalTreeHelper
            //    if (parent == null)
            //    {
            //        parent = LogicalTreeHelper.GetParent(source);
            //    }

            //    source = parent;
            //}
            //return null;
        }

        //双击图片 双击截图
        private void Border_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is MediaChannel channel)
            {
                string mediaName = channel.Snapshot;

                if (!string.IsNullOrEmpty(mediaName))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(mediaName) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法打开文件: {mediaName}\n错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        //右键打开 打开所在文件夹
        private void OpenFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前右键点击的项的 DataContext
            if (sender is MenuItem menuItem && menuItem.DataContext is MediaChannel channel)
            {
                string filePath = channel.Snapshot;

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    try
                    {
                        //打开文件夹但不选中当前文件
                        //string folderPath = System.IO.Path.GetDirectoryName(filePath);
                        //Process.Start(new ProcessStartInfo
                        //{
                        //    FileName = folderPath,
                        //    UseShellExecute = true,
                        //});

                        //打开文件夹但且选中当前文件
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = $"/select,\"{filePath}\"", // 使用 /select 参数选中文件
                            UseShellExecute = true,
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法打开文件夹: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("文件不存在或路径无效！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        //复制截图区播放地址
        private async void CopySnapshotUrl_Click(object sender, RoutedEventArgs e)
        {

            // 获取当前右键点击的项的 DataContext
            if (sender is MenuItem menuItem && menuItem.DataContext is MediaChannel channel)
            {
                Clipboard.SetText(channel.MediaUrl);  
            }
        }


        //重新截图
        private async void Re_Screenshot_Click(object sender, RoutedEventArgs e)
        {

            // 获取当前右键点击的项的 DataContext
            if (sender is MenuItem menuItem && menuItem.DataContext is MediaChannel channel)
            {
                //var shotx = await CaptureSnapshotAsync(channel.MediaUrl, System.IO.Path.Combine(Environment.CurrentDirectory + "\\Snapshot\\", $"{channel.MediaName}_{DateTime.Now:yyyy-MM-dd_HHmmss}.jpg"));
                string snapshotPath = System.IO.Path.Combine(Environment.CurrentDirectory + "\\Snapshot\\", $"{channel.MediaName}_{DateTime.Now:yyyy-MM-dd_HHmmss}.jpg");

                //Thread thread = new Thread(  () =>
                //{
                //    var shot = CaptureSnapshotAsync(channel.MediaUrl, snapshotPath).Result;
                //    if (shot != null)
                //    {
                //        channel.Snapshot = shot.ToString();
                //        // 必须使用 Dispatcher 来更新 UI
                //        //Application.Current.Dispatcher.Invoke(() =>
                //        //{
                //        //    channel.Snapshot = shot.ToString();
                //        //});
                //    }
                //});
                //thread.IsBackground = true; // 让线程在程序退出时自动结束
                //thread.Start();




                //_ = ... 告诉编译器我知道这个 Task 存在，但我不需要它的返回值，这样 VS 不会再警告。
                _ = Task.Run(async () =>
                {
                    var shot = await CaptureSnapshotAsync(channel.MediaUrl, snapshotPath);
                    if (!string.IsNullOrEmpty(shot))
                    {
                        Application.Current.Dispatcher.Invoke(() => channel.Snapshot = shot);
                    }
                });



                //var shot = await CaptureSnapshotAsync(channel.MediaUrl, snapshotPath);
                //if (shot != null)
                //{
                //    channel.Snapshot = shot.ToString();
                //}
            }
        }


        private void MenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var textBlock = (TextBlock)menuItem.DataContext;
            Clipboard.SetText(textBlock.Text);
        }
        private void ccav(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var textBlock = (TextBlock)menuItem.DataContext;
            Clipboard.SetText(textBlock.Text);
        }
        private bool isDragging = false; // 是否正在拖动
        private Point startPoint;        // 鼠标按下时的位置
        // 实现窗口拖动
        //private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount == 2)
        //    {
        //        ToggleWindowState();
        //    }
        //    else if (e.ButtonState == MouseButtonState.Pressed && WindowState == WindowState.Normal)
        //    {
        //        DragMove();
        //    }
        //    else if (e.ButtonState == MouseButtonState.Pressed && WindowState == WindowState.Maximized)
        //    {
        //        // 记录鼠标按下时的位置
        //        startPoint = e.GetPosition(this);
        //        isDragging = true;

        //    }
        //}
        // 鼠标移动事件
        //private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isDragging && e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        // 获取鼠标在屏幕中的位置
        //        var mousePosition = PointToScreen(e.GetPosition(this));

        //        // 恢复窗口到普通状态
        //        WindowState = WindowState.Normal;

        //        // 调整窗口位置，使鼠标位于窗口顶部中间
        //        Left = mousePosition.X - (RestoreBounds.Width / 2);
        //        Top = mousePosition.Y - (30 / 2); // 假设标题栏高度为 30

        //        // 开始拖动窗口
        //        DragMove();

        //        // 停止拖动标记
        //        isDragging = false;
        //    }
        //}

        // 鼠标左键释放事件
        //private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    isDragging = false;
        //}
        // 双击标题栏切换窗口最大化或还原
        //private void ToggleWindowState()
        //{

        //    WindowState = WindowState == WindowState.Maximized
        //        ? WindowState.Normal
        //        : WindowState.Maximized;

        //}

        //开始测试按钮
        private void StartTestButton_Click(object sender, RoutedEventArgs e)
        {
            StartTest();
        }

        //开始截图按钮
        private async void SnapshotButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                StartSnapshot();
            });
        }
        async void StartSnapshot()
        {
            //var semaphore = new SemaphoreSlim(10); // 限制同时进行的任务数为 5
            //var tasks = Channels.Where(item => item.isAvailable == 1 && !item.Radio).Select(async item =>
            //{
            //    await semaphore.WaitAsync();
            //    try
            //    {
            //        return new
            //        {
            //            MediaChannel = item,
            //            Snapshot = await CaptureSnapshotAsync(item.MediaUrl, System.IO.Path.Combine(Environment.CurrentDirectory + "\\Snapshot\\", $"{item.MediaName}_{DateTime.Now:yyyy-MM-dd_HHmmss}.jpg"))        
            //        };
            //    }
            //    finally
            //    {
            //        semaphore.Release();
            //    }
            //}).ToList();

            //foreach (var task in tasks)
            //{
            //    var result = await task;
            //    result.MediaChannel.Snapshot = result.Snapshot;
            //    await Task.Delay(10);// 增强视觉效果？
            //}
            SemaphoreSlim semaphore = new SemaphoreSlim(20); // 限制并发任务数量为 5
            //foreach (var item in Channels.Where(item => item.isAvailable == 1 && !item.Radio))
            //{
            //    await semaphore.WaitAsync();
            //    try
            //    {
            //        var shot = await CaptureSnapshotAsync(item.MediaUrl, System.IO.Path.Combine(Environment.CurrentDirectory + "\\Snapshot\\", $"{item.MediaName}_{DateTime.Now:yyyy-MM-dd_HHmmss}.jpg"));
            //        if (shot != null)
            //        {
            //            item.Snapshot = shot;
            //        }

            //    }
            //    finally
            //    {
            //        semaphore.Release();
            //    }
            //}







            //---真正的同时更新UI
            foreach (var item in Channels.Where(item => item.isAvailable == 1 && !item.Radio))
            {
                _ = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var shot = await CaptureSnapshotAsync(item.MediaUrl,
                            System.IO.Path.Combine(Environment.CurrentDirectory + "\\Snapshot\\",
                            $"{item.MediaName}_{DateTime.Now:yyyy-MM-dd_HHmmss}.jpg"));

                        if (shot != null)
                        {
                            //让 UI 线程更新
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                item.Snapshot = shot;
                            });
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }

            //虚假的并发
            //var tasks = Channels
            //    .Where(item => item.isAvailable == 1 && !item.Radio)
            //    .Select(async item =>
            //    {
            //        await semaphore.WaitAsync();
            //        try
            //        {
            //            var shot = await CaptureSnapshotAsync(item.MediaUrl, System.IO.Path.Combine(Environment.CurrentDirectory + "\\Snapshot\\", $"{item.MediaName}_{DateTime.Now:yyyy-MM-dd_HHmmss}.jpg"));
            //            if (shot != null)
            //            {
            //                item.Snapshot = shot;
            //            }
            //        }
            //        finally
            //        {
            //            semaphore.Release();
            //        }
            // });
            // await Task.WhenAll(tasks); // 等待所有截图任务完成


















        }

        //截图方法FFMmpeg
        async Task<string> CaptureSnapshotAsync(string url, string outputPath)
        {
            // 获取主程序所在目录的绝对路径
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;

            // 组合 ffmpeg 的完整路径
            var ffmpegPathLocal = System.IO.Path.Combine(exeDir, "ffmpeg.exe");
            if (!File.Exists(ffmpegPathLocal))
            {
               // MessageBox.Show("缺少必要的 ffmpeg 组件！");
                
            }

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
                string error = process.StandardError.ReadToEnd(); //读取错误输出
                await process.WaitForExitAsync();
                // 使用正则表达式匹配分辨率信息
                //string resulution;
                //Match match = Regex.Match(error, @"(\d{2,4})x(\d{2,4})");
                //if (match.Success)
                //{
                //    resulution = $"{match.Groups[1].Value}x{match.Groups[2].Value}";
                //}
                //else
                //{
                //    resulution = "";
                //}

                // 成功创建图像文件则返回路径
                return File.Exists(outputPath) ? outputPath : "";
            }
            catch (Exception ex)
            {
                return "";
            }

            //Thread.Sleep(100);
            //return outputPath;
        }

        async Task<string> CaptureSnapshotAsyncInVLC(string url, string outputPath)
        {
            Core.Initialize();

            string snapshotPath = $"C:\\Users\\Sky\\source\\repos\\ConsoleAppNet8\\Snapshot_{DateTime.Now.ToString("yyMMddHHmmss")}.jpg";

            //using var libVLC = new LibVLC();
            using var libVLC = new LibVLC("--vout=dummy", "--no-audio");//不弹出视频不启用音频

            using var mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVLC);
            //mediaPlayer.Media = new Media(libVLC, url, FromType.FromLocation);
            using var media = new Media(libVLC, url, FromType.FromLocation);
            mediaPlayer.Media = media;
            media.Parse(MediaParseOptions.ParseNetwork);

            // 播放媒体
            mediaPlayer.Play();

            // 等待加载一帧视频
            System.Threading.Thread.Sleep(5000); // 根据流的加载速度调整时间

            // 获取视频的原生分辨率
            //int videoWidth  = mediaPlayer.Video.Width;
            //int videoHeight =  mediaPlayer.Video.Height;
            // 截取当前帧

            bool success = mediaPlayer.TakeSnapshot(0, outputPath, 640, 360); // 0 = 视频轨道，指定分辨率
            ;
            if (success)
            {
                mediaPlayer.Stop();
                return outputPath;
            }
            else
            {
                mediaPlayer.Stop();
                return null;
            }
        }
        //System.Threading.Thread.Sleep(200000);

        async Task<string> CaptureSnapshotAsyncInVLC2(string url, string outputPath)
        {
            Core.Initialize();

            using var libVLC = new LibVLC("--vout=dummy", "--no-audio"); // 不显示视频、不播放音频
            using var mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVLC);
            using var media = new Media(libVLC, url, FromType.FromLocation);

            var tcs = new TaskCompletionSource<bool>();

            // 监听事件以确定视频开始正常播放
            mediaPlayer.Playing += (sender, args) =>
            {
                // 当进入播放状态时，完成任务
                tcs.TrySetResult(true);
            };

            mediaPlayer.Stopped += (sender, args) =>
            {
                // 防止因异常或停止而卡住
                tcs.TrySetResult(false);
            };

            // 设置媒体并开始播放
            mediaPlayer.Media = media;
            mediaPlayer.Play();

            // 等待视频进入播放状态
            if (await tcs.Task)
            {
                // 截图操作
                bool success = mediaPlayer.TakeSnapshot(0, outputPath, 640, 360); // 0 为视频轨道，指定分辨率
                mediaPlayer.Stop();
                return success ? outputPath : null;
            }
            else
            {
                // 播放失败或中止
                mediaPlayer.Stop();
                return null;
            }
        }


        private void ContextMenu2_Click(object sender, RoutedEventArgs e)
        {
            int s = 4;
        }




        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {

                this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;


                WindowState = WindowState.Maximized;


            }



            //if (this.WindowState == WindowState.Maximized)
            //{
            //    this.WindowState = WindowState.Normal;
            //}
            //else if (this.WindowState == WindowState.Normal)
            //{
            //    this.WindowState = WindowState.Maximized;
            //    //代码在这
            //    this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            //    this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            //} 
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Maximized)
            {
                var margin = new Thickness(7); // 根据系统设置调整边框宽度
                this.Padding = margin;
            }
            else
            {
                this.Padding = new Thickness(0);
            }



            //if (WindowState == WindowState.Maximized)
            //{
            //    // 获取窗口的 DPI 缩放
            //    var source = PresentationSource.FromVisual(this);
            //    var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
            //    var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

            //    // 获取当前屏幕的工作区域（适配 DPI 缩放）
            //    var screen = SystemParameters.WorkArea;
            //    this.Left = screen.Left / dpiX;
            //    this.Top = screen.Top / dpiY;
            //    this.Width = screen.Width / dpiX;
            //    this.Height = screen.Height / dpiY;
            //}
            base.OnStateChanged(e);
            this.BorderThickness = this.WindowState switch
            {
                WindowState.Maximized => InflateBorder(SystemParameters.WindowResizeBorderThickness),
                WindowState.Normal or WindowState.Minimized when this.OldWindowState == WindowState.Maximized => DeflateBorder(SystemParameters.WindowResizeBorderThickness),
                _ => this.BorderThickness
            };
            this.OldWindowState = this.WindowState;
        }
        #region 最大化时添加虚拟边框
        private WindowState OldWindowState { get; set; }
        private int borderthin = 3;
        private Thickness InflateBorder(Thickness thickness)
        {

            double left = this.BorderThickness.Left + thickness.Left + borderthin;
            double top = this.BorderThickness.Top + thickness.Top + borderthin;
            double right = this.BorderThickness.Right + thickness.Right + borderthin;
            double bottom = this.BorderThickness.Bottom + thickness.Bottom + borderthin;
            return new Thickness(left, top, right, bottom);
        }
        private Thickness DeflateBorder(Thickness thickness)
        {
            double left = this.BorderThickness.Left - thickness.Left - borderthin;
            double top = this.BorderThickness.Top - thickness.Top - borderthin;
            double right = this.BorderThickness.Right - thickness.Right - borderthin;
            double bottom = this.BorderThickness.Bottom - thickness.Bottom - borderthin;
            return new Thickness(left, top, right, bottom);
        }
        #endregion


        //打开设置面板
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建新窗口
            var settingsWindow = new SettingWindow(); // CustomPopupWindow 是自定义的弹窗
            settingsWindow.Owner = this;                 // 设置主窗口为弹窗的 Owner
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // 居中显示
            settingsWindow.ShowDialog();                 // 以模态方式显示
        }
        private void MyWindowButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建新窗口
            //var myWindow = new MyWindow(); // CustomPopupWindow 是自定义的弹窗
            //myWindow.Owner = this;                 // 设置主窗口为弹窗的 Owner
            //myWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // 居中显示
            //myWindow.Show();                 // 以独立窗口方式显示
        }

        //打开关于窗体
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            //var infoWindow = new InfoWindow(); // 另一个弹窗
            //infoWindow.Owner = this;
            //infoWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //infoWindow.ShowDialog();

            // 获取主窗口和 Popup 宽高
            var window = System.Windows.Application.Current.MainWindow;
            var popupWidth = 300; // 与 Popup 宽度保持一致
            var popupHeight = 200; // 与 Popup 高度保持一致

            // 计算 Popup 的中心点
            var centerX = (window.ActualWidth - popupWidth) / 2;
            var centerY = (window.ActualHeight - popupHeight) / 2;

            // 将 Popup 定位到主窗口中心
            MyPopup.HorizontalOffset = centerX + 150;
            MyPopup.VerticalOffset = centerY - (centerY * 2);

            MyPopup.IsOpen = true;
        }


        private void UpdateLatency(object sender, ElapsedEventArgs e)
        {
            Random random = new Random();
            foreach (var wifi in WifiList)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                 {
                     wifi.Latency = random.Next(500, 5000); // 模拟延迟变化
                 });
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建打开文件对话框
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置过滤器，支持多种文件类型
            openFileDialog.Filter = "M3U Files (*.m3u)|*.m3u|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = "请选择播放列表文件";
            var result = openFileDialog.ShowDialog();

            // 显示对话框并检查用户是否选择了文件
            if (result == true)
            {
                // 获取选择的文件路径
                string selectedFilePath = openFileDialog.FileName;
                OpenMyFile(selectedFilePath);
                // 显示文件路径（或在此处处理文件）
                //System.Windows.MessageBox.Show($"Selected File: {selectedFilePath}", "File Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Clear buttom
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentFileName = "";
            OnLineChannels = 0;
            OffLineChannels = 0;
            EffectiveRate = 0;
            CheckRate = 0;
            Channels.Clear();
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string textBoxValue = Search_TextBox.Text; // 获取 TextBox 的值
            //MessageBox.Show($"你输入的内容是: {textBoxValue}");

            Channels.Where(item => item.MediaName == textBoxValue).ToList();
        }
        private void SearchButtonClear_Click(object sender, RoutedEventArgs e)
        {
            SearchText = "";
        }



        private void FilterChannels()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                _channels_forSearch = null; // 🔹 清空搜索结果，恢复原始数据
                IsSearchCloseButtonVisibility = Visibility.Hidden;
            }
            else
            {
                IsSearchCloseButtonVisibility = Visibility.Visible;
                _channels_forSearch = new ObservableCollection<MediaChannel>(
                    _channels.Where(c => c.MediaName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                );
            }
            OnPropertyChanged(nameof(Channels)); // 🔹 触发 UI 刷新
        }


        private void DropdownButton_PreviewMouseUp(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
            if (DropdownPopup.IsOpen)
            {
                DropdownPopup.IsOpen = false;
                return;
            }
        }

        //自定义弹出下拉框
        private void DropdownButton_Click(object sender, RoutedEventArgs e)
        {

            e.Handled = false;
            // 切换 Popup 显示状态
            //DropdownPopup.IsOpen = !DropdownPopup.IsOpen;

            // 如果已经弹出，则关闭 Popup
            if (DropdownPopup.IsOpen)
            {
                DropdownPopup.IsOpen = false;
                return;
            }

            // 获取按钮的位置和尺寸
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button == null) return;

            // 获取按钮的相对于屏幕的左上角位置
            Point buttonPosition = button.PointToScreen(new Point(0, 0));

            // 转换为相对于窗口的位置
            Point relativePosition = this.PointFromScreen(buttonPosition);


            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Maximized
                : WindowState.Normal;
            string stat = WindowState.ToString();
            int c = 0;
            if (stat == "Maximized")
            {
                c -= 7;
            }

            // 设置 Popup 的位置
            DropdownPopup.HorizontalOffset = relativePosition.X;
            DropdownPopup.VerticalOffset = relativePosition.Y + button.ActualHeight - 90 + c;

            // 显示 Popup
            DropdownPopup.IsOpen = true;





        }


        private void WrapPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var wrapPanel = sender as WrapPanel;

            if (wrapPanel == null || wrapPanel.Children.Count == 0)
                return;

            // 获取容器的可用宽度
            double panelWidth = wrapPanel.ActualWidth;

            // 计算每行最多可以容纳多少个项目
            int maxItemsPerRow = (int)(panelWidth / 356); // 356 是最小宽度

            if (maxItemsPerRow == 0)
                return;

            // 动态计算项目宽度和高度，保持 16:9 比例
            double itemWidth = panelWidth / maxItemsPerRow;
            double itemHeight = itemWidth * 9 / 16;

            //// 设置每个子项的宽度和高度
            foreach (UIElement child in wrapPanel.Children)
            {
                if (child is FrameworkElement element)
                {

                    element.Width = itemWidth;
                    element.Height = itemHeight;
                }
            }
        }


        // 处理文件拖入事件
        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            // 检查拖入的内容是否为文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // 获取拖入的文件路径
                var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 检查文件扩展名是否符合要求
                if (filePaths.Any(path => IsValidFileType(path)))
                {
                    e.Effects = DragDropEffects.Copy; // 显示允许操作的图标
                }
                else
                {
                    e.Effects = DragDropEffects.None; // 显示禁止操作的图标
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        // 处理文件释放事件
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 筛选有效文件并显示路径
                var validFiles = filePaths.Where(IsValidFileType).ToList();
                if (validFiles.Any())
                {
                    //System.Windows.MessageBox.Show("文件路径:\n" + string.Join("\n", validFiles));
                    //System.Windows.MessageBox.Show($"{filePaths[0].ToString()}", "文件路径", MessageBoxButton.OK, MessageBoxImage.Information);
                    //ChannelFun.show(filePaths[0]);
                    OpenMyFile(filePaths[0]);
                }
                else
                {
                    e.Effects = DragDropEffects.None; // 非文件内容，显示禁止光标
                                                      //System.Windows.MessageBox.Show("未找到有效的文件（仅限 .m3u 或 .txt）")

                }
            }
        }

        private async void OpenMyFile(string filePath)
        {
            string JsonData = "";
            string extension = System.IO.Path.GetExtension(filePath);
            CurrentFileName = System.IO.Path.GetFileName(filePath);
            if (extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                JsonData = DecodeTxt.TxtToJson(filePath);
            }
            else if (extension.Equals(".m3u", StringComparison.OrdinalIgnoreCase))
            {
                JsonData = DecodeM3u.M3uToJson(filePath);
            }
            else
            {
                MessageBox.Show("不支持该格式！");
            }

            Channels.Clear(); // 清空现有集合
            OnLineChannels = 0;
            OffLineChannels = 0;
            EffectiveRate = 0;
            CheckRate = 0;
            var newChannels = JsonConvert.DeserializeObject<ObservableCollection<MediaChannel>>(JsonData);
            Channels = newChannels;
            //int s = Channels.Count;

        }
        async void StartTest()
        {
            var semaphore = new SemaphoreSlim(5); // 限制同时进行的任务数为 5 避免“并行任务共享资源导致阻塞”
            var tasks = Channels.Select(async item =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return new
                    {
                        MediaChannel = item,
                        DelayTime = await CheckStreamAvailabilityAsync(item.MediaUrl, 5000, Channels.Count),

                    };
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            foreach (var task in tasks)
            {
                var result = await task;

                // 更新 MediaChannel 数据
                result.MediaChannel.isAvailable = result.DelayTime > 0 ? 1 : 0;
                result.MediaChannel.Timeout = result.DelayTime >= 0 ? result.DelayTime : -1;

                // 增强视觉效果
                await Task.Delay(100);

                // 更新统计信息
                OnLineChannels = Channels.Count(c => c.isAvailable == 1);
                OffLineChannels = Channels.Count(c => c.isAvailable == 0);
                EffectiveRate = (double)OnLineChannels / TotalChannels;
                CheckRate = (double)(OnLineChannels + OffLineChannels) / TotalChannels;
            }
            //CollectionViewSource.GetDefaultView(Channels).Refresh();
        }
        #region

        async void StartTest5()
        {
            var semaphore = new SemaphoreSlim(1);
            var tasks = Channels.Select(async item => new
            {
                MediaChannel = item,
                DelayTime = await CheckStreamAvailabilityAsync(item.MediaUrl, 5000, Channels.Count)
            }).ToList();

            foreach (var task in tasks)
            {
                var result = await task;

                // 更新 MediaChannel 数据
                result.MediaChannel.isAvailable = result.DelayTime >= 0 ? 1 : 0;
                result.MediaChannel.Timeout = result.DelayTime >= 0 ? result.DelayTime : -1;

                // 等待短时间，增强视觉效果
                await Task.Delay(200);

                // 更新统计信息
                OnLineChannels = Channels.Count(c => c.isAvailable == 1);
                OffLineChannels = TotalChannels - OnLineChannels;
                EffectiveRate = (double)OnLineChannels / TotalChannels;
            }
            CollectionViewSource.GetDefaultView(Channels).Refresh();
        }

        async void StartTest11()
        {
            var tasks = Channels.Select(async item => new
            {
                MadiaChannel = item,
                DelayTime = await CheckStreamAvailabilityAsync(item.MediaUrl, 7000, Channels.Count)
            }).ToList();

            foreach (var task in tasks)
            {
                var result = await task;

                if (result.DelayTime >= 0)
                {
                    result.MadiaChannel.isAvailable = 1;
                    result.MadiaChannel.Timeout = result.DelayTime;
                }
            }

            // 更新统计属性
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnLineChannels = Channels.Count(c => c.isAvailable == 1);
                OffLineChannels = TotalChannels - OnLineChannels;
                EffectiveRate = (double)OnLineChannels / TotalChannels;


            });
        }

        async void StartTest1()//old func
        {
            /*1、按顺序输出，较慢
           foreach (var item in Channels)
           {
               int intOutTime = await CheckStreamAvailabilityAsync(item.MediaUrl, 7000, Channels.Count);
               if (intOutTime >= 0)
               {
                    item.isAvailable = true;
                    item.Timeout = intOutTime;
 
               }
           }*/

            /* */
            var tasks = Channels.Select(async item => new
            {
                MadiaChannel = item,
                DelayTime = await CheckStreamAvailabilityAsync(item.MediaUrl, 7000, Channels.Count)
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
                    result.MadiaChannel.isAvailable = 1;
                    result.MadiaChannel.Timeout = result.DelayTime;


                    //Console.WriteLine($"{result.MadiaChannel.MediaName} 有效，延时{result.DelayTime}毫秒");
                }
                else
                {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    //Console.WriteLine($"{result.MadiaChannel.MediaName} 无效！");
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            double AvailableRate = (double)AvailabilityCount / ChannelCount;

            //Console.WriteLine($"共{ChannelCount}个频道，其中有效{AvailabilityCount}个，有效率{AvailableRate.ToString("P2")}");

            List<MediaChannel> AvailabilityChannels = Channels.Where(x => x.isAvailable == 1).ToList();
            OnLineChannels = AvailabilityChannels.Count;
            OffLineChannels = TotalChannels - OnLineChannels;
            EffectiveRate = (double)OnLineChannels / TotalChannels;
        }
        #endregion

        private void SaveAllM3uButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "M3U文件另存为";
            saveFileDialog.Filter = "m3u文件 (*.m3u)|*.m3u";
            saveFileDialog.FileName = $"IPTV_ALL_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.m3u";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                SaveAvailableM3u(Channels.ToList(), fileName);
            }
        }
        private void SaveOkM3uButton_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "M3U文件另存为";
            saveFileDialog.Filter = "m3u文件 (*.m3u)|*.m3u";
            saveFileDialog.FileName = $"IPTV_OK_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.m3u";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                List<MediaChannel> newChs = Channels.Where(x => x.isAvailable == 1).ToList();
                SaveAvailableM3u(newChs, fileName);
            }
        }
        //把频道另存为新M3U文件
        private void SaveAvailableM3u(List<MediaChannel> lists, string SAVEPATH)
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
            string newPath = System.IO.Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, $"Data\\playlist_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.m3u");
            File.WriteAllText(SAVEPATH, sb.ToString(), Encoding.UTF8);
        }

        private void SaveAllTxtButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "txt文件另存为";
            saveFileDialog.Filter = "txt文件 (*.txt)|*.txt";
            saveFileDialog.FileName = $"IPTV_ALL_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                SaveTxt(Channels.ToList(), fileName);
            }
        }
        private void SaveOkTxtButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "txt文件另存为";
            saveFileDialog.Filter = "txt文件 (*.txt)|*.txt";
            saveFileDialog.FileName = $"IPTV_OK_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                List<MediaChannel> newChs = Channels.Where(x => x.isAvailable == 1).ToList();
                SaveTxt(newChs, fileName);
            }
        }
        //把频道另存为新Txt文件
        private void SaveTxt(List<MediaChannel> lists, string SAVEPATH)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in lists)
            {
                sb.Append($"{item.MediaName},{item.MediaUrl}\r\n");
            }
            File.WriteAllText(SAVEPATH, sb.ToString(), Encoding.UTF8);
        }


        static int i = 0;
        static async Task<int> CheckStreamAvailabilityAsync(string url, int timeout, int channelCount)
        {
            i++;
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

                        // string outputPath = System.IO.Path.Combine(Environment.CurrentDirectory, $"Snapshot_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.jpg");
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







        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private void Channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalChannels));
            OnPropertyChanged(nameof(OnLineChannels));
            OnPropertyChanged(nameof(OffLineChannels));
            OnPropertyChanged(nameof(EffectiveRate));
            OnPropertyChanged(nameof(CurrentFileName));

        }









        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 获取被双击的项
            var listView = sender as ListView;
            if (listView?.SelectedItem is MediaChannel selectedChannel)
            {
                foreach (var item in Channels)
                {
                    item.IsPlaying = false;
                }
                selectedChannel.IsPlaying = true;
                CurrentPlayChannel = selectedChannel;
                // 处理 MediaUrl 属性
                string mediaUrl = selectedChannel.MediaUrl;

                // 示例：弹出消息框或执行播放逻辑
                //MessageBox.Show($"Media URL: {mediaUrl}");
                PlayMedia(mediaUrl); // 自定义播放逻辑
            }
        }
        private void PlayMedia(string mediaUrl)
        {
            Task.Run(() =>
            {
                try
                {
                    // 停止当前播放
                    if (_mediaPlayer.IsPlaying)
                    {
                        _mediaPlayer.Stop();
                    }

                    // 加载并播放新的媒体
                    var media = new Media(_libVLC, mediaUrl, FromType.FromLocation);
                    _mediaPlayer.Play(media);

                    // 显示正在播放的消息（可选）
                    //Dispatcher.Invoke(() =>
                    //{
                    //    MessageBox.Show($"正在播放: {mediaUrl}");
                    //});
                }
                catch (Exception ex)
                {
                    // 在主线程上显示错误信息
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"播放失败: {ex.Message}");
                    });
                }
            });
        }
        private void StopPlayButton_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {

                // 停止当前播放
                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Stop();
                }
                CurrentPlayChannel = null;
            });
        }
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 将滑块的值设置为音量
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Volume = (int)VolumeSlider.Value;
            }
        }
        private void VideoView_MouseDown(object sender, EventArgs e)
        {
            int i = 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            // 释放资源
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
            base.OnClosed(e);
        }







        // 检查文件扩展名是否有效
        private bool IsValidFileType(string filePath)
        {
            string[] allowedExtensions = { ".m3u", ".txt" };
            string extension = System.IO.Path.GetExtension(filePath)?.ToLower();
            return allowedExtensions.Contains(extension);
        }


        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                //    MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                //  MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }
        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}