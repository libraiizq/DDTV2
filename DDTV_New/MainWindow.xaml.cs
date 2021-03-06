﻿using Auxiliary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using static Auxiliary.bilibili;
using static DDTV_New.RoomInit;
using MessageBox = System.Windows.MessageBox;

namespace DDTV_New
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public static SolidColorBrush 弹幕颜色 = new SolidColorBrush();
        public static SolidColorBrush 字幕颜色 = new SolidColorBrush();
        public static List<PlayW.MainWindow> playList1 = new List<PlayW.MainWindow>();
        public static int 播放器版本 = 1;
      //  public static List<硬解播放器.Form1> playList2 = new List<硬解播放器.Form1>();
       


        public MainWindow()
        {
            InitializeComponent();
            this.Title = "DDTV2.0主窗口";
            home.Visibility = Visibility.Visible;
            top层.Visibility = Visibility.Collapsed;
            直播层.Visibility = Visibility.Collapsed;
            设置层.Visibility = Visibility.Collapsed;
            关于层.Visibility = Visibility.Collapsed;
            帮助层.Visibility = Visibility.Collapsed;
            插件层.Visibility = Visibility.Collapsed;
            工具层.Visibility = Visibility.Collapsed;
            AOE直播层.Visibility = Visibility.Collapsed;
            软件启动配置初始化();
            icon();
            MMPU.弹窗.IcoUpdate += A_IcoUpdate;

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // **** Always accept
            };
            System.Net.ServicePointManager.DefaultConnectionLimit = 999;/*---------这里最重要--------*/
            System.Net.ServicePointManager.MaxServicePoints = 999;
        }


        public void 软件启动配置初始化()
        {
            //检查配置文件
            bilibili.BiliUser.CheckPath(MMPU.BiliUserFile);
            #region 配置文件设置
            //房间配置文件
            RoomConfigFile = MMPU.读取exe默认配置文件("RoomConfiguration", "./RoomListConfig.json");
            //房间配置文件
            MMPU.下载储存目录 = MMPU.读取exe默认配置文件("file", "./tmp/");
            //直播表刷新默认间隔
            MMPU.直播列表刷新间隔 = int.Parse(MMPU.读取exe默认配置文件("LiveListTime", "5"));
            //播放窗口默认高度
            MMPU.PlayWindowH = int.Parse(MMPU.读取exe默认配置文件("PlayWindowH", "450"));
            //播放窗口默认宽度
            MMPU.PlayWindowW = int.Parse(MMPU.读取exe默认配置文件("PlayWindowW", "800"));
            //直播缓存目录
            MMPU.直播缓存目录 = MMPU.读取exe默认配置文件("Livefile", "./tmp/LiveCache/");
            //直播更新时间
            MMPU.直播更新时间 = int.Parse(MMPU.读取exe默认配置文件("RoomTime", "40"));
            //默认音量
            MMPU.默认音量 = int.Parse(MMPU.读取exe默认配置文件("DefaultVolume", "50"));
            //缩小功能
            MMPU.缩小功能 = int.Parse(MMPU.读取exe默认配置文件("Zoom", "1"));
            //最大直播并行数量
            MMPU.最大直播并行数量 = int.Parse(MMPU.读取exe默认配置文件("PlayNum", "5"));
            //默认弹幕颜色
            MMPU.默认弹幕颜色 = MMPU.读取exe默认配置文件("DanMuColor", "0xFF, 0x00, 0x00, 0x00");
            //默认字幕颜色
            MMPU.默认字幕颜色 = MMPU.读取exe默认配置文件("ZiMuColor", "0xFF, 0x00, 0x00, 0x00");
            //默认字幕大小
            MMPU.默认字幕大小 = int.Parse(MMPU.读取exe默认配置文件("ZiMuSize", "24"));
            //默认弹幕大小
            MMPU.默认弹幕大小 = int.Parse(MMPU.读取exe默认配置文件("DanMuSize", "20"));
            //默认弹幕大小
            MMPU.播放缓冲时长 = int.Parse(MMPU.读取exe默认配置文件("BufferDuration", "3"));
            #endregion

            #region BiliUser配置文件初始化
            //账号登陆cookie

            try
            {
                MMPU.Cookie = string.IsNullOrEmpty(MMPU.读ini配置文件("User", "Cookie", MMPU.BiliUserFile)) ? "" : Encryption.UnAesStr(MMPU.读ini配置文件("User", "Cookie", MMPU.BiliUserFile), MMPU.AESKey, MMPU.AESVal);
            }
            catch (Exception)
            {
                MMPU.Cookie = "";
            }
            //账号UID
            MMPU.UID = MMPU.读ini配置文件("User", "UID", MMPU.BiliUserFile); //string.IsNullOrEmpty(MMPU.读取exe默认配置文件("UID", "")) ? null : MMPU.读取exe默认配置文件("UID", "");
            //账号登陆cookie的有效期
            try
            {
                if (!string.IsNullOrEmpty(MMPU.读ini配置文件("User", "CookieEX", MMPU.BiliUserFile)))
                {
                    MMPU.CookieEX = DateTime.Parse(MMPU.读ini配置文件("User", "CookieEX", MMPU.BiliUserFile));
                    if (DateTime.Compare(DateTime.Now, MMPU.CookieEX) > 0)
                    {
                        MessageBox.Show("BILIBILI账号登陆已过期");
                        MMPU.Cookie = "";
                        MMPU.写ini配置文件("User", "Cookie", "", MMPU.BiliUserFile);
                        MMPU.csrf = null;
                        MMPU.写ini配置文件("User", "csrf", "", MMPU.BiliUserFile);
                    }
                }
                if (!string.IsNullOrEmpty(MMPU.Cookie))
                {
                    登陆B站账号.IsEnabled = false;
                    注销B站账号.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                MMPU.写ini配置文件("User", "Cookie", "", MMPU.BiliUserFile);
                MMPU.Cookie = null;

            }
            //账号csrf

            MMPU.csrf = MMPU.读ini配置文件("User", "csrf", MMPU.BiliUserFile);
            #endregion

            //初始化房间
            RoomInit.start();
            //公告加载线程
            new Thread(new ThreadStart(delegate
            {
                公告项目启动();
            })).Start();

            new Task(() =>
            {
                MMPU.加载网络房间方法.更新网络房间缓存();
            }).Start();

            //房间刷新线程
            new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    刷新房间列表UI();
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        newtime.Content = "数据更新时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }));
                    while (true)
                    {
                        if (房间信息更新次数 > 0)
                        {
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                首次更新.Visibility = Visibility.Collapsed;
                            }));
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(MMPU.直播列表刷新间隔 * 1000);
                }
            })).Start();
            //延迟测试
            new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    try
                    {
                        double 国内 = MMPU.测试延迟("https://live.bilibili.com");
                        string 国内延迟 = string.Empty;
                        string 国外延迟 = string.Empty;
                        if (国内 > 0)
                        {
                            国内延迟 = "国内服务器延迟(阿B):" + 国内.ToString().Split('.')[0] + "ms";
                            MMPU.是否能连接阿B = true;
                        }
                        else
                        {
                            国内延迟 = "国内服务器延迟(阿B): 连接超时";
                            MMPU.是否能连接阿B = false;
                        }

                        if (MMPU.连接404使能)
                        {
                            double 国外 = MMPU.测试延迟("https://www.google.com");
                            if (国外 > 0)
                            {
                                国外延迟 = "国内服务器延迟(404):" + 国外.ToString().Split('.')[0] + "ms";
                                MMPU.是否能连接404 = true;
                            }
                            else
                            {
                                国外延迟 = "国内服务器延迟(404): 连接超时";
                                MMPU.是否能连接404 = false;
                            }

                        }
                        else
                        {
                            MMPU.是否能连接404 = false;
                            国外延迟 = "";
                        }
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            国内服务器延迟.Content = 国内延迟;
                        }));
                        if (MMPU.连接404使能)
                        {
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                国外服务器延迟.Content = 国外延迟;
                            }));

                        }
                        else
                        {
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                国外服务器延迟.Content = "";
                            }));

                        }

                    }
                    catch (Exception) { }
                    Thread.Sleep(4000);
                }
            })).Start();
            //缩小功能
            {
                MMPU.缩小功能 = int.Parse(MMPU.getFiles("Zoom"));
                if (MMPU.缩小功能 == 1)
                {
                    缩小到任务栏选择按钮.IsChecked = true;
                }
                else
                {
                    隐藏到后台托盘选择按钮.IsChecked = true;
                }
            }
            //加载配置文件
            {
                默认音量值显示.Content = MMPU.默认音量;
                修改默认音量.Value = MMPU.默认音量;
                默认下载路径.Text = MMPU.下载储存目录;
                //读取字幕弹幕颜色
                {
                    SolidColorBrush S1 = new SolidColorBrush(Color.FromArgb(0xFF, Convert.ToByte(MMPU.默认字幕颜色.Split(',')[1], 16), Convert.ToByte(MMPU.默认字幕颜色.Split(',')[2], 16), Convert.ToByte(MMPU.默认字幕颜色.Split(',')[3], 16)));
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        字幕默认颜色.Foreground = S1;
                        字幕颜色 = S1;
                    }));
                    SolidColorBrush S2 = new SolidColorBrush(Color.FromArgb(0xFF, Convert.ToByte(MMPU.默认弹幕颜色.Split(',')[1], 16), Convert.ToByte(MMPU.默认弹幕颜色.Split(',')[2], 16), Convert.ToByte(MMPU.默认弹幕颜色.Split(',')[3], 16)));
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        弹幕默认颜色.Foreground = S2;
                        弹幕颜色 = S2;
                    }));
                }
                //读取字幕弹幕字体大小
                {
                    字幕文字大小.Text = MMPU.默认字幕大小.ToString();
                    弹幕文字大小.Text = MMPU.默认弹幕大小.ToString();
                }
                //默认音量
                {
                    修改默认音量.Value = MMPU.默认音量;
                }
                //播放窗口默认大小
                {
                    默认播放宽度.Text = MMPU.PlayWindowW.ToString();
                    默认播放高度.Text = MMPU.PlayWindowH.ToString();
                }
                //播放缓冲时长
                {
                    播放缓冲时长.Text = MMPU.播放缓冲时长.ToString();
                }
            }
            //增加插件列表
            {

                PluginC.Items.Add(new
                {
                    编号 = "0",
                    名称 = "DDTV",
                    版本 = MMPU.版本号,
                    是否加载 = "强制",
                    说明 = "本软件的所有必须内容()",
                    备注 = ""
                });
                PluginC.Items.Add(new
                {
                    编号 = "1",
                    名称 = "bilibili数据接口插件",
                    版本 = "1.0.1.1",
                    是否加载 = "√",
                    说明 = "获取和处理来自阿B的直播数据流",
                    备注 = ""
                });
                PluginC.Items.Add(new
                {
                    编号 = "2",
                    名称 = "播放插件",
                    版本 = "1.0.0.4",
                    是否加载 = "√",
                    说明 = "用于播放直播视频流",
                    备注 = ""
                });
                PluginC.Items.Add(new
                {
                    编号 = "3",
                    名称 = "DDNA直播统计插件",
                    版本 = "1.0.0.1",
                    是否加载 = "√",
                    说明 = "用于获取目前已知正在直播的vtb列表(工具箱内)",
                    备注 = ""
                });
                PluginC.Items.Add(new
                {
                    编号 = "4",
                    名称 = "弹幕录制工具",
                    版本 = "1.0.0.1",
                    是否加载 = "X",
                    说明 = "用于录制直播弹幕内容(工具箱内)",
                    备注 = "调试中的功能，还没写完"
                });
                PluginC.Items.Add(new
                {
                    编号 = "5",
                    名称 = "BiliAccount",
                    版本 = "2.2.0.12",
                    是否加载 = "√",
                    说明 = "用于处理B站账号类的操作",
                    备注 = "基于MIT授权引用GITHUB @LeoChen98/BiliAccount"
                });
            }



            this.Dispatcher.Invoke(new Action(delegate
            {
                版本显示.Content = "版本：" + MMPU.版本号;
            }));

        }
        /// <summary>
        /// 通过get方式返回内容
        /// </summary>
        /// <param name="url">目标网页地址</param>
        /// <param name="headername">http标头名</param>
        /// <param name="headervl">http标头值</param>
        /// <returns></returns>
        public static string get返回网页内容(Uri url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        public void 公告项目启动()
        {
            //动态推送1
            new Thread(new ThreadStart(delegate
            {
                try
                {

                    while (true)
                    {
                        bool 动态推送1开关 = MMPU.TcpSend(20009, "{}", true) == "1" ? true : false;
                        if (动态推送1开关)
                        {
                            string 动态推送内容 = MMPU.TcpSend(20010, "{}", true);
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                动态推送1.Content = 动态推送内容;
                            }));
                        }
                        Thread.Sleep(600 * 1000);
                    }
                }
                catch (Exception) { }
            })).Start();
            //版本检查
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    string 服务器版本号 = MMPU.TcpSend(20011, "{}", true);
                    if (!string.IsNullOrEmpty(服务器版本号))
                    {
                        bool 检测状态 = true;
                        foreach (var item in MMPU.不检测的版本号)
                        {
                            if(服务器版本号 == item)
                            {
                                检测状态 = false;
                            }
                        }
                        if (MMPU.版本号 != 服务器版本号 && 检测状态)
                        {
                            MessageBoxResult dr = System.Windows.MessageBox.Show("检测到版本更新,更新公告:\n" + MMPU.TcpSend(20012, "{}", true) + "\n\n点击确定跳转到补丁(大约1MB)下载网页，点击取消忽略", "有新版本", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                            if (dr == MessageBoxResult.OK)
                            {
                                System.Diagnostics.Process.Start("https://github.com/CHKZL/DDTV2/releases/latest");
                            }
                        }
                    }
                }
                catch (Exception) { }
            })).Start();
            //推送内容1
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    string 推送内容1text = MMPU.TcpSend(20005, "{}", true);
                    if (推送内容1text.Length < 25)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            推送内容1.Content = 推送内容1text;
                        }));
                    }

                }
                catch (Exception) { }
            })).Start();
        }
        private void 刷新房间列表UI()
        {

            InitRoomList();
        }

        public void InitRoomList()
        {
            List<RoomInfo> 正在直播 = new List<RoomInfo>();
            List<RoomInfo> 未直播 = new List<RoomInfo>();
            foreach (var item in bilibili.RoomList)
            {
                if (item.直播状态)
                {
                    正在直播.Add(item);
                }
                else
                {
                    未直播.Add(item);
                }
            }
            int i = 1;
            string 检测房间状态变化用的字符串 = string.Empty;
            foreach (var item in 正在直播)
            {
                检测房间状态变化用的字符串 += i;
                检测房间状态变化用的字符串 += item.名称;
                检测房间状态变化用的字符串 += item.直播状态;
                检测房间状态变化用的字符串 += "bilibili";
                检测房间状态变化用的字符串 += item.是否提醒;
                检测房间状态变化用的字符串 += item.是否录制视频;
                检测房间状态变化用的字符串 += item.房间号;
                检测房间状态变化用的字符串 += item.原名;
            }
            foreach (var item in 未直播)
            {
                检测房间状态变化用的字符串 += i;
                检测房间状态变化用的字符串 += item.名称;
                检测房间状态变化用的字符串 += item.直播状态;
                检测房间状态变化用的字符串 += "bilibili";
                检测房间状态变化用的字符串 += item.是否提醒;
                检测房间状态变化用的字符串 += item.是否录制视频;
                检测房间状态变化用的字符串 += item.房间号;
                检测房间状态变化用的字符串 += item.原名;
            }
            if (MMPU.房间状态MD5值 != MMPU.GetMD5(检测房间状态变化用的字符串))
            {
                MMPU.房间状态MD5值 = MMPU.GetMD5(检测房间状态变化用的字符串);
                this.Dispatcher.Invoke(new Action(delegate
                {
                    LiveList.Items.Clear();
                }));
                foreach (var item in 正在直播)
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        LiveListAdd(i, item.名称, item.直播状态, "bilibili", item.是否提醒, item.是否录制视频, item.房间号, item.原名);
                    }));
                    i++;

                }
                foreach (var item in 未直播)
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        LiveListAdd(i, item.名称, item.直播状态, "bilibili", item.是否提醒, item.是否录制视频, item.房间号, item.原名);
                    }));
                    i++;
                }
            }
            int 单推人数 = i - 1;
            this.Dispatcher.Invoke(new Action(delegate
            {
                ppnum.Content = 单推人数;
                
            }));

            if (正在直播.Count == 0)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    tabText.Content = "监控列表中没有直播中的单推对象";
                }));

            }
            else
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    tabText.Content = "在监控列表中有" + 正在直播.Count + "个单推对象正在直播";
                }));

            }
            this.Dispatcher.Invoke(new Action(delegate
            {
                ppnum.Content = i - 1;
                等待框.Visibility = Visibility.Collapsed;
            }));

        }
        public void LiveListAdd(int 编号, string 名称, bool 状态, string 平台, bool 直播提醒, bool 是否录制, string 唯一码, string 原名)
        {
            LiveList.Items.Add(new { 编号 = 编号, 名称 = 名称, 状态 = 状态 ? "●直播中" : "○未直播", 平台 = 平台, 是否提醒 = 直播提醒 ? "√" : "", 是否录制 = 是否录制 ? "√" : "", 唯一码 = 唯一码, 原名 = 原名 });
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
            }
        }


        private void 工具箱_按钮事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("工具层");
        }
        private void 帮助_按钮事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("帮助层");
        }
        private void DDNA列表_按钮事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("DDNA列表");
        }
        private void 关于_按钮事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("关于层");
        }

        private void 关闭按钮_Click(object sender, MouseButtonEventArgs e)
        {
            DDTV关闭事件();
        }
        public static void DDTV关闭事件()
        {

            MessageBoxResult dr = System.Windows.MessageBox.Show("确定退出DDTV？", "退出", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        FileInfo[] files = new DirectoryInfo("./tmp/LiveCache/").GetFiles();
                        foreach (var item in files)
                        {
                            MMPU.文件删除委托("./tmp/LiveCache/" + item.Name);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    Environment.Exit(0);
                })).Start();
            }
        }
        NotifyIcon notifyIcon;
        private void 最小化按钮_Click(object sender, MouseButtonEventArgs e)
        {
            if (MMPU.缩小功能 == 1)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                this.Hide();
            }
        }

        private void A_IcoUpdate(object sender, EventArgs e)
        {
            MMPU.弹窗提示 A = (MMPU.弹窗提示)sender;
            this.notifyIcon.ShowBalloonTip(A.持续时间, A.标题, A.内容, ToolTipIcon.None);
        }

        private void icon()
        {
            this.notifyIcon = new NotifyIcon
            {
                BalloonTipText = "DDTV已启动", //设置程序启动时显示的文本
                Text = "DDTV",//最小化到托盘时，鼠标点击时显示的文本
                Icon = new System.Drawing.Icon("DDTV.ico"),//程序图标
                Visible = true
            };
            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(1000);
        }
        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }
        private void 返回首页_点击事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("home");
        }
        private void 切换界面(string name)
        {
            home.Visibility = Visibility.Collapsed;
            top层.Visibility = Visibility.Collapsed;
            直播层.Visibility = Visibility.Collapsed;
            设置层.Visibility = Visibility.Collapsed;
            关于层.Visibility = Visibility.Collapsed;
            帮助层.Visibility = Visibility.Collapsed;
            插件层.Visibility = Visibility.Collapsed;
            工具层.Visibility = Visibility.Collapsed;
            AOE直播层.Visibility = Visibility.Collapsed;
            switch (name)
            {
                case "home":
                    home.Visibility = Visibility.Visible;
                    break;
                case "直播层":
                    top层.Visibility = Visibility.Visible;
                    直播层.Visibility = Visibility.Visible;
                    break;
                case "设置层":
                    top层.Visibility = Visibility.Visible;
                    设置层.Visibility = Visibility.Visible;
                    break;
                case "关于层":
                    top层.Visibility = Visibility.Visible;
                    关于层.Visibility = Visibility.Visible;
                    break;
                case "帮助层":
                    top层.Visibility = Visibility.Visible;
                    帮助层.Visibility = Visibility.Visible;
                    break;
                case "插件层":
                    top层.Visibility = Visibility.Visible;
                    插件层.Visibility = Visibility.Visible;
                    break;
                case "工具层":
                    top层.Visibility = Visibility.Visible;
                    工具层.Visibility = Visibility.Visible;
                    break;
                case "DDNA列表":
                    top层.Visibility = Visibility.Visible;
                    AOE直播层.Visibility = Visibility.Visible;
                    break;
            }

        }
        private void 默认音量修改事件(object sender, System.Windows.Input.MouseEventArgs e)
        {
            默认音量值显示.Content = (int)修改默认音量.Value;
            MMPU.默认音量 = (int)修改默认音量.Value;
            MMPU.修改默认音量设置(MMPU.默认音量);
        }
        private void 关注列表_点击事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("直播层");
        }
        private void 设置层_点击事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("设置层");
        }
        private void 插件_点击事件(object sender, MouseButtonEventArgs e)
        {
            切换界面("插件层");
        }

        private void 直播表单击事件(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Windows.Controls.ListView LV = (System.Windows.Controls.ListView)sender;
                if (LV.Items.Count != 0)
                {
                    已选内容 = LV.SelectedItems[0].ToString();
                    选中内容1.Content = MMPU.获取livelist平台和唯一码.平台(已选内容) + "\n" + MMPU.获取livelist平台和唯一码.唯一码(已选内容) + "\n" + MMPU.获取livelist平台和唯一码.名称(已选内容);
                }
                //Console.WriteLine("已选内容");
            }
            catch (Exception)
            {
            }
        }
        public static string 已选内容 = "";
        private void 添加直播列表按钮点击事件(object sender, RoutedEventArgs e)
        {
            等待框.Visibility = Visibility.Visible;
            AddMonitoringList AML = new AddMonitoringList("添加新单推", "", "", "", "", false);
            AML.ShowDialog();

        }
        private void 修改直播列表按钮点击事件(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(已选内容))
            {
                System.Windows.MessageBox.Show("未选择");
                return;
            }
            等待框.Visibility = Visibility.Visible;
            AddMonitoringList AML = new AddMonitoringList("修改单推属性", MMPU.获取livelist平台和唯一码.名称(已选内容), MMPU.获取livelist平台和唯一码.原名(已选内容), MMPU.获取livelist平台和唯一码.平台(已选内容), MMPU.获取livelist平台和唯一码.唯一码(已选内容), MMPU.获取livelist平台和唯一码.状态(已选内容) == "●直播中" ? true : false);
            AML.ShowDialog();
        }

        private void AOE直播表双击事件(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Controls.ListView LV = (System.Windows.Controls.ListView)sender;
                string A = LV.SelectedItems[0].ToString();
                A = A.Replace("\"", "").Replace(" ", "").Replace("{", "").Replace("}", "");
                string[] B = A.Split(',');
                string 直播地址 = B[4].Replace("直播URL=", "");
                if (string.IsNullOrEmpty(直播地址))
                {
                    return;
                }
                System.Diagnostics.Process.Start(直播地址);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("打开这个直播发生错误，可能是由于无法连接目标网页");
            }
        }
        private void 直播表双击事件(object sender, MouseButtonEventArgs e)
        {
            if (MMPU.当前直播窗口数量 >= MMPU.最大直播并行数量)
            {
                System.Windows.MessageBox.Show("达到了设置的最大直播窗口数量,新建直播窗口失败");
                return;
            }
            等待框.Visibility = Visibility.Visible;
            System.Windows.Controls.ListView LV = (System.Windows.Controls.ListView)sender;
            try
            {
                if (!bilibili.GetRoomInfo(MMPU.获取livelist平台和唯一码.唯一码(LV.SelectedItems[0].ToString())).直播状态)
                {
                    System.Windows.MessageBox.Show("该房间未直播");
                    等待框.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            catch (Exception)
            {
                等待框.Visibility = Visibility.Collapsed;
                return;
            }
            string 平台 = MMPU.获取livelist平台和唯一码.平台(LV.SelectedItems[0].ToString());
            string 唯一码 = MMPU.获取livelist平台和唯一码.唯一码(LV.SelectedItems[0].ToString());
            string 标题 = "";
            try
            {
                switch (平台)
                {
                    case "bilibili":
                        {

                            string GUID = Guid.NewGuid().ToString();
                            foreach (var item in bilibili.RoomList)
                            {
                                if (item.房间号 == 唯一码)
                                {
                                    标题 = item.标题;
                                }
                            }
                            string 下载地址 = string.Empty;
                            try
                            {
                                下载地址 = bilibili.根据房间号获取房间信息.下载地址(唯一码);
                            }
                            catch (Exception)
                            {
                                System.Windows.MessageBox.Show("获取下载地址错误");
                                return;
                            }
                            Downloader 下载对象 = new Downloader
                            {
                                DownIofo = new Downloader.DownIofoData() { 平台 = 平台, 房间_频道号 = 唯一码, 标题 = 标题, 事件GUID = GUID, 下载地址 = 下载地址, 备注 = "视频播放缓存", 是否保存 = false }
                            };
                            //Downloader 下载对象 = Downloader.新建下载对象(平台, 唯一码, 标题, GUID, 下载地址, "视频播放缓存", false);

                            Task.Run(() =>
                            {
                                this.Dispatcher.Invoke(new Action(delegate
                                {
                                    打开直播列表(下载对象);
                                    MMPU.当前直播窗口数量++;
                                    等待框.Visibility = Visibility.Collapsed;
                                }));
                            });
                            break;
                        }
                    default:
                        System.Windows.MessageBox.Show("发现了与当前版本不支持的平台，请检查更新");
                        return;
                }
            }
            catch (Exception ex)
            {
                错误窗 ERR = new 错误窗("新建播放窗口发生意外错误，请重试", "新建播放窗口发生意外错误，请重试\n" + ex.ToString());
                ERR.ShowDialog();
                return;
            }
        }
        public void 打开直播列表(Downloader DL)
        {
            //System.Diagnostics.Process p = new System.Diagnostics.Process();
            //p.StartInfo.FileName = @"D:\Program Files (x86)\Pure Codec\x64\PotPlayerMini64.exe";//需要启动的程序名       
            //p.StartInfo.Arguments = " \""+DL.DownIofo.下载地址+"\"";//启动参数       
            //p.Start();//启动       
          
            //return;


            if (DL != null)
            {
                DL.DownIofo.播放状态 = true;
                DL.DownIofo.是否是播放任务 = true;
                switch(播放器版本)
                {
                    case 1:
                        {
                            PlayW.MainWindow PlayWindow = new PlayW.MainWindow(DL, MMPU.默认音量, 弹幕颜色, 字幕颜色, MMPU.默认弹幕大小, MMPU.默认字幕大小, MMPU.PlayWindowW, MMPU.PlayWindowH);
                            PlayWindow.Closed += 播放窗口退出事件;
                            PlayWindow.Show();
                            PlayWindow.BossKey += 老板键事件;
                            playList1.Add(PlayWindow);
                            break;
                        }
                    case 2:
                        {
                            //硬解播放器.Form1 PlayWindow = new 硬解播放器.Form1(DL, MMPU.默认音量,MMPU.PlayWindowW, MMPU.PlayWindowH);
                            //PlayWindow.Closed += 播放窗口退出事件;
                            //PlayWindow.Show();
                            //playList2.Add(PlayWindow);
                            break;
                        }
                }
               // PlayW.MainWindow PlayWindow = new PlayW.MainWindow(DL, MMPU.默认音量, 弹幕颜色, 字幕颜色, MMPU.默认弹幕大小, MMPU.默认字幕大小, MMPU.PlayWindowW, MMPU.PlayWindowH);
               
                
                
                // MMPU.ClearMemory();
            }
            else
            {
                System.Windows.MessageBox.Show("Downloader结构体不能为Null,出现了未知的错误！");
                return;
            }

        }

        private void 老板键事件(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            foreach (var item in playList1)
            {
                if (item.窗口是否打开)
                {
                    item.WindowState = WindowState.Minimized;
                }
            }
        }

        private void 播放窗口退出事件(object sender, EventArgs e)
        {
            try
            {
                new Thread(new ThreadStart(delegate
                {
                    MMPU.当前直播窗口数量--;
                    switch (播放器版本)
                    {
                        case 1:
                            {
                                PlayW.MainWindow p = (PlayW.MainWindow)sender;
                                playList1.Remove(p);
                                foreach (var item in MMPU.DownList)
                                {
                                    if (item.DownIofo.事件GUID == p.DD.DownIofo.事件GUID)
                                    {
                                        item.DownIofo.WC.CancelAsync();
                                        item.DownIofo.下载状态 = false;
                                        item.DownIofo.结束时间 = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
                                        if (item.DownIofo.是否保存)
                                        {

                                        }
                                        else
                                        {
                                            MMPU.文件删除委托(p.DD.DownIofo.文件保存路径);
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        case 2:
                            {
                                //硬解播放器.Form1 p = (硬解播放器.Form1)sender;
                                //playList2.Remove(p);
                                //foreach (var item in MMPU.DownList)
                                //{
                                //    if (item.DownIofo.事件GUID == p.DD.DownIofo.事件GUID)
                                //    {
                                //        item.DownIofo.WC.CancelAsync();
                                //        item.DownIofo.下载状态 = false;
                                //        item.DownIofo.结束时间 = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
                                //        if (item.DownIofo.是否保存)
                                //        {

                                //        }
                                //        else
                                //        {
                                //            MMPU.文件删除委托(p.DD.DownIofo.文件保存路径);
                                //        }
                                //        break;
                                //    }
                                //}
                                break;
                            }
                    }
                   
                })).Start();
            }
            catch (Exception)
            {


            }
        }

        private void 直播列表删除按钮点击事件(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(已选内容))
            {
                System.Windows.MessageBox.Show("未选择");
                return;
            }
            等待框.Visibility = Visibility.Visible;
            var rlc2 = JsonConvert.DeserializeObject<RoomBox>(ReadConfigFile(RoomConfigFile));
            foreach (var item in rlc2.data)
            {
                if (item.RoomNumber == MMPU.获取livelist平台和唯一码.唯一码(已选内容))
                {
                    rlc2.data.Remove(item);
                    break;
                }
            }

            string JOO = JsonConvert.SerializeObject(rlc2);
            MMPU.储存文本(JOO, RoomConfigFile);
            InitializeRoomList();
            //更新房间列表(MMPU.获取livelist平台和唯一码(已选内容, "平台"), MMPU.获取livelist平台和唯一码(已选内容, "唯一码"),0);
            System.Windows.MessageBox.Show("删除完成");

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void 修改录制状态点击事件(object sender, RoutedEventArgs e)
        {
            修改列表设置(true);

        }

        private void 修改提醒状态点击事件(object sender, RoutedEventArgs e)
        {
            修改列表设置(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">T修改录制设置，F修改提醒设置</param>
        public void 修改列表设置(bool a)
        {
            Console.WriteLine(已选内容);
            bool 是否改过 = false;
            if (string.IsNullOrEmpty(已选内容))
            {
                System.Windows.MessageBox.Show("未选择");
                return;
            }
            //编号 = 1, 名称 = 智障爱, 状态 = ○未直播, 平台 = bilibili, 是否提醒 = √, 是否录制 = , 唯一码 = 1485080, 原名 = 
            等待框.Visibility = Visibility.Visible;
            RoomBox RB = new RoomBox
            {
                data = new List<RoomCadr>()
            };
            while (RoomInit.房间主表长度 != 房间主表.Count() && RoomInit.房间主表长度 != 0)
            {
                Thread.Sleep(10);
            }
            int rlclen = 房间主表.Count();
            for (int i = 0; i < rlclen; i++)
            {
                if (房间主表[i].唯一码 == MMPU.获取livelist平台和唯一码.唯一码(已选内容))
                {
                    if (!是否改过)
                    {
                        是否改过 = true;

                        房间主表.Remove(房间主表[i]);
                        rlclen--;
                        i--;
                        bool 是否录制 = MMPU.获取livelist平台和唯一码.是否录制(已选内容) == "√" ? true : false;
                        bool 是否提醒 = MMPU.获取livelist平台和唯一码.是否提醒(已选内容) == "√" ? true : false;
                        if (a)
                        {
                            是否录制 = !是否录制;
                            if (是否录制)
                            {
                                已选内容 = 已选内容.Replace("是否录制 = ", "是否录制 = √");
                            }
                            else
                            {
                                已选内容 = 已选内容.Replace("是否录制 = √", "是否录制 = ");
                            }
                        }
                        else
                        {
                            是否提醒 = !是否提醒;
                            if (是否提醒)
                            {
                                已选内容 = 已选内容.Replace("是否提醒 = ", "是否提醒 = √");
                            }
                            else
                            {
                                已选内容 = 已选内容.Replace("是否提醒 = √", "是否提醒 = ");
                            }
                        }

                        RB.data.Add(new RoomCadr
                        {
                            Name = MMPU.获取livelist平台和唯一码.名称(已选内容),
                            RoomNumber = MMPU.获取livelist平台和唯一码.唯一码(已选内容),
                            Types = MMPU.获取livelist平台和唯一码.平台(已选内容),
                            RemindStatus = 是否提醒,
                            status = MMPU.获取livelist平台和唯一码.状态(已选内容) == "●直播中" ? true : false,
                            VideoStatus = 是否录制,
                            OfficialName = MMPU.获取livelist平台和唯一码.原名(已选内容),
                            LiveStatus = MMPU.获取livelist平台和唯一码.状态(已选内容) == "●直播中" ? true : false
                        });
                    }
                }
                else
                {
                    RB.data.Add(new RoomCadr() { LiveStatus = 房间主表[i].直播状态, Name = 房间主表[i].名称, OfficialName = 房间主表[i].原名, RoomNumber = 房间主表[i].唯一码, VideoStatus = 房间主表[i].是否录制, Types = 房间主表[i].平台, RemindStatus = 房间主表[i].是否提醒, status = false });
                    if (RoomInit.根据唯一码获取直播状态(房间主表[i].唯一码))
                    {
                        RB.data[RB.data.Count() - 1].LiveStatus = true;
                    }
                }
            }
            string JOO = JsonConvert.SerializeObject(RB);
            MMPU.储存文本(JOO, RoomConfigFile);
            InitializeRoomList();
            //更新房间列表(平台.SelectedItem.ToString(), 唯一码.Text,2);
            //System.Windows.MessageBox.Show("修改成功");

        }

        private void 录制按钮点击事件(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(已选内容))
            {
                System.Windows.MessageBox.Show("未选择");
                return;
            }
            new Thread(new ThreadStart(delegate
            {
                switch (MMPU.获取livelist平台和唯一码.平台(已选内容))
                {
                    case "bilibili":
                        {
                            if (!bilibili.根据房间号获取房间信息.是否正在直播(MMPU.获取livelist平台和唯一码.唯一码(已选内容)))
                            {
                                System.Windows.MessageBox.Show("该房间当前未直播");
                                return;
                            }
                            break;
                        }
                    //case "youtube":
                    //    {
                    //        break;
                    //    }
                    //case "tc":
                    //    {
                    //        break;
                    //    }
                    //case "douyu":
                    //    {
                    //        break;
                    //    }
                    //case "老鼠台":
                    //    {
                    //        break;
                    //    }
                    //case "AcFun":
                    //    {
                    //        break;
                    //    }
                    default:
                        System.Windows.MessageBox.Show("发现了与当前版本不支持的平台，请检查更新");
                        return;
                }
                string GUID = Guid.NewGuid().ToString();
                string 标题 = bilibili.根据房间号获取房间信息.获取标题(MMPU.获取livelist平台和唯一码.唯一码(已选内容));
                string 下载地址 = string.Empty;
                try
                {
                    下载地址 = bilibili.根据房间号获取房间信息.下载地址(MMPU.获取livelist平台和唯一码.唯一码(已选内容));
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("获取下载地址失败");
                    return;
                }
                Downloader 下载对象 = Downloader.新建下载对象(MMPU.获取livelist平台和唯一码.平台(已选内容), MMPU.获取livelist平台和唯一码.唯一码(已选内容), 标题, GUID, 下载地址, "手动下载任务", true);

                System.Windows.MessageBox.Show("下载任务添加完成");
            })).Start();
        }

        private void 显示下载队列按钮点击事件(object sender, RoutedEventArgs e)
        {
            下载工具 A = new 下载工具();
            A.Show();
        }

        private void 跳转到网页按钮点击事件(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(已选内容))
            {
                System.Windows.MessageBox.Show("未选择");
                return;
            }
            switch (MMPU.获取livelist平台和唯一码.平台(已选内容))
            {
                case "bilibili":
                    System.Diagnostics.Process.Start("https://live.bilibili.com/" + MMPU.获取livelist平台和唯一码.唯一码(已选内容));
                    break;
                default:
                    System.Windows.MessageBox.Show("发现了与当前版本不支持的平台，请检查更新");
                    return;
            }

        }

        private void Slider_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void 修改默认下载目录按钮事件(object sender, RoutedEventArgs e)
        {


            FolderBrowserDialog P_File_Folder = new FolderBrowserDialog();

            if (P_File_Folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MMPU.下载储存目录 = P_File_Folder.SelectedPath;
                默认下载路径.Text = MMPU.下载储存目录;
                MMPU.setFiles("file", MMPU.下载储存目录);

            }
            P_File_Folder.Dispose();
        }

        private void 缩小_任务栏选择事件(object sender, RoutedEventArgs e)
        {
            MMPU.缩小功能 = 1;
            MMPU.setFiles("Zoom", "1");
        }

        private void 缩小_后台托盘选择事件(object sender, RoutedEventArgs e)
        {
            MMPU.缩小功能 = 0;
            MMPU.setFiles("Zoom", "0");
        }

        private void 连接404类使能开关点击事件(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(MMPU.测试延迟("https://www.google.com"));
            if (连接404类使能开关.IsChecked == true)
            {
                MMPU.连接404使能 = true;
                MMPU.是否能连接404 = true;
            }
            else
            {

                MMPU.连接404使能 = false;
                MMPU.是否能连接404 = false;
            }
        }

        private void 修改最大直播并行数量确定按钮点击事件(object sender, RoutedEventArgs e)
        {
            MMPU.最大直播并行数量 = int.Parse(并行直播数量.Text);
            MMPU.setFiles("PlayNum", 并行直播数量.Text);
            System.Windows.MessageBox.Show("修改成功");
        }
        private void 修改弹幕颜色按钮点击事件(object sender, RoutedEventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color))
                    {
                        SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));
                        弹幕默认颜色.Foreground = solidColorBrush;
                        MMPU.默认弹幕颜色 = solidColorBrush.Color.A.ToString("X2") + "," + solidColorBrush.Color.R.ToString("X2") + "," + solidColorBrush.Color.G.ToString("X2") + "," + solidColorBrush.Color.B.ToString("X2");
                        MMPU.setFiles("DanMuColor", MMPU.默认弹幕颜色);
                        弹幕颜色 = solidColorBrush;
                    }
                }
            }
        }
        private void 修改字幕颜色按钮点击事件(object sender, RoutedEventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color))
                    {
                        SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));
                        字幕默认颜色.Foreground = solidColorBrush;
                        MMPU.默认字幕颜色 = solidColorBrush.Color.A.ToString("X2") + "," + solidColorBrush.Color.R.ToString("X2") + "," + solidColorBrush.Color.G.ToString("X2") + "," + solidColorBrush.Color.B.ToString("X2");
                        MMPU.setFiles("ZiMuColor", MMPU.默认字幕颜色);
                        字幕颜色 = solidColorBrush;
                    }
                }
            }
        }
        private void 并行直播数量_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            检测输入框是否为数字((System.Windows.Controls.TextBox)sender, 30,3);
        }
        private void 弹幕文字大小_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            检测输入框是否为数字((System.Windows.Controls.TextBox)sender, 30,1);
        }
        private void 修改弹幕文字大小确定按钮点击事件(object sender, RoutedEventArgs e)
        {
            MMPU.默认弹幕大小 = int.Parse(弹幕文字大小.Text);
            MMPU.setFiles("DanMuSize", 弹幕文字大小.Text);
            System.Windows.MessageBox.Show("修改成功");
        }
        private void 字幕文字大小_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            检测输入框是否为数字((System.Windows.Controls.TextBox)sender, 99,1);
        }
        private void 分辨率大小_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            检测输入框是否为数字((System.Windows.Controls.TextBox)sender, 4000,128);
        }
        private void 修改字幕文字大小确定按钮点击事件(object sender, RoutedEventArgs e)
        {
            MMPU.默认字幕大小 = int.Parse(字幕文字大小.Text);
            MMPU.setFiles("ZiMuSize", 字幕文字大小.Text);
            System.Windows.MessageBox.Show("修改成功");
        }
        private void 修改播放器默认大小确定按钮点击事件(object sender, RoutedEventArgs e)
        {
            MMPU.PlayWindowW = int.Parse(默认播放宽度.Text);
            MMPU.setFiles("PlayWindowW", 默认播放宽度.Text);
            MMPU.PlayWindowH = int.Parse(默认播放高度.Text);
            MMPU.setFiles("PlayWindowH", 默认播放高度.Text);

            System.Windows.MessageBox.Show("修改成功");
        }
        private void 检测输入框是否为数字(System.Windows.Controls.TextBox A, int max,int min)
        {
            if (!string.IsNullOrEmpty(A.Text))
            {
                try
                {
                    int.Parse(A.Text);
                    if (int.Parse(A.Text) >= max)
                    {
                        A.Text = (max - 1).ToString();
                    }
                    if (int.Parse(A.Text) < min)
                    {
                        A.Text = min.ToString();
                    }
                }
                catch (Exception)
                {
                    A.Text = A.Text.Substring(0, A.Text.Length - 1);
                }
            }
        }

        private void 刷新AOE直播列表按钮点击事件_Click(object sender, RoutedEventArgs e)
        {
            刷新AOE直播列表按钮.IsEnabled = false;
            刷新AOE直播列表按钮.Content = "刷新中....";
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    外部API.正在直播数据 直播数据 = new 外部API.正在直播数据();
                    直播数据.更新正在直播数据();
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        bilibiliAOE直播列表.Items.Clear();
                        youtubeAOE直播列表.Items.Clear();
                        tcAOE直播列表.Items.Clear();
                    }));
                    foreach (var item in 直播数据.直播数据)
                    {
                        string 时间 = MMPU.将时间戳转换为日期类型(item.实际开始时间);
                        if (!string.IsNullOrEmpty(item.直播连接))
                        {
                            switch (item.频道类型)
                            {
                                case 4:

                                    this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        bilibiliAOE直播列表.Items.Add(new { 名称 = item.主播名称, 标题 = item.标题, 观看人数 = item.当前观众, 直播开始时间 = 时间, 直播URL = item.直播连接 });
                                    }));
                                    break;
                                case 1:
                                    this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        youtubeAOE直播列表.Items.Add(new { 名称 = item.主播名称, 标题 = item.标题, 观看人数 = item.当前观众, 直播开始时间 = 时间, 直播URL = item.直播连接 });
                                    }));
                                    break;
                                case 8:
                                    this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        tcAOE直播列表.Items.Add(new { 名称 = item.主播名称, 标题 = item.标题, 观看人数 = item.当前观众, 直播开始时间 = 时间, 直播URL = item.直播连接 });
                                    }));
                                    break;
                            }
                        }
                    }
                    if (bilibiliAOE直播列表.Items.Count == 0)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            bilibiliAOE直播列表.Items.Add(new { 名称 = "", 标题 = "当前没有在BILIBILI直播的VTB", 观看人数 = "", 直播开始时间 = "", 直播URL = "" });
                        }));
                    }
                    if (youtubeAOE直播列表.Items.Count == 0)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            youtubeAOE直播列表.Items.Add(new { 名称 = "", 标题 = "当前没有在YouTuBe直播的VTB", 观看人数 = "", 直播开始时间 = "", 直播URL = "" });
                        }));
                    }
                    if (tcAOE直播列表.Items.Count == 0)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            tcAOE直播列表.Items.Add(new { 名称 = "", 标题 = "当前没有在TwitCasting直播的VTB", 观看人数 = "", 直播开始时间 = "", 直播URL = "" });
                        }));
                    }
                }
                catch (Exception)
                {

                }
                this.Dispatcher.Invoke(new Action(delegate
                {
                    刷新AOE直播列表按钮.IsEnabled = true;
                    刷新AOE直播列表按钮.Content = "刷新列表";
                }));
            })).Start();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void 获取网络列表按钮点击事件(object sender, RoutedEventArgs e)
        {
            window.NetRoomList NRL = new window.NetRoomList();
            NRL.Show();
        }

        private void 登陆B站账号_Click(object sender, RoutedEventArgs e)
        {
            Plugin.BilibiliAccount.BiliLogin();
            if (!string.IsNullOrEmpty(MMPU.Cookie))
            {
                登陆B站账号.IsEnabled = false;
                注销B站账号.IsEnabled = true;
            }
        }

        private void 注销B站账号_Click(object sender, RoutedEventArgs e)
        {
            MMPU.Cookie = "";
            MMPU.写ini配置文件("User", "Cookie", "", MMPU.BiliUserFile);
            MMPU.csrf = null;
            MMPU.写ini配置文件("User", "csrf", "", MMPU.BiliUserFile);
            登陆B站账号.IsEnabled = true;
            注销B站账号.IsEnabled = false;
        }

        private void 一键导入账号关注VTB和VUP_Click(object sender, RoutedEventArgs e)
        {
           
            if (string.IsNullOrEmpty(MMPU.Cookie))
            {
                MessageBox.Show("请先登录");
                return;
            }
            else
            {
                MessageBox.Show("=======================\n点击确定开始导入，在此期间请勿操作\n=======================");
            }
            new Thread(new ThreadStart(delegate {
                int 增加的数量 = 0;
                RoomBox rlc = JsonConvert.DeserializeObject<RoomBox>(ReadConfigFile(RoomConfigFile));
                RoomBox RB = new RoomBox
                {
                    data = new List<RoomCadr>()
                };
                if (rlc.data != null)
                {
                    foreach (var item in rlc.data)
                    {
                        RB.data.Add(item);
                    }
                }
                List<MMPU.加载网络房间方法.选中的网络房间> 符合条件的房间 = new List<MMPU.加载网络房间方法.选中的网络房间>();
                JObject BB = bilibili.根据UID获取关注列表(MMPU.UID);
                foreach (var 账号关注数据 in BB["data"])
                {
                    foreach (var 网络房间数据 in MMPU.加载网络房间方法.列表缓存)
                    {
                        if (账号关注数据["UID"].ToString() == 网络房间数据.UID)
                        {
                            符合条件的房间.Add(new MMPU.加载网络房间方法.选中的网络房间()
                            {
                                UID = 网络房间数据.UID,
                                名称 = 网络房间数据.名称,
                                官方名称 = 网络房间数据.官方名称,
                                平台 = 网络房间数据.平台,
                                房间号 = null,
                                编号 = 0
                            });
                            break;
                        }
                    }
                }

                foreach (var 符合条件的 in 符合条件的房间)
                {

                    if (!string.IsNullOrEmpty(符合条件的.UID))
                    {
                        string 房间号 = 通过UID获取房间号(符合条件的.UID);

                        符合条件的.房间号 = 房间号;
                        bool 是否已经存在 = false;
                        foreach (var item in bilibili.RoomList)
                        {
                            if (item.房间号 == 房间号)
                            {
                                是否已经存在 = true;
                                break;
                            }
                        }
                        if (!是否已经存在 && !string.IsNullOrEmpty(房间号.Trim('0')))
                        {
                            增加的数量++;
                            RB.data.Add(new RoomCadr { Name = 符合条件的.名称, RoomNumber = 符合条件的.房间号, Types = 符合条件的.平台, RemindStatus = false, status = false, VideoStatus = false, OfficialName = 符合条件的.官方名称, LiveStatus = false });
                        }
                    }

                }
                string JOO = JsonConvert.SerializeObject(RB);
                MMPU.储存文本(JOO, RoomConfigFile);
                InitializeRoomList();
               
                MessageBox.Show("导入完成，新导入" + 增加的数量 + "个,主窗口列表可能会有延迟，请多等待几秒");
            })).Start();
            
        }

        private void 播放本地视频文件按钮_Click(object sender, RoutedEventArgs e)
        {

        }

        private void 播放缓冲时长_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            检测输入框是否为数字((System.Windows.Controls.TextBox)sender, 60, 1);
        }

        private void 修改播放缓冲时长确定按钮点击事件(object sender, RoutedEventArgs e)
        {
            MMPU.播放缓冲时长 = int.Parse(播放缓冲时长.Text);
            MMPU.setFiles("BufferDuration", 播放缓冲时长.Text);
            MessageBox.Show("修改成功");
        }

        private void 播放窗口排序按钮点击事件(object sender, RoutedEventArgs e)
        {

            double W = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            double H = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            switch (playList1.Count)
            {
                case 1:
                    {
                        playList1[0].Width = W;
                        playList1[0].Height = H;
                        break;
                    }
            }

            //playList
        }
    }
}