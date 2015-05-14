using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Data;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Web.Security;
using System.Net;
using Hx.Tools;
using System.ComponentModel;
using System.Threading;

namespace Hx.Tools
{
    public class Utils
    {
        #region 文件操作

        /// <summary>
        /// 获得当前真实物理路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>物理路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("~", "");
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 0)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }

        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return System.IO.File.Exists(filename);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);

        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="name">文件夹名</param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return Utils.MakeSureDirectoryPathExists(name);
        }

        /// <summary>
        /// 根据日期获取当前时间
        /// </summary>
        /// <returns></returns>
        public static string GetFileName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssff");
        }

        /// <summary>
        /// 判断是否正常的文件路径（window）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFilePath(string path)
        {

            Regex regex = new Regex(@"[a-zA-Z]\:[\\a-zA-Z0-9_\\]+[\.]?[a-zA-Z0-9_]+", RegexOptions.Compiled);
            return regex.IsMatch(path);
        }

        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
                return false;

            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }
        #endregion

        #region 时间格式操作

        /// <summary>
        /// 返回标准日期格式string
        /// </summary>
        public static string GetStandardDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetStandardTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetStandardDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetStandardDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        /// <param name="o">需要转化的对象</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static string GetDate(object o, string format)
        {
            string datetimestr = string.Empty;
            if (o != null)
            {
                try
                {
                    datetimestr = Convert.ToDateTime(o).ToString(format);
                }
                catch { }
            }
            return datetimestr;
        }


        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间 
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
                return fDateTime;
            DateTime time = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            if (DateTime.TryParse(fDateTime, out time))
                return time.ToString(formatStr);
            else
                return "N/A";
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd
        /// </sumary>
        public static string GetStandardDate(string fDate)
        {
            return GetStandardDateTime(fDate, "yyyy-MM-dd");
        }

        /// <summary>
        /// 判断是否是时间格式
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 转换日期格式如：20060907-》2006-09-07
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToStandardDate(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                StringBuilder sb = new StringBuilder();
                try
                {
                    string year = str.Substring(0, 4);
                    sb.Append(year);
                    sb.Append("-");
                    string month = str.Substring(4, 2);
                    sb.Append(month);
                    sb.Append("-");
                    string day = str.Substring(6, 2);
                    sb.Append(day);
                    if (str.Length > 8)
                    {
                        sb.Append(" ");
                        sb.Append(str.Substring(8, 2));
                        sb.Append(":");
                        sb.Append(str.Substring(10, 2));
                        sb.Append(":");
                        sb.Append(str.Substring(12, 2));
                    }
                    return sb.ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 转换日期格式如：20060907-》2006-09-07
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToStandardDateTime(string str)
        {
            string s = ToStandardDate(str);
            return DataConvert.SafeDate(s, DateTime.MaxValue);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dl"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static string DateToChina(DateTime dl)
        {
            TimeSpan ts = DateTime.Now.Subtract(dl);
            string result = null;
            if (ts.Ticks > 0)
            {
                result = "{0}之后";
            }
            else
            {
                result = "{0}之前";
            }
            ts = ts.Duration();
            if (ts.TotalDays > 10)
            {
                return dl.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                if (ts.TotalDays >= 1)
                {
                    result = string.Format(result, (int)ts.TotalDays + "天");
                }
                else
                {
                    if (ts.TotalHours >= 1)
                    {
                        result = string.Format(result, (int)ts.TotalHours + "小时");
                    }
                    else
                    {
                        if (ts.TotalMinutes >= 1)
                        {
                            result = string.Format(result, (int)ts.TotalMinutes + "分钟");
                        }
                        else
                        {
                            result = string.Format(result, (int)ts.Seconds + "秒");
                        }
                    }
                }
            }
            return result.ToString();
        }


        /// <summary>
        /// 返回相差的秒数
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Sec"></param>
        /// <returns></returns>
        public static int StrDateDiffSeconds(string Time, int Sec)
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(Time).AddSeconds(Sec);
            if (ts.TotalSeconds > int.MaxValue)
                return int.MaxValue;

            else if (ts.TotalSeconds < int.MinValue)
                return int.MinValue;

            return (int)ts.TotalSeconds;
        }

        /// <summary>
        /// 返回相差的分钟数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static int StrDateDiffMinutes(string time, int minutes)
        {
            if (!string.IsNullOrEmpty(time))
                return 1;

            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddMinutes(minutes);
            if (ts.TotalMinutes > int.MaxValue)
                return int.MaxValue;
            else if (ts.TotalMinutes < int.MinValue)
                return int.MinValue;

            return (int)ts.TotalMinutes;
        }

        /// <summary>
        /// 返回相差的小时数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static int StrDateDiffHours(string time, int hours)
        {
            if (!string.IsNullOrEmpty(time))
                return 1;

            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddHours(hours);
            if (ts.TotalHours > int.MaxValue)
                return int.MaxValue;
            else if (ts.TotalHours < int.MinValue)
                return int.MinValue;

            return (int)ts.TotalHours;
        }

        /// <summary>
        /// 判断字符串是否是yy-mm-dd字符串
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsDateString(string str)
        {
            return Regex.IsMatch(str, @"(\d{4})-(\d{1,2})-(\d{1,2})");
        }

        #endregion

        /// <summary>
        /// 格式化字节数字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatBytesStr(int bytes)
        {
            if (bytes > 1073741824)
                return ((double)(bytes / 1073741824)).ToString("0") + "G";

            if (bytes > 1048576)
                return ((double)(bytes / 1048576)).ToString("0") + "M";

            if (bytes > 1024)
                return ((double)(bytes / 1024)).ToString("0") + "K";

            return bytes.ToString() + "Bytes";
        }

        public static string FormatStr(string format, object obj)
        {
            return string.Format("{" + format + "}", obj);
        }

        /// <summary>
        /// 转换为静态html
        /// </summary>
        public void transHtml(string path, string outpath)
        {
            Page page = new Page();
            StringWriter writer = new StringWriter();
            page.Server.Execute(path, writer);
            FileStream fs;
            if (File.Exists(page.Server.MapPath("") + "\\" + outpath))
            {
                File.Delete(page.Server.MapPath("") + "\\" + outpath);
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            else
            {
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            byte[] bt = Encoding.Default.GetBytes(writer.ToString());
            fs.Write(bt, 0, bt.Length);
            fs.Close();
        }

        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {
            string[] userip = StrHelper.SplitString(ip, @".");

            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = StrHelper.SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                        return true;

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                            r++;
                        else
                            break;
                    }
                    else
                        break;
                }

                if (r == 4)
                    return true;
            }
            return false;
        }

        #region 数字判断的方法

        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]*$");
        }


        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");

            return false;
        }

        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
                return false;

            if (strNumber.Length < 1)
                return false;

            foreach (string id in strNumber)
            {
                if (!IsNumeric(id))
                    return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName + "文件不存在！");

            if (!overwrite && System.IO.File.Exists(destFileName))
                return false;

            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }

        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要恢复的文件名</param>
        /// <param name="backupTargetFileName">要恢复文件再次备份的名称,如果为null,则不再备份恢复文件</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!System.IO.File.Exists(backupFileName))
                    throw new FileNotFoundException(backupFileName + "文件不存在！");

                if (backupTargetFileName != null)
                {
                    if (!System.IO.File.Exists(targetFileName))
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    else
                        System.IO.File.Copy(targetFileName, backupTargetFileName, true);
                }
                System.IO.File.Delete(targetFileName);
                System.IO.File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要恢复的文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }

        /// <summary>
        /// 将字符串转换为Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);

            }
        }

        /// <summary>
        /// 是否为数值串列表，各数值间用","间隔
        /// </summary>
        /// <param name="numList"></param>
        /// <returns></returns>
        public static bool IsNumericList(string numList)
        {
            if (!string.IsNullOrEmpty(numList))
                return false;

            return IsNumericArray(numList.Split(','));
        }

        /// <summary>
        /// 产生随机码
        /// </summary>
        /// <param name="begin">开始数</param>
        /// <param name="to">截止数</param>
        /// <returns></returns>
        public static int CreateRandom(int begin, int to)
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random(Guid.NewGuid().GetHashCode());
            return ran.Next(begin, to);
        }

        #region 汉字转拼音，不建议使用

        /// <summary> 
        /// 把汉字转换成拼音(全拼) 
        /// </summary> 
        /// <param name="hzString">汉字字符串</param> 
        /// <returns>转换后的拼音(全拼)字符串</returns> 
        public static string ConvertChines(string hzString)
        {
            if (string.IsNullOrEmpty(hzString))
                return null;
            hzString = hzString.Trim().Replace(" ", "").Replace("?", "_").Replace("\\", "_").Replace("/", "_").Replace(":", "").Replace("*", "").Replace(">", "").Replace("<", "").Replace("?", "").Replace("|", "").Replace("\"", "’ˉ").Replace("(", "_").Replace(")", "_").Replace(";", "_");
            hzString = hzString.Replace("，?", ",").Replace("；?", "_").Replace("。￡", "_").Replace("”±", "").Replace("“°", "").Replace("[", "").Replace("]", "").Replace("【?", "").Replace("】?", "");
            hzString = hzString.Replace("{", "").Replace("}", "").Replace("^", "").Replace("&", "_").Replace("=", "").Replace("~", "_").Replace("@", "_").Replace("￥¤", "");

            Regex regex = new Regex(@"([a-zA-Z0-9\._]+)", RegexOptions.IgnoreCase);
            if (regex.IsMatch(hzString))
            {
                if (hzString.Equals(regex.Match(hzString).Groups[1].Value, StringComparison.OrdinalIgnoreCase))
                {
                    return hzString;
                }
            }
            // 匹配中文字符 
            regex = new Regex("^[\u4e00-\u9fa5]$");
            byte[] array = new byte[2];
            string pyString = "";
            int chrAsc = 0;
            int i1 = 0;
            int i2 = 0;
            char[] noWChar = hzString.ToCharArray();
            for (int j = 0; j < noWChar.Length; j++)
            {
                // 中文字符 
                if (regex.IsMatch(noWChar[j].ToString()))
                {
                    array = System.Text.Encoding.Default.GetBytes(noWChar[j].ToString());
                    i1 = (short)(array[0]);
                    i2 = (short)(array[1]);
                    chrAsc = i1 * 256 + i2 - 65536;
                    if (chrAsc > 0 && chrAsc < 160)
                    {
                        pyString += noWChar[j];
                    }
                    else
                    {
                        // 修正部分文字 
                        if (chrAsc == -9254) // 修正“圳”字 
                            pyString += "Zhen";
                        else
                        {
                            for (int i = (pyValue.Length - 1); i >= 0; i--)
                            {
                                if (pyValue[i] <= chrAsc)
                                {
                                    pyString += pyName[i];
                                    break;
                                }
                            }
                        }
                    }
                }
                // 非中文字符 
                else
                {
                    pyString += noWChar[j].ToString();
                }
            }
            return pyString;
        }

        /// <summary> 
        /// 定义拼音区编码数组 
        /// </summary> 
        private static int[] pyValue = new int[] 
                { 
                -20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036, 
                -20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763, 
                -19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515, 
                -19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249, 
                -19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006, 
                -19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735, 
                -18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448, 
                -18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012, 
                -17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752, 
                -17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482, 
                -17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733, 
                -16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448, 
                -16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216, 
                -16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944, 
                -15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659, 
                -15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394, 
                -15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150, 
                -15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109, 
                -14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902, 
                -14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654, 
                -14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345, 
                -14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112, 
                -14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907, 
                -13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601, 
                -13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343, 
                -13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076, 
                -13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831, 
                -12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300, 
                -12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798, 
                -11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067, 
                -11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832, 
                -10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328, 
                -10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254 
                };
        /// <summary> 
        /// 定义数组 
        /// </summary> 
        private static string[] pyName = new string[] 
                { 
                "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben", 
                "Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can", 
                "Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng", 
                "Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong", 
                "Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De", 
                "Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui", 
                "Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo", 
                "Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong", 
                "Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han", 
                "Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan", 
                "Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing", 
                "Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke", 
                "Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo", 
                "La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang", 
                "Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun", 
                "Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian", 
                "Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang", 
                "Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning", 
                "Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan", 
                "Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po", 
                "Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu", 
                "Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou", 
                "Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen", 
                "Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu", 
                "Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan", 
                "Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian", 
                "Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai", 
                "Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao", 
                "Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang", 
                "Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun", 
                "Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan", 
                "Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan", 
                "Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo" 
                };


        #endregion

        /// <summary>
        /// 获取搜索引擎名
        /// </summary>
        /// <returns></returns>
        public static string GetBot(HttpContext context)
        {
            string result = string.Empty;
            string uag = context.Request.UserAgent;
            if (!string.IsNullOrEmpty(uag) && (uag.IndexOf("bot", StringComparison.OrdinalIgnoreCase) >= 0 || uag.IndexOf("spider", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                if (uag.IndexOf("Googlebot") >= 0)
                {
                    return "Googlebot";
                }
                if (uag.IndexOf("Baiduspider") >= 0)
                {
                    return "Baiduspider";
                }
                if (uag.IndexOf("Sosospider") >= 0)
                {
                    return "Sosospider";
                }
                if (uag.IndexOf("Sogou inst spider") >= 0)
                {
                    return "Sogou inst spider";
                }
                if (uag.IndexOf("Sogou web spider") >= 0)
                {
                    return "Sogou web spider";
                }
                if (uag.IndexOf("YoudaoBot") >= 0)
                {
                    return "YoudaoBot";
                }
                if (uag.IndexOf("msnbot") >= 0)
                {
                    return "msnbot";
                }
                if (uag.IndexOf("Slurp") >= 0)
                {
                    return "yahoo";
                }
                if (uag.IndexOf("Jike") >= 0)
                {
                    return "即刻";
                }
                if (uag.IndexOf("360") >= 0)
                {
                    return "360";
                }
                return uag;
            }
            return result;
        }

        #region 日期格式转换

        /// <summary>
        /// 日期转换格式
        /// </summary>
        /// <param name="o">日期类型</param>
        /// <param name="ft">格式化：yyyy-MM-dd</param>
        /// <returns></returns>
        public static string DatetoStr(object o, string ft)
        {
            try
            {
                if (o != null)
                {
                    if (!string.IsNullOrEmpty(ft))
                    {
                        DateTime dt = Convert.ToDateTime(o);
                        return dt.ToString(ft);
                    }
                    else
                    {
                        DateTime dt = Convert.ToDateTime(o);
                        return DataConvert.DateToChina(dt);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 日期转换格式
        /// </summary>
        /// <param name="o">日期类型</param>
        /// <returns></returns>
        public static string DatetoStr(object o)
        {
            return DatetoStr(o, null);
        }

        /// <summary>
        /// 日期转换格式
        /// </summary>
        /// <param name="o">日期类型</param>
        /// <returns></returns>
        public static string DatatoStr(object o)
        {
            if (o != null)
            {
                return o.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 当前时间差额计算
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetTimeDiff(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            DateTime time = DateTime.Parse(o.ToString());
            TimeSpan ts = DateTime.Now.Subtract(time);
            if (ts.Days >= 31 && ts.Days < 365)
                return ts.Days / 30 + "个月前";
            if (ts.Days >= 1 && ts.Days < 31)
                return ts.Days + "天前";
            if (ts.Hours >= 1 && ts.Days < 1)
                return ts.Hours + "小时前";
            if (ts.Minutes >= 1 && ts.Hours < 1)
                return ts.Minutes + "分钟前";
            if (ts.Minutes < 1)
                return (ts.Seconds > 0 ? ts.Seconds : 0) + "秒";
            return "1年前";
        }

        #endregion

        #region  截断字符串

        /// <summary>
        /// 截断字符串
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="length">需要截断的长度</param>
        /// <returns></returns>
        public static string GetSubString(string s, int length)
        {
            return GetSubString(s, length, true);
        }

        /// <summary>
        /// 截断字符串
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="length">需要截断的长度</param>
        /// <param name="isend">是否添加...标记</param>
        /// <returns></returns>
        public static string GetSubString(string s, int length, bool isend)
        {
            if (!string.IsNullOrEmpty(s) && length > 0)
            {
                if (isend)
                {
                    return StrHelper.GetUnicodeSubString(s, length, "...");
                }
                else
                {
                    return StrHelper.GetUnicodeSubString(s, length, "");
                }
            }
            return s;
        }

        #endregion

        public static string Escape(string str)
        {
            if (str == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            int len = str.Length;

            for (int i = 0; i < len; i++)
            {
                char c = str[i];

                //everything other than the optionally escaped chars _must_ be escaped 
                if (Char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '/' || c == '\\' || c == '.')
                    sb.Append(c);
                else
                    sb.Append(Uri.HexEscape(c));
            }

            return sb.ToString();
        }


        public static string GetCheckKey(string str, string str2)
        {
            string key = EncryptString.DESEncode(str, str2);
            return key;
        }

        /// <summary>
        /// 时间单位转化
        /// </summary>
        /// <param name="time">时间数字表示</param>
        /// <param name="units">单位</param>
        /// <returns>具体时间</returns>
        public static long ConvertTime(int time, TimeUnits units)
        {
            long runtime = 0;
            switch (units)
            {
                case TimeUnits.Millisecond:
                    runtime = time * 1;
                    break;
                case TimeUnits.Second:
                    runtime = time * 1000;
                    break;
                case TimeUnits.Minute:
                    runtime = time * 1000 * 60;
                    break;
                case TimeUnits.Hour:
                    runtime = time * 1000 * 60 * 60;
                    break;
                case TimeUnits.Day:
                    runtime = time * 1000 * 60 * 60 * 24;
                    break;

            }
            return runtime;
        }

        /// <summary>
        ///  ip转化为数字(ipv4有效)
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long ConverIP(string ip)
        {
            try
            {
                return (long)(uint)IPAddress.NetworkToHostOrder(
               (int)IPAddress.Parse(ip).Address);
            }
            catch
            {
                return -1;
            }
            //if(string.IsNullOrEmpty(ip))
            //{
            //    return -1;
            //}
            //try
            //{
            //int ipv1 = DataConvert.SafeInt(ip.Split('.')[0]);
            //int ipv2 = DataConvert.SafeInt(ip.Split('.')[1]);
            //int ipv3 = DataConvert.SafeInt(ip.Split('.')[2]);
            //int ipv4 = DataConvert.SafeInt(ip.Split('.')[3]);

            //string[] ips = ip.Split(new char[]{'.'}, StringSplitOptions.RemoveEmptyEntries);
            //long ipvalue = -1;
            //if (ips.Length != 4)
            //{
            //    return -1;
            //}
            //try
            //{
            //    ipvalue = long.Parse(ips[3]) + long.Parse(ips[2]) * 256 + long.Parse(ips[1]) * 256 * 256 + long.Parse(ips[0]) * 256 * 256 * 256;
            //}
            //catch{
            //    return -1;
            //}
            //return ipvalue;

        }


        /// <summary>
        /// long类型转化为ip
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string ConverLongToIP(long address)
        {
            return IPAddress.Parse(address.ToString()).ToString();
        }

        /// <summary>
        /// 是否是IP格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIPv4(string str)
        {
            Regex reg = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$", RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(str))
            {
                string[] arr = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 3)
                {
                    string ip = arr[3];
                    if (reg.IsMatch(ip))
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        /// <summary>
        /// 获取字符中的纯数字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetNumberFromStr(string key)
        {
            return System.Text.RegularExpressions.Regex.Replace(key, @"[^\d]*", "");
        }

        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StrIsNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;

            return false;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!Utils.StrIsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }


        /// <summary>
        /// 获取指定文件的扩展名
        /// </summary>
        /// <param name="fileName">指定文件名</param>
        /// <returns>扩展名</returns>
        public static string GetFileExtName(string fileName)
        {
            if (Utils.StrIsNullOrEmpty(fileName) || fileName.IndexOf('.') <= 0)
                return "";

            fileName = fileName.ToLower().Trim();

            return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
        }

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                        startIndex = startIndex - length;
                }

                if (startIndex > str.Length)
                    return "";
            }
            else
            {
                if (length < 0)
                    return "";
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                        return "";
                }
            }

            if (str.Length - startIndex < length)
                length = str.Length - startIndex;

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 数据流转换为十六进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] bytes)
        {

            if (bytes == null || bytes.Length == 0)

                throw new ArgumentException("bytes [] 参数出错");

            StringBuilder hexString = new StringBuilder(2 * bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
                hexString.AppendFormat("{0:X2}", bytes[i]);

            return hexString.ToString();

        }

        /// <summary>
        /// 十六进制字符串转换为数据流
        /// </summary>
        /// <param name="strHexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string strHexString)
        {

            int len = strHexString.Length;


            if ((len % 2) != 0)
                throw new Exception("HexString 字符出错!!");

            int byteLen = len / 2;

            byte[] bytes = new byte[byteLen];

            for (int i = 0; i < byteLen; i++)
            {
                bytes[i] = Convert.ToByte(strHexString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }


        /// <summary>
        /// 指定日期属于一年中的第几周
        /// </summary>
        /// <param name="dtime"></param>
        /// <returns></returns>
        public static int Weekofyear(DateTime dtime)
        {
            int weeknum = 0;
            DateTime tmpdate = DateTime.Parse(dtime.Year.ToString() + "-1" + "-1");
            DayOfWeek firstweek = tmpdate.DayOfWeek;
            //if(firstweek) 
            for (int i = (int)firstweek + 1; i <= dtime.DayOfYear; i = i + 7)
            {
                weeknum = weeknum + 1;
            }
            return weeknum;
        }

        /// <summary>
        /// 指定日期的月份有几天
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int MonthDays(DateTime day)
        { 
            DateTime daytemp = DateTime.Now;
            if (DateTime.TryParse(day.ToString("yyyy-MM-") + "31", out daytemp))
                return 31;
            else if (DateTime.TryParse(day.ToString("yyyy-MM-") + "30", out daytemp))
                return 30;
            else if (DateTime.TryParse(day.ToString("yyyy-MM-") + "29", out daytemp))
                return 29;
            else if (DateTime.TryParse(day.ToString("yyyy-MM-") + "28", out daytemp))
                return 28;
            return 31;
        }

        #region 多线程

        public delegate void DelayRunFunc();

        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="?"></param>
        public static void DelayRun(int delay, DelayRunFunc func)
        {
            BackgroundWorker b = new BackgroundWorker();
            b.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                func();
            };
            b.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                Thread.Sleep(delay);
            };
            b.RunWorkerAsync();
        }

        #endregion
    }
}
