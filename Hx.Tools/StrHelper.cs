using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

namespace Hx.Tools
{
    public class StrHelper
    {
        #region 随机码
        /// <summary>
        /// 随机码类型
        /// </summary>
        public enum RandomType
        {
            Num,//纯数字
            Char,//纯字母
            CharAndNum//字母跟数字混合
        }

        /// <summary>
        /// 随机码生成
        /// </summary>
        /// <param name="Length">长度</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GenerateRandom(int Length, RandomType type)
        {
            char[] constant = null;
            switch (type)
            {
                case RandomType.Char:
                    constant = new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
                    break;
                case RandomType.Num:
                    constant = new char[10] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
                    break;
                case RandomType.CharAndNum:
                    constant = new char[36] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
                    break;
            }
            StringBuilder newRandom = new StringBuilder();
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(constant.Length)]);
            }
            return newRandom.ToString();
        }

        #endregion

        #region 把汉字转化成全拼音
        /// <summary> 
        /// 取单个字符的拼音声母 
        /// 2004-11-30 
        /// </summary> 
        /// <param name="c">要转换的单个汉字</param> 
        /// <returns>拼音声母</returns> 
        public static string GetPYChar(string str)
        {
            if (str.CompareTo("吖") < 0)
            {
                string s = str.Substring(0, 1).ToUpper();
                return s;

            }
            else if (str.CompareTo("八") < 0)
            {
                return "A";
            }
            else if (str.CompareTo("嚓") < 0)
            {
                return "B";
            }
            else if (str.CompareTo("咑") < 0)
            {
                return "C";
            }
            else if (str.CompareTo("妸") < 0)
            {
                return "D";
            }
            else if (str.CompareTo("发") < 0)
            {
                return "E";
            }
            else if (str.CompareTo("旮") < 0)
            {
                return "F";
            }
            else if (str.CompareTo("铪") < 0)
            {
                return "G";
            }
            else if (str.CompareTo("讥") < 0)
            {
                return "H";
            }
            else if (str.CompareTo("咔") < 0)
            {
                return "J";
            }
            else if (str.CompareTo("垃") < 0)
            {
                return "K";
            }
            else if (str.CompareTo("嘸") < 0)
            {
                return "L";
            }
            else if (str.CompareTo("拏") < 0)
            {
                return "M";
            }
            else if (str.CompareTo("噢") < 0)
            {
                return "N";
            }
            else if (str.CompareTo("妑") < 0)
            {
                return "O";
            }
            else if (str.CompareTo("七") < 0)
            {
                return "P";
            }
            else if (str.CompareTo("亽") < 0)
            {
                return "Q";
            }
            else if (str.CompareTo("仨") < 0)
            {
                return "R";
            }
            else if (str.CompareTo("他") < 0)
            {
                return "S";
            }
            else if (str.CompareTo("哇") < 0)
            {
                return "T";
            }
            else if (str.CompareTo("夕") < 0)
            {
                return "W";
            }
            else if (str.CompareTo("丫") < 0)
            {
                return "X";
            }
            else if (str.CompareTo("帀") < 0)
            {
                return "Y";
            }
            else if (str.CompareTo("咗") < 0)
            {
                return "Z";
            }
            else
            {
                return "?";
            }
        }

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

        public static string GetPYChars(string strIn)
        {
            string strOut = string.Empty;
            // 匹配中文字符
            Regex regex = new Regex("^[\u4e00-\u9fa5]$");
            char[] noWChar = strIn.ToCharArray();

            for (int j = 0; j < noWChar.Length; j++)
            {
                // 中文字符
                if (regex.IsMatch(noWChar[j].ToString()))
                {
                    strOut += GetPYChar(noWChar[j].ToString());
                }
                // 非中文字符
                else
                {
                    strOut += noWChar[j].ToString();
                }
            }
            return strOut;
        }

        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="hzString">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string ConvertE(string hzString)
        {
            // 匹配中文字符
            Regex regex = new Regex("^[\u4e00-\u9fa5]$");
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
                        if (chrAsc == -9254)  // 修正“圳”字
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

        ///   <summary>  
        ///   判断是否为汉字  
        ///   </summary>  
        ///   <param   name="chrStr">待检测字符串</param>  
        ///   <returns>是汉字返回true</returns>  
        private static bool IsChineseCharacters(string chrStr)
        {
            Regex CheckStr = new Regex("[\u4e00-\u9fa5]");
            return CheckStr.IsMatch(chrStr);
        }

        /// <summary>
        /// 得到每个汉字的字首拼音码字母(大写)
        /// </summary>
        /// <param name="chrStr">输入字符串</param>
        /// <returns>返回结果</returns>
        public static string GetHeadCharacters(string chrStr)
        {
            string strHeadString = string.Empty;

            Encoding gb = System.Text.Encoding.GetEncoding("gb2312");
            for (int i = 0; i < chrStr.Length; i++)
            {
                //检测该字符是否为汉字
                if (!IsChineseCharacters(chrStr.Substring(i, 1)))
                {
                    strHeadString += chrStr.Substring(i, 1);
                    continue;
                }

                byte[] bytes = gb.GetBytes(chrStr.Substring(i, 1));
                string lowCode = System.Convert.ToString(bytes[0] - 0xA0, 16);
                string hightCode = System.Convert.ToString(bytes[1] - 0xA0, 16);
                int nCode = Convert.ToUInt16(lowCode, 16) * 100 + Convert.ToUInt16(hightCode, 16);      //得到区位码
                strHeadString += FirstLetter(nCode);
            }
            return strHeadString;
        }

        /// <summary>
        /// 通过汉字区位码得到其首字母(大写)
        /// </summary>
        /// <param name="nCode">汉字编码</param>
        /// <returns></returns>
        private static string FirstLetter(int nCode)
        {
            if (nCode >= 1601 && nCode < 1637) return "A";
            if (nCode >= 1637 && nCode < 1833) return "B";
            if (nCode >= 1833 && nCode < 2078) return "C";
            if (nCode >= 2078 && nCode < 2274) return "D";
            if (nCode >= 2274 && nCode < 2302) return "E";
            if (nCode >= 2302 && nCode < 2433) return "F";
            if (nCode >= 2433 && nCode < 2594) return "G";
            if (nCode >= 2594 && nCode < 2787) return "H";
            if (nCode >= 2787 && nCode < 3106) return "J";
            if (nCode >= 3106 && nCode < 3212) return "K";
            if (nCode >= 3212 && nCode < 3472) return "L";
            if (nCode >= 3472 && nCode < 3635) return "M";
            if (nCode >= 3635 && nCode < 3722) return "N";
            if (nCode >= 3722 && nCode < 3730) return "O";
            if (nCode >= 3730 && nCode < 3858) return "P";
            if (nCode >= 3858 && nCode < 4027) return "Q";
            if (nCode >= 4027 && nCode < 4086) return "R";
            if (nCode >= 4086 && nCode < 4390) return "S";
            if (nCode >= 4390 && nCode < 4558) return "T";
            if (nCode >= 4558 && nCode < 4684) return "W";
            if (nCode >= 4684 && nCode < 4925) return "X";
            if (nCode >= 4925 && nCode < 5249) return "Y";
            if (nCode >= 5249 && nCode < 5590) return "Z";
            return "";
        }

        #endregion

        #region 特殊字符连接字符串
        //在字符末尾追加字符串，并以‘，’分隔
        public static void AppendString(StringBuilder sb, string append)
        {
            AppendString(sb, append, ",");
        }

        /// <summary>
        /// 在字符末尾追加字符串
        /// </summary>
        /// <param name="sb">StringBuilder对象</param>
        /// <param name="append">需要添加的字符串</param>
        /// <param name="split">分隔符</param>
        public static void AppendString(StringBuilder sb, string append, string split)
        {
            if (sb.Length == 0)
            {
                sb.Append(append);
            }
            else
            {
                sb.Append(split);
                sb.Append(append);
            }
        }
        #endregion

        #region 去除字符串前后的特殊字符

        /// <summary>
        /// 去除字符串前面的特殊字符
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="rmstr">特殊字符</param>
        /// <returns></returns>
        public static string TrimBegin(string str, char rmstr)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.TrimStart(rmstr);
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 去除字符串的前面空字符
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns></returns>
        public static string TrimBegin(string str)
        {
            return TrimBegin(str, ' ');
        }
        /// <summary>
        /// 去除字符串后面的特殊字符
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="rmstr">特殊字符</param>
        /// <returns></returns>
        public static string TrimEnd(string str, char rmstr)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.TrimEnd(rmstr);
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 去除字符串前后的特殊字符
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="rmstr">特殊字符</param>
        /// <returns></returns>
        public static string Trim(string str, char rmstr)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Trim(rmstr);
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 去除字符串前后的空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Trim(string str)
        {
            return Trim(str, ' ');
        }
        #endregion

        #region 特殊字符处理
        /// <summary>
        /// 替换sql中的特殊字符
        /// </summary>
        /// <param name="sqlStr">源字符串</param>
        /// <returns></returns>
        public static string FilterSql(string sqlStr)
        {
            return Regex.Replace(sqlStr, "--|;|'|\"", "", RegexOptions.Compiled);
        }

        /// <summary>
        /// 字符串过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FiterStr(string str)
        {
            str = str.Replace("&", "&amp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt");
            str = str.Replace("'", "''");
            str = str.Replace("*", "");
            str = str.Replace("\n", "<br/>");
            str = str.Replace("\r\n", "<br/>");
            //str   =   str.Replace("?","");   
            str = str.Replace("select", "");
            str = str.Replace("insert", "");
            str = str.Replace("update", "");
            str = str.Replace("delete", "");
            str = str.Replace("create", "");
            str = str.Replace("drop", "");
            str = str.Replace("delcare", "");
            str = str.Replace("   ", "&nbsp;");
            str = str.Replace("<script>", "");
            str = str.Replace("SCRIPT", "");
            str = str.Trim();
            if (str.Trim().ToString() == "")
                str = "无";
            return str;
        }

        public static string FilterStr(string strIn)
        {
            if (string.IsNullOrEmpty(strIn))
            {
                return string.Empty;
            }
            string strOut = string.Empty;
            strOut = strIn.Replace("\"", "“")
                .Replace("'", "’");

            return strOut;
        }

        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            return Regex.Replace(content, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        /// 从HTML中获取文本
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {

            System.Text.RegularExpressions.Regex regEx0 = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regEx1 = new System.Text.RegularExpressions.Regex(@"<[^>]+>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regEx2 = new System.Text.RegularExpressions.Regex(@"\[[^\]]+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return regEx2.Replace(regEx1.Replace(regEx0.Replace(HTML, ""), ""), "");
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        public static bool IsValidDoEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        public static string GetEmailHostName(string strEmail)
        {
            if (strEmail.IndexOf("@") < 0)
            {
                return "";
            }
            return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        #endregion

        #region 字符串截断

        /// <summary>
        /// 截取字符串长度
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="len">截取的字符串长度（其他用'...'表示）</param>
        /// <returns></returns>
        public static String GetFuzzyChar(String str, Int32 len)
        {
            return GetFuzzyChar(str, len, "...");
        }

        /// <summary>
        /// 截取字符串长度
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="len">截取的字符串长度（其他用'...'表示）</param>
        /// <param name="sp">截断部分的表示字符</param>
        /// <returns></returns>
        public static String GetFuzzyChar(string str, Int32 len, string sp)
        {
            if (str.Length > len)
            {
                str = str.Substring(0, len);
                str = str + sp;
                return str;
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }

        public static string GetUnicodeSubString(string str, int len, string p_TailString)
        {
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }

                if (pos >= 0)
                    result = str.Substring(0, pos) + p_TailString;
            }
            else
                result = str;

            return result;
        }


        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;

            Byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
            foreach (char c in Encoding.UTF8.GetChars(bComments))
            {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                {
                    //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                    //当截取的起始位置超出字段串长度时
                    if (p_StartIndex >= p_SrcString.Length)
                        return "";
                    else
                        return p_SrcString.Substring(p_StartIndex,
                                                       ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }

            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }

                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                            nFlag = 0;

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        nRealLength = p_Length + 1;

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
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
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        #endregion

        #region 字符串操作
        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        /// 替换关键字
        /// </summary>
        /// <param name="SearchName">查找到的内容</param>
        /// <param name="key">将被替换的关键字</param>
        /// <returns></returns>
        public static string KeyReplace(string SearchName, string key)
        {
            if (SearchName != "" && SearchName != null && key != "" && key != null)
            {
                string keyFormat = "<font color=red>" + key + "</font>";
                return SearchName.Replace(key, keyFormat);
            }

            else
            {
                return SearchName;
            }
        }

        /// <summary>
        /// 将String[]转换为int[]
        /// </summary>
        /// <param name="StrArray">字符串数组</param>
        /// <returns></returns>
        public static int[] IntArray(String[] StrArray)
        {
            Int32 j = StrArray.Length;
            Int32[] nums = new Int32[j];
            for (int i = 0; i < j; i = i + 1)
            {
                nums[i] = Int32.Parse(StrArray[i]);
            }
            return nums;
        }

        /// <summary>
        /// 判断源字符串中是否包含给定的字符
        /// </summary>
        /// <param name="strValue">源字符串</param>
        /// <param name="strKey">查找的字符</param>
        /// <returns>是否包含</returns>
        public static bool IsInclude(string strValue, string strKey)
        {
            if (0 <= strValue.IndexOf(strKey))
                return true;
            else
                return false;

        }

        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        /// <param name="SourceString">源字符串</param>
        /// <param name="SearchString">查找的字符串</param>
        /// <param name="ReplaceString">替换的字符串</param>
        /// <param name="IsCaseInsensetive">是否忽略大小写</param>
        /// <returns></returns>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StrToFirstUp(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                s = textInfo.ToTitleCase(s);
            }
            return s;
        }
        #endregion

        #region InArray
        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                        return i;
                }
                else if (strSearch == stringArray[i])
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }
        #endregion

        #region 字符串分割
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];
            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <param name="maxElementLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);

            return ignoreRepeatItem ? DistinctStringArray(result, maxElementLength) : result;
        }


        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <param name="minElementLength">单个元素最小长度</param>
        /// <param name="maxElementLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int minElementLength, int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);

            if (ignoreRepeatItem)
            {
                result = DistinctStringArray(result);
            }
            return PadStringArray(result, minElementLength, maxElementLength);
        }

        /// <summary>
        /// 进行指定的替换(脏字过滤)
        /// </summary>
        public static string StrFilter(string str, string bantext)
        {
            string text1 = "", text2 = "";
            string[] textArray1 = SplitString(bantext, "\r\n");
            for (int num1 = 0; num1 < textArray1.Length; num1++)
            {
                text1 = textArray1[num1].Substring(0, textArray1[num1].IndexOf("="));
                text2 = textArray1[num1].Substring(textArray1[num1].IndexOf("=") + 1);
                str = str.Replace(text1, text2);
            }
            return str;
        }

        /// <summary>
        /// 过滤字符串数组中每个元素为合适的大小
        /// 当长度小于minLength时，忽略掉,-1为不限制最小长度
        /// 当长度大于maxLength时，取其前maxLength位
        /// 如果数组中有null元素，会被忽略掉
        /// </summary>
        /// <param name="minLength">单个元素最小长度</param>
        /// <param name="maxLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] PadStringArray(string[] strArray, int minLength, int maxLength)
        {
            if (minLength > maxLength)
            {
                int t = maxLength;
                maxLength = minLength;
                minLength = t;
            }

            int iMiniStringCount = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (minLength > -1 && strArray[i].Length < minLength)
                {
                    strArray[i] = null;
                    continue;
                }
                if (strArray[i].Length > maxLength)
                    strArray[i] = strArray[i].Substring(0, maxLength);

                iMiniStringCount++;
            }

            string[] result = new string[iMiniStringCount];
            for (int i = 0, j = 0; i < strArray.Length && j < result.Length; i++)
            {
                if (strArray[i] != null && strArray[i] != string.Empty)
                {
                    result[j] = strArray[i];
                    j++;
                }
            }
            return result;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem)
        {
            return SplitString(strContent, strSplit, ignoreRepeatItem, 0);
        }

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <param name="maxElementLength">字符串数组中单个元素的最大长度</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray, int maxElementLength)
        {
            Hashtable h = new Hashtable();

            foreach (string s in strArray)
            {
                string k = s;
                if (maxElementLength > 0 && k.Length > maxElementLength)
                {
                    k = k.Substring(0, maxElementLength);
                }
                h[k.Trim()] = s;
            }

            string[] result = new string[h.Count];

            h.Keys.CopyTo(result, 0);

            return result;
        }

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray)
        {
            return DistinctStringArray(strArray, 0);
        }

        #endregion

        public static bool IsStrIn(string source, string va, string splict)
        {
            string[] arr = SplitString(source, splict);
            return GetInArrayID(va, arr, true) >= 0;
        }

        /// <summary>
        /// 获取数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetNumber(String str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsNumber(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static bool ComperStr(string adids, string areaids)
        {
            if (adids == null || string.IsNullOrEmpty(areaids))
            {
                return true;
            }
            string[] arr = areaids.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string a in arr)
            {
                if (adids.IndexOf("|" + a + "|") >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWord(string lsstr)
        {
            Regex r = new Regex(@"([a-zA-Z]+)");
            if (r.IsMatch(lsstr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将html转化为js
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConverHtmlToJs(string s)
        {
            return String.Format("document.write(\"{0}\");",
            String.Join("\\\r\n", s.Replace("\\", "\\\\")
                                    .Replace("/", "\\/")
                                    .Replace("'", "\\'")
                                    .Replace("\"", "\\\"")
                                    .Split(new char[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries)));
        }

        #region 弃用

        public static string DateToChina(DateTime dl, DateTime d2)
        {
            TimeSpan ts = dl.Subtract(d2);
            string result = null;
            if (ts.Ticks > 0)
            {
                result = "{0}之后";
            }
            else
            {
                result = "{0}前";
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
                            result = "刚才";
                        }
                    }
                }
            }
            return result.ToString();
        }

        ////将整数数组转换成字符串，加“，”为分隔符
        ///// <summary>
        ///// 将整数数组转换成字符串，加“，”为分隔符
        ///// </summary>
        ///// <param name="Str"></param>
        ///// <returns></returns>
        //public static String ComStr(int[] Str)
        //{
        //    String Str1 = "";
        //    foreach (int i in Str)
        //    {
        //        Str1 = Str1 + i.ToString() + ",";
        //    }
        //    return Str1;
        //}

        ///// <summary>
        ///// 获取拆分后的字符串列表。
        ///// </summary>
        ///// <param name="mOrigianlString"></param>
        ///// <param name="subStringCharNumber"></param>
        ///// <returns></returns>
        //public static ArrayList GetSeparateSubString(string mOrigianlString, int subStringCharNumber)
        //{
        //    ArrayList resultList = new ArrayList();
        //    string tempStr = mOrigianlString;
        //    int charNumber = subStringCharNumber;
        //    int totalCount = 0;
        //    string mSubStr = "";
        //    for (int i = 0; i < tempStr.Length; i++)
        //    {
        //        string mChar = tempStr.Substring(i, 1);
        //        int byteCount = Encoding.Default.GetByteCount(mChar);//关键点判断字符所占的字节数

        //        if (byteCount == 1)
        //        {
        //            totalCount++;
        //            mSubStr += mChar;
        //            if (totalCount == charNumber || i == tempStr.Length - 1)
        //            {
        //                resultList.Add(mSubStr);
        //                totalCount = 0;
        //                mSubStr = "";
        //            }
        //        }
        //        else if (byteCount > 1)
        //        {

        //            totalCount += 2;
        //            if (totalCount > charNumber)
        //            {
        //                resultList.Add(mSubStr);
        //                if (i == tempStr.Length - 1)
        //                {
        //                    mSubStr = mChar;
        //                    resultList.Add(mSubStr);
        //                }
        //                else
        //                {
        //                    totalCount = 2;
        //                    mSubStr = mChar;
        //                }
        //            }
        //            else if (totalCount == charNumber)
        //            {
        //                mSubStr += mChar;
        //                resultList.Add(mSubStr);
        //                totalCount = 0;
        //                mSubStr = "";
        //            }
        //            else if (i == tempStr.Length - 1)
        //            {
        //                mSubStr += mChar;
        //                resultList.Add(mSubStr);
        //            }
        //            else
        //            {
        //                mSubStr += mChar;
        //            }
        //        }
        //    }
        //    return resultList;
        //}


        public static string PostClassNameGet(int ClassID)
        {
            switch (ClassID)
            {
                case 0:
                    {
                        return "分析";
                        //break;
                    }
                case 1:
                    {
                        return "疑问";
                        // break;
                    }
                case 2:
                    {
                        return "讨论";
                        // break;
                    }
                case 3:
                    {
                        return "投票";
                        // break;
                    }
                default:
                    {
                        return "暂无分类";
                    }
            }
        }

        ///// <summary>
        ///// 按指定长度截取字符串(以字节计算长度)
        ///// </summary>
        ///// <param name="stringToSub"></param>
        ///// <param name="length"></param>
        ///// <returns></returns>
        //public static string GetFirstString(string stringToSub, int length)
        //{
        //    if (System.Text.Encoding.Default.GetByteCount(stringToSub) <= length)
        //        return stringToSub;

        //    Regex regex = new Regex("[\u4e00-\u9fa5]+", RegexOptions.Compiled);
        //    char[] stringChar = stringToSub.ToCharArray();
        //    StringBuilder sb = new StringBuilder();
        //    int nLength = 0;
        //    bool isCut = false;
        //    for (int i = 0; i < stringChar.Length; i++)
        //    {
        //        if (regex.IsMatch((stringChar[i]).ToString()))
        //        {
        //            sb.Append(stringChar[i]);
        //            nLength += 2;
        //        }
        //        else
        //        {
        //            sb.Append(stringChar[i]);
        //            nLength = nLength + 1;
        //        }

        //        if (nLength > length)
        //        {
        //            isCut = true;
        //            break;
        //        }
        //    }
        //    if (isCut)
        //        return sb.ToString() + ""; //可加上省略号
        //    else
        //        return sb.ToString();
        //}


        ///// <summary>
        ///// 字符串分解函数
        ///// </summary>
        ///// <param name="str">要分解的字符串</param>
        ///// <param name="splitstr">分割符,可以为string类型(多个字符)</param>
        ///// <returns>字符数组</returns>
        //public static String[] SplitStrs(String str, String splitstr)
        //{
        //    if (splitstr != "")
        //    {
        //        System.Collections.ArrayList c = new System.Collections.ArrayList();

        //        while (true)
        //        {
        //            int thissplitindex = str.IndexOf(splitstr);
        //            if (thissplitindex >= 0)
        //            {
        //                c.Add(str.Substring(0, thissplitindex));
        //                str = str.Substring(thissplitindex + splitstr.Length);
        //            }
        //            else
        //            {
        //                c.Add(str);
        //                break;
        //            }
        //        }

        //        String[] d = (String[])c.ToArray(typeof(String));
        //        return d;
        //    }
        //    else
        //    {
        //        return new String[] { str };
        //    }
        //}

        ///// <summary>
        ///// 按分隔符（SplitChar）分割字符串（Str）为数组（SplitStr）
        ///// </summary>
        ///// <param name="Str"></param>
        ///// <param name="SplitChar"></param>
        ///// <returns></returns>
        //public static String[] SplitStr(String Str, String SplitChar)
        //{
        //    String[] SplitedString = System.Text.RegularExpressions.Regex.Split(Str, SplitChar);
        //    return SplitedString;
        //}

        ///// <summary>
        ///// 将字符数组转换为字符串
        ///// </summary>
        ///// <param name="Str"></param>
        ///// <returns></returns>
        //public static String ComStr(String[] Str)
        //{
        //    String Str1 = "";
        //    foreach (string i in Str)
        //    {
        //        Str1 = Str1 + i.ToString();
        //    }
        //    return Str1;

        //}

        #endregion

        /// <summary>
        /// 获取中文字符
        /// </summary>
        /// <param name="oriText"></param>
        /// <returns></returns>
        public static string GetChineseWord(string oriText)
        {
            string x = @"[\u4E00-\u9FFF]+";
            MatchCollection Matches = Regex.Matches(oriText, x, RegexOptions.IgnoreCase);
            StringBuilder sb = new StringBuilder();
            foreach (Match NextMatch in Matches)
            {
                sb.Append(NextMatch.Value);
            }
            return sb.ToString();
        }

        #region 计算相似度

        /// <summary>
        /// 取最小的一位数
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <returns></returns>
        private static int LowerOfThree(int first, int second, int third)
        {
            int min = Math.Min(first, second);
            return Math.Min(min, third);
        }

        public static int Levenshtein_Distance(string str1, string str2)
        {
            int[,] Matrix;
            int n = str1.Length;
            int m = str2.Length;

            int temp = 0;
            char ch1;
            char ch2;
            int i = 0;
            int j = 0;
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {

                return n;
            }
            Matrix = new int[n + 1, m + 1];

            for (i = 0; i <= n; i++)
            {
                //初始化第一列
                Matrix[i, 0] = i;
            }

            for (j = 0; j <= m; j++)
            {
                //初始化第一行
                Matrix[0, j] = j;
            }

            for (i = 1; i <= n; i++)
            {
                ch1 = str1[i - 1];
                for (j = 1; j <= m; j++)
                {
                    ch2 = str2[j - 1];
                    if (ch1.Equals(ch2))
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    Matrix[i, j] = LowerOfThree(Matrix[i - 1, j] + 1, Matrix[i, j - 1] + 1, Matrix[i - 1, j - 1] + temp);
                }
            }
            for (i = 0; i <= n; i++)
            {
                for (j = 0; j <= m; j++)
                {
                    Console.Write(" {0} ", Matrix[i, j]);
                }
                Console.WriteLine("");
            }

            return Matrix[n, m];
        }

        /// <summary>
        /// 计算字符串相似度
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static decimal LevenshteinDistancePercent(string str1, string str2)
        {
            //int maxLenth = str1.Length > str2.Length ? str1.Length : str2.Length;
            int val = Levenshtein_Distance(str1, str2);
            return 1 - (decimal)val / Math.Max(str1.Length, str2.Length);
        }

        #endregion

        #region 金额格式化

        public static string FormatMoney(string num)
        {
            string newstr = string.Empty;
            Regex r = new Regex(@"(-?\d+?)(\d{3})*(\.\d+|$)");
            Match m = r.Match(num);
            newstr += m.Groups[1].Value;
            for (int i = 0; i < m.Groups[2].Captures.Count; i++)
            {
                newstr += "," + m.Groups[2].Captures[i].Value;
            }
            newstr += m.Groups[3].Value;
            return newstr;
        }

        #endregion
    }
}
