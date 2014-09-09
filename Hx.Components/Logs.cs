using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Web;
using Hx.Tools;
using System.IO;

namespace Hx.Components
{
    public class Logs
    {
        private static object o = new object();
        private static object o2 = new object();
        private static object o3 = new object();
        private static object o4 = new object();
        private static readonly int MAX_LENG = 500;

        private static Dictionary<string, List<string>> errorurllogdt = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> rebotdt = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> fromrebotdt = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> urlreferrerdt = new Dictionary<string, List<string>>();

        //private static List<string> txts = new List<string>();
        //private static List<string> txts2 = new List<string>();
        //private static List<string> txts3 = new List<string>();

        #region 错误url地址日志

        /// <summary>
        /// 获取访问错误地址列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetErrorUrlList()
        {
            lock (o)
            {
                string host = HXContext.Current.CurrentHost;
                if (!errorurllogdt.Keys.Contains(host))
                {
                    errorurllogdt[host] = new List<string>();

                }
                return errorurllogdt[host];
            }
        }

        /// <summary>
        /// 写入错误Url地址到文件
        /// </summary>
        /// <param name="txt"></param>
        public static void WriteErrorUrlLog(string txt)
        {
            WriteErrorUrlLog(txt, false);
        }

        /// <summary>
        /// 写入错误Url地址到文件
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="flush"></param>
        public static void WriteErrorUrlLog(string txt, bool flush)
        {
            lock (o)
            {
                try
                {
                    List<string> txts = GetErrorUrlList();
                    if (!string.IsNullOrEmpty(txt))
                    {
                        txts.Add("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "                      " + txt);
                    }
                    if (txts.Count >= MAX_LENG || flush)
                    {
                        string s = "";
                        if (!string.IsNullOrEmpty(HXContext.Current.CurrentHost))
                        {
                            s = HXContext.Current.CurrentHost + "/";
                        }
                        string path = Utils.GetMapPath(string.Format("~/{0}log/errorurl/{1}/{2}/", s, DateTime.Now.Year, DateTime.Now.Month));
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = path + DateTime.Now.Day + ".txt";
                        StreamWriter sw = null;
                        if (File.Exists(path))
                        {
                            sw = File.AppendText(path);
                        }
                        else
                        {
                            sw = File.CreateText(path);
                        }
                        foreach (string str in txts)
                        {
                            sw.WriteLine(str);
                        }
                        sw.Flush();
                        sw.Close();
                        sw = null;
                        txts.Clear();
                    }
                }
                catch { }
            }
        }

        #endregion

        #region  搜索引擎访问记录

        /// <summary>
        /// 获取搜索引擎访问记录
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRebotsList()
        {
            lock (o2)
            {
                string host = HXContext.Current.CurrentHost;
                if (!rebotdt.Keys.Contains(host))
                {
                    rebotdt[host] = new List<string>();

                }
                return rebotdt[host];

            }
        }

        /// <summary>
        /// 记录搜索引擎访问的记录
        /// </summary>
        /// <param name="txt"></param>
        public static void WriteRebotsLog(string txt)
        {
            WriteRebotsLog(txt, false);
        }

        /// <summary>
        /// 记录搜索引擎访问的记录
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="flush"></param>
        public static void WriteRebotsLog(string txt, bool flush)
        {
            lock (o2)
            {
                try
                {

                    string s = "";
                    List<string> txts2 = GetRebotsList();
                    if (!string.IsNullOrEmpty(txt))
                    {
                        txts2.Add("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "         " + txt);
                    }
                    if (txts2.Count >= MAX_LENG || flush)
                    {
                        if (!string.IsNullOrEmpty(HXContext.Current.CurrentHost))
                        {
                            s = HXContext.Current.CurrentHost + "/";
                        }
                        string path = Utils.GetMapPath(string.Format("~/{0}log/rebots/{1}/{2}/", s, DateTime.Now.Year, DateTime.Now.Month));
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        StreamWriter sw = null;
                        path = path + DateTime.Now.Day + ".txt";
                        if (File.Exists(path))
                        {
                            sw = File.AppendText(path);
                        }
                        else
                        {
                            sw = File.CreateText(path);
                        }
                        foreach (string str in txts2)
                        {
                            sw.WriteLine(str);
                        }
                        sw.Flush();
                        sw.Close();
                        sw = null;
                        txts2.Clear();
                    }

                }
                catch { }
            }
        }

        #endregion

        #region 从搜索引擎访问过来的用户日志

        /// <summary>
        /// 获取通过搜索引擎访问过来的用户
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFromRebotsList()
        {
            lock (o2)
            {
                string host = HXContext.Current.CurrentHost;
                if (!fromrebotdt.Keys.Contains(host))
                {
                    fromrebotdt[host] = new List<string>();

                }
                return fromrebotdt[host];
            }
        }

        /// <summary>
        /// 获取通过搜索引擎访问过来的用户
        /// </summary>
        /// <param name="txt"></param>
        public static void WriteFromRebotsLog(string txt)
        {
            WriteFromRebotsLog(txt, false);
        }

        /// <summary>
        /// 获取通过搜索以前访问过来的用户
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="flush"></param>
        public static void WriteFromRebotsLog(string txt, bool flush)
        {
            lock (o3)
            {
                try
                {
                    List<string> txts3 = GetFromRebotsList();
                    string s = "";
                    if (!string.IsNullOrEmpty(txt))
                    {
                        txts3.Add("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "         " + txt);
                    }
                    if (txts3.Count >= MAX_LENG || flush)
                    {
                        if (!string.IsNullOrEmpty(HXContext.Current.CurrentHost))
                        {
                            s = HXContext.Current.CurrentHost + "/";
                        }
                        string path = Utils.GetMapPath(string.Format("~/{0}log/formbots/{1}/{2}/", s, DateTime.Now.Year, DateTime.Now.Month));
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        StreamWriter sw = null;
                        path = path + DateTime.Now.Day + ".txt";
                        if (File.Exists(path))
                        {
                            sw = File.AppendText(path);
                        }
                        else
                        {
                            sw = File.CreateText(path);
                        }
                        foreach (string str in txts3)
                        {
                            sw.WriteLine(str);
                        }
                        sw.Flush();
                        sw.Close();
                        sw = null;
                        txts3.Clear();
                    }

                }
                catch { }
            }
        }

        #endregion
        /// <summary>
        /// 清空全部缓存区的日志内容，输出到文本
        /// </summary>
        public static void ClearWriteLog()
        {
            WriteErrorUrlLog("", true);
            WriteRebotsLog("", true);
            WriteFromRebotsLog("", true);
        }
    }
}
