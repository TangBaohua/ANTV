using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;


//#EXTINF:-1 tvg-id="10179" tvg-chno="140" tvg-name="ESPN US HD" tvg-logo="http://*/*/misc/logos/240/549.png" group-title="Eng;Sports",ESPN US HD

public class MediaChannel : INotifyPropertyChanged
{
    private ObservableCollection<MediaChannel> _channels;

    public ObservableCollection<MediaChannel> Channels
    {
        get => _channels;
        set
        {
            if (_channels != value)
            {
                _channels = value;
                OnPropertyChanged(nameof(Channels));
            }
        }
    }

    private bool _isPlaying;
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;
            OnPropertyChanged(nameof(IsPlaying));
        }
    }


    private string? _mediaName;
    public string? TvgName { get; set; }
    public string? TvgLogo { get; set; }
    public string? Symbol 
    { 
        get=> MediaName is null? null
           //  :$"/symbols/{MatchChannel(MediaName,channelRules)}.png";
          : System.IO.Path.Combine(Environment.CurrentDirectory + $"/Symbols/{MatchChannel(MediaName,channelRules)}.png");

    }
    public string? GroupTitle { get; set; }
    public bool Radio { get; set; }
    // `MediaName` 属性，支持通知机制
    public required string MediaName
    {
        get => _mediaName!;
        set
        {
            if (_mediaName != value)
            {
                _mediaName = value;
                OnPropertyChanged(nameof(MediaName));
                OnPropertyChanged(string.Empty); // 通知所有属性的变动
                OnPropertyChanged(nameof(Symbol));// 通知 Symbol 属性发生了变化
            }
        }
    }
    public required string MediaUrl { get; set; }
    //public int Timeout { get; set; } = -1;
    private int _timeout = -1;
    public int Timeout
    {
        get => _timeout;
        set
        {
            if (_timeout != value)
            {
                _timeout = value;
                OnPropertyChanged(nameof(Timeout));
            }
        }
    }

    //public bool isAvailable { get; set; } ;

    //private bool _isAvailable= false;
    //public bool isAvailable
    //{
    //    get => _isAvailable;
    //    set
    //    {
    //        if (_isAvailable != value)
    //        {
    //            _isAvailable = value;
    //            OnPropertyChanged(nameof(isAvailable));
    //        }
    //    }
    //}
    /// <summary>
    /// 是否可用：-1:未检测；0:不可用；1:可用
    /// </summary>
    private int _isAvailable = -1;
    public int isAvailable
    {
        get => _isAvailable;
        set
        {
            if (_isAvailable != value)
            {
                _isAvailable = value;
                OnPropertyChanged(nameof(isAvailable));
            }
        }
    }

    public string _snapshot = "";
    public string Snapshot
    {
        get => _snapshot;
        set
        {
            //if (_snapshot != value)
            //{
            //    _snapshot = value;
            //    OnPropertyChanged(nameof(Snapshot));


            //}

            _snapshot = value;
            OnPropertyChanged(nameof(Snapshot));
        }

    }


    // 实现 INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    // 定义频道匹配规则：目标频道名 -> 关键词数组
    private Dictionary<string, string[]> channelRules = new Dictionary<string, string[]>
            {
                { "CNR", new[] { "之", "声"} },
                { "CRI", new[] { "中国国际", "广播电台", "CRI", "CRI" } },
                { "VOA", new[] { "VOA", "VOA", "美国", "之音" } },
                { "RFA", new[] { "RFA", "RFA", "自由亚洲", "电台" } },

                { "凤凰卫视中文台", new[] { "凤凰", "中文","凤凰中文","凤凰卫视" , "凤凰台", "^资讯" } },
                { "凤凰卫视资讯台", new[] { "凤凰", "资讯","凤凰资讯","凤凰资讯台" } },
                { "CETV-1", new[] { "CETV", "1" , "CETV1", "教育一台", "中国教育电视台" } },
                { "CETV-2", new[] { "CETV", "2"} },
                { "CETV-3", new[] { "CETV", "3"} },
                { "CETV-4", new[] { "CETV", "4"} },
                { "CCTV-1", new[] { "CCTV", "1" , "综合", "央视", "^10", "^11", "^12", "^13", "^14", "^15", "^16", "^17" } },
                { "CCTV-2", new[] { "CCTV", "2" , "财经", "央视", "^12" } },
                { "CCTV-3", new[] { "CCTV", "3" , "综合", "央视", "^13" } },
                { "CCTV-4", new[] { "CCTV", "4" , "国际", "中文国际", "央视","^k", "^14" } },
                { "CCTV-5", new[] { "央视", "体育" ,"5","CCTV","^+", "^15" } },
                { "CCTV-5+", new[] { "央视", "体育赛事" ,"5","CCTV","+", "^15" } },
                { "CCTV-6", new[] { "CCTV", "6" , "电影", "央视", "^16" } },
                { "CCTV-7", new[] { "CCTV", "7" , "国防军事", "央视", "^17" } },
                { "CCTV-8", new[] { "CCTV", "8" , "电视剧", "央视" } },
                { "CCTV-9", new[] { "CCTV", "9" , "纪录", "央视" } },
                { "CCTV-10", new[] { "CCTV", "10" , "科教", "央视" } },
                { "CCTV-11", new[] { "CCTV", "11" , "戏曲", "央视" } },
                { "CCTV-12", new[] { "CCTV", "12" , "社会与法", "央视" } },
                { "CCTV-13", new[] { "CCTV", "13" , "新闻", "央视" } },
                { "CCTV-14", new[] { "CCTV", "14" , "少儿", "央视" } },
                { "CCTV-15", new[] { "CCTV", "15" , "音乐", "央视" } },
                { "CCTV-16", new[] { "CCTV", "16" , "奥林匹克", "央视" } },
                { "CCTV-17", new[] { "CCTV", "17" , "农业农村", "央视" } },
                { "CCTV-4K", new[] { "CCTV", "4K" , "超高清", "央视","CCTV UHD", "CCTVUHD", "CCTV-UHD" } },

                { "CCTV-电视指南", new[] { "CCTV", "电视指南"} },
                { "CCTV-第一剧场", new[] { "CCTV", "第一剧场" } },
                { "CCTV-风云剧场", new[] { "CCTV", "风云剧场" } },
                { "CCTV-风云音乐", new[] { "CCTV", "风云音乐" } },
                { "CCTV-风云足球", new[] { "CCTV", "风云足球" } },
                { "CCTV-兵器科技", new[] { "CCTV", "兵器科技" } },
                { "CCTV-世界地理", new[] { "CCTV", "世界地理" } },
                { "CCTV-女性时尚", new[] { "CCTV", "女性时尚" } },
                { "CCTV-高尔夫网球", new[] { "CCTV", "高尔夫" } },
                { "CCTV-怀旧剧场", new[] { "CCTV", "怀旧剧场" } },
                { "CCTV-卫视健康", new[] { "CCTV", "卫视健康" } },
                { "CCTV-央视台球", new[] { "CCTV", "台球" } },
                { "CCTV-央视文化精品", new[] { "CCTV", "央视文化精品" } },
                { "CCTV-中学生", new[] { "CCTV", "中学生" } },
                { "CCTV-新科动漫", new[] { "CCTV", "新科动漫" } },
                { "CCTV-老故事", new[] { "CCTV", "老故事" } },
                { "重温经典", new[] { "重温", "经典" } },
                { "CGTN", new[] { "CGTN", "CGTN", "^Documentary" } },
                { "CGTNDocumentary", new[] { "CGTN", "Documentary" } },
                { "气象频道", new[] { "气象频道", "中国气象频道" } },

                { "湖南卫视", new[] { "湖南", "卫视"} },
                { "湖南经视", new[] { "湖南", "经视" } },
                { "湖南都市", new[] { "湖南", "都市" } },
                { "湖南电影", new[] { "湖南", "电影" } },
                { "湖南电视剧", new[] { "湖南", "电视剧" } },
                { "湖南娱乐", new[] { "湖南", "娱乐" } },
                { "湖南爱晚", new[] { "湖南", "爱晚" } },
                { "湖南教育台", new[] { "湖南", "教育" } },
                { "金鹰卡通", new[] { "金鹰", "卡通" } },
                { "金鹰纪实", new[] { "金鹰", "纪实" } },

                { "东方卫视", new[] { "东方", "卫视"} },
                { "东方影视", new[] { "东方", "影视"} },
                { "上海电视台", new[] { "上海", "新闻综合", "上海电视台" } },                
                { "浙江卫视", new[] { "浙江", "卫视"} },
                { "江苏卫视", new[] { "江苏", "卫视"} },
                { "安徽卫视", new[] { "安徽", "卫视"} },
                { "北京卫视", new[] { "北京", "卫视"} },
                { "广东卫视", new[] { "广东", "卫视"} },
                { "深圳卫视", new[] { "深圳", "卫视"} },
                { "海南卫视", new[] { "海南", "卫视"} },
                { "三沙卫视", new[] { "三沙", "卫视"} },
                { "天津卫视", new[] { "天津", "卫视"} },
                { "重庆卫视", new[] { "重庆", "卫视"} },
                { "河北卫视", new[] { "河北", "卫视"} },
                { "河南卫视", new[] { "河南", "卫视"} },
                { "山东卫视", new[] { "山东", "卫视"} },
                { "山西卫视", new[] { "山西", "卫视"} },
                { "陕西卫视", new[] { "陕西", "卫视"} },
                { "云南卫视", new[] { "云南", "卫视"} },
                { "四川卫视", new[] { "四川", "卫视"} },
                { "湖北卫视", new[] { "湖北", "卫视"} },
                { "贵州卫视", new[] { "贵州", "卫视"} },
                { "江西卫视", new[] { "江西", "卫视"} },
                { "甘肃卫视", new[] { "甘肃", "卫视"} },
                { "宁夏卫视", new[] { "宁夏", "卫视"} },
                { "东南卫视", new[] { "东南", "卫视"} },
                { "广西卫视", new[] { "广西", "卫视"} },
                { "黑龙江卫视", new[] { "黑龙江", "卫视"} },
                { "吉林卫视", new[] { "吉林", "卫视"} },
                { "辽宁卫视", new[] { "辽宁", "卫视"} },
                { "内蒙古卫视", new[] { "内蒙古", "卫视"} },
                { "新疆卫视", new[] { "新疆", "卫视"} },
                { "西藏卫视", new[] { "西藏", "卫视"} },


                { "NASATV", new[] { "NASA", "NASA" } },
                { "CNNWORLD", new[] { "CNN", "CNN" } },
                { "FOXNEWS", new[] { "FOX", "FOX" } },
                { "BBC_World_News", new[] { "BBC", "BBC" } },
                { "RT", new[] { "Russia", "Today", "今日", "俄罗斯","^纪录片","^Documentary" } },
                { "RT_Doc", new[] { "Russia", "Today","Documentary", "今日", "俄罗斯纪录片频道" } },

                { "动物星球频道", new[] { "动物", "星球" , "Animal", "Planet" } },
                { "国家地理频道", new[] { "国家地理频道", "国家地理","national","geographic","^野生", "^wild" } },
                { "国家地理野生频道", new[] { "国家地理野生频道", "国家地理野生" ,"geographic","wild"} },
                { "TLC", new[] { "TLC", "TLC" } },
                { "DMAX", new[] { "DMAX", "DMAX" } },
                { "HBO高清频道", new[] { "HBO", "高清","HD" } },
                { "HBO", new[] { "HBO", "HBO", "HBO" } },
                { "新加坡亚洲新闻台", new[] { "CNA", "新加坡","亚洲","新闻台","channel news asia", "channel news asia", "cna" } },
                { "探索频道", new[] { "discovery", "discovery", "discovery channel", "探索", "频道","^asia","^亚洲","^探索亚洲" } },
                { "探索亚洲频道", new[] { "discovery asia", "探索亚洲频道", "探索亚洲","亚洲" ,"asia","discovery"} },
                { "TVB", new[] { "TVB", "翡翠台", "翡翠台","翡翠" ,"^TVBS"} },
                { "TVB_Pearl", new[] { "TVB", "明珠台", "明珠台","明珠", "Pearl", "Pearl", "^TVBS" } },
                { "TVB无线新闻台", new[] { "TVB", "新闻台", "无线新闻台", "无线新闻", "^TVBS" } },

                 { "澳门电视台", new[] { "澳门", "澳视","Macau" } },
                 { "澳门莲花卫视", new[] { "澳门", "莲花" } },

                { "台视", new[] { "台视", "台视" } },
                { "中视", new[] { "中视", "中视" } },
                { "华视", new[] { "华视", "华视" } },
                { "公视", new[] { "公视", "公视" } },
                { "民视", new[] { "民视", "民视" } },
                { "TVBS", new[] { "TVBS", "TVBS新闻台" } },
                { "中天", new[] { "中天", "中天" } },
                { "东森电视", new[] { "东森", "东森" } },

                { "NHK", new[] { "NHK", "NHK" } },
                { "PBS", new[] { "PBS", "PBS" } },
                { "FashionTV", new[] { "FashionTV", "Fashion","法国时尚", "法国时尚" } },

                { "CHC影迷电影", new[] { "CHC", "影迷电影","高清" } },
                { "CHC动作电影", new[] { "CHC", "动作电影" } },
                { "CHC家庭影院", new[] { "CHC", "家庭影院" } }
            };

    private string MatchChannel(string input, Dictionary<string, string[]> rules)
    {
        foreach (var rule in rules)
        {
            string targetChannel = rule.Key;
            string[] keywords = rule.Value;

            // 判断输入频道是否至少包含两个关键词
            int matchCount = 0;
            foreach (string keyword in keywords)
            {
                if (keyword.StartsWith("^")) // 取反关键词
                {
                    string negativeKeyword = keyword.Substring(1);
                    if (input.Contains(negativeKeyword, StringComparison.OrdinalIgnoreCase))
                    {
                        matchCount--;
                        break;
                    }

                }
                else if (input.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    matchCount++;
                }
            }

            // 如果匹配关键词的数量达到两个或更多，则返回目标频道
            if (matchCount >= 2)
            {
                return targetChannel;
            }
        }

        // 如果没有匹配到，则返回原输入
        return input;
    }
}

