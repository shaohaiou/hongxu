using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.IO.Compression;
using Hx.Components.Entity;
using Hx.Tools;
using Hx.Car.Entity;
using Hx.Components.Data;

namespace Cardt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        string datastr1 = "";
        private Thread t;
        private void button1_Click(object sender, EventArgs e)
        {
            t = new Thread(new ThreadStart(DoCollect));
            t.Start();
        }

        /// <summary>
        /// 执行采集
        /// </summary>
        private void DoCollect()
        {
            button1.Enabled = false;
            //设置数据库连接字符串
            datastr1 = string.Format("Server={0};UID={1};PWD={2};database={3};", textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text);
            //设置采集ID范围
            int n1 = Convert.ToInt32(textBox1.Text.Trim());
            int n2 = Convert.ToInt32(textBox2.Text.Trim());
            int insertcount = 0;    //采集成功数
            int docount = 0;        //已采集数量，用于计算采集进度
            int allnum = n2 - n1 + 1;   //范围内总采集数，用于计算采集进度
            for (int k = n1; k <= n2; k++)
            {
                try
                {
                    lblStatus.Text = string.Format("正在抓取 {0}", k);

                    string strDetail = string.Empty;
                    string strContent = gethttp("http://www.autohome.com.cn/spec/" + k + "/config.html", 3000, 0, 2, false, 1000);

                    //获取数值
                    Regex r1 = new Regex(string.Format(@"""specid"":{0},""value"":""([\s\S]*?)""", k), RegexOptions.None);
                    foreach (Match m in r1.Matches(strContent))
                    {
                        string Str = m.Groups[1].ToString();
                        strContent = strContent.Replace(m.Groups[0].ToString(), Str);
                        strDetail += Str + "|";
                    }

                    //获取颜色
                    string strColor = string.Empty;
                    Regex r2 = new Regex(string.Format(string.Format(@"""specid"":{0},""coloritems"":\[([\s\S]*?)]", k), RegexOptions.None));
                    foreach (Match m in r2.Matches(strContent))
                    {
                        strColor = m.Groups[1].ToString();
                        strContent = strContent.Replace(m.Groups[0].ToString(), strColor);
                    }
                    string color = string.Empty;
                    Regex r3 = new Regex(@"""name"":""([\s\S]*?)"",""value"":""([\s\S]*?)""", RegexOptions.None);
                    foreach (Match m in r3.Matches(strColor))
                    {
                        string Str = m.Groups[1].ToString();
                        string Strval = m.Groups[2].ToString();
                        strContent = strContent.Replace(m.Groups[0].ToString(), Str);
                        color += Str + "," + Strval + "|";
                    }

                    if (!string.IsNullOrEmpty(strDetail))
                    {
                        string[] detail = strDetail.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                        #region 添加车辆信息

                        CarInfo car = new CarInfo();
                        car.cCxmc = detail[0];
                        car.fZdj = DataConvert.SafeDecimal(detail[1].Replace("万", string.Empty));
                        car.cChangs = detail[2];
                        car.cJibie = detail[3];
                        car.cFdj = detail[4];
                        car.cBsx = detail[5];
                        car.cCkg = detail[6];
                        car.cCsjg = detail[7];
                        car.cZgcs = detail[8];
                        car.cGfjs = detail[9];
                        car.cScjs = detail[10];
                        car.cSczd = detail[11];
                        car.cScyh = detail[12];
                        car.cGxbyh = detail[13];
                        car.cZczb = detail[14];
                        car.cChang = detail[15];
                        car.cKuan = detail[16];
                        car.cGao = detail[17];
                        car.cZhouju = detail[18];
                        car.cQnju = detail[19];
                        car.cHnju = detail[20];
                        car.cLdjx = detail[21];
                        car.cZbzl = detail[22];
                        car.cChesjg = detail[23];
                        car.cCms = detail[24];
                        car.cZws = detail[25];
                        car.cYxrj = detail[26];
                        car.cXlxrj = detail[27];
                        car.cFdjxh = detail[28];
                        car.fPail = detail[29];
                        car.cJqxs = detail[31];
                        car.cQgpl = detail[32];
                        car.fQgs = DataConvert.SafeInt(detail[33]);
                        car.cQms = detail[34];
                        car.cYsb = detail[35];
                        car.cPqjg = detail[36];
                        car.cGangj = detail[37];
                        car.cChongc = detail[38];
                        car.cZdml = detail[39];
                        car.cZdgl = detail[40];
                        car.cZhuans = detail[41];
                        car.cZdlz = detail[42];
                        car.cLzzs = detail[43];
                        car.cTyjs = detail[44];
                        car.cRlxs = detail[45];
                        car.cRybh = detail[46];
                        car.cGyfs = detail[47];
                        car.cGgcl = detail[48];
                        car.cGtcl = detail[49];
                        car.cHbbz = detail[50];
                        car.cJianc = detail[51];
                        car.cDwgs = detail[52];
                        car.cBsxlx = detail[53];
                        car.cQdfs = detail[54];
                        car.cQxglx = detail[55];
                        car.cHxglx = detail[56];
                        car.cZllx = detail[57];
                        car.cCtjg = detail[58];
                        car.cQzdq = detail[59];
                        car.cHzdq = detail[60];
                        car.cZczd = detail[61];
                        car.cQnt = detail[62];
                        car.cHnt = detail[63];
                        car.cBetai = detail[64];
                        car.cJszqls = "-";
                        car.cFjsqls = "-";
                        if (detail[65].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                        {
                            car.cJszqls = detail[65].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("主", string.Empty).Trim();
                            car.cFjsqls = detail[65].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("副", string.Empty).Trim();
                        }
                        car.cQpcql = "-";
                        car.cHpcql = "-";
                        if (detail[66].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                        {
                            car.cQpcql = detail[66].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                            car.cHpcql = detail[66].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                        }
                        car.cQptb = "-";
                        car.cHptb = "-";
                        if (detail[67].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                        {
                            car.cQptb = detail[67].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                            car.cHptb = detail[67].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                        }
                        car.cQbql = detail[68];
                        car.cTyjc = detail[69];
                        car.cLty = detail[70];
                        car.cHqdts = detail[71];
                        car.cIso = detail[72].IndexOf("iso", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cLatch = detail[72].IndexOf("latch", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cFdjfd = detail[73];
                        car.cCnzks = detail[74];
                        car.cYkys = detail[75];
                        car.cWysqd = detail[76];
                        car.cAbs = detail[78];
                        car.cCbc = detail[79];
                        car.cBa = detail[80];
                        car.cTrc = detail[81];
                        car.cVsc = detail[82];
                        car.cZdzc = detail[83];
                        car.cDphj = detail[84];
                        car.cKbxg = detail[85];
                        car.cKqxg = detail[86];
                        car.cKbzxb = detail[87];
                        car.cZdtc = detail[91];
                        car.cQjtc = detail[92];
                        car.cYdwg = detail[93];
                        car.cLhjn = detail[94];
                        car.cDdxhm = detail[95];
                        car.cZpfxp = detail[96];
                        car.cSxtj = detail[97].IndexOf("上下", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cQhtj = detail[97].IndexOf("前后", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cDdtj = detail[98];
                        car.cDgnfxp = detail[99];
                        car.cFxphd = detail[100];
                        car.cDsxh = detail[102];
                        car.cBcfz = detail[103];
                        car.cDcsp = detail[104];
                        car.cDnxsp = detail[105];
                        car.cHud = detail[106];
                        car.cZpzy = detail[107];
                        car.cYdzy = detail[108];
                        car.cZygd = detail[109];
                        car.cYbzc = detail[110];
                        car.cJbzc = detail[111];
                        car.cQpddtj = detail[112];
                        car.cEpjdtj = detail[113];
                        car.cEpzyyd = detail[114];
                        car.cHptj = detail[115];
                        car.cDdjy = detail[116];
                        car.cQpzyjr = "-";
                        car.cHpzyjr = "-";
                        if (detail[117].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                        {
                            car.cQpzyjr = detail[117].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                            car.cHpzyjr = detail[117].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                        }
                        car.cZytf = detail[118];
                        car.cZyam = detail[119];
                        car.cHpztpf = detail[120].Trim() == "整体放倒" ? "●" : "-";
                        car.cHpblpf = detail[120].Trim() == "比例放倒" ? "●" : "-";
                        car.cSpzy = detail[121];
                        car.cQzfs = "-";
                        car.cHzfs = "-";
                        if (detail[122].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                        {
                            car.cQzfs = detail[122].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                            car.cHzfs = detail[122].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                        }
                        car.cHphj = detail[123];
                        car.cDdhbx = detail[124];
                        car.cGps = detail[125];
                        car.cDwfw = detail[126];
                        car.cCsdp = detail[127];
                        car.cNzyp = detail[128];
                        car.cCzdh = detail[129];
                        car.cCzds = detail[130];
                        car.cHpyjp = detail[131];
                        car.cIpod = detail[132];
                        car.cMp3 = detail[133];
                        car.cDdcd = detail[134].IndexOf("单碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cDdcd = detail[134].IndexOf("选装单碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDdcd;
                        car.cXndd = detail[134].IndexOf("虚拟多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cXndd = detail[134].IndexOf("选装虚拟多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cXndd;
                        car.cDuodcd = (detail[134].IndexOf("多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 && detail[134].IndexOf("虚拟多碟CD", StringComparison.OrdinalIgnoreCase) < 0 && detail[134].IndexOf("选装虚拟多碟CD", StringComparison.OrdinalIgnoreCase) < 0) ? "●" : "-";
                        car.cDuodcd = detail[134].IndexOf("选装多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDuodcd;
                        car.cDddvd = detail[134].IndexOf("单碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cDddvd = detail[134].IndexOf("选装单碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDddvd;
                        car.cDuodvd = detail[134].IndexOf("多碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cDuodvd = detail[134].IndexOf("选装多碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDuodvd;
                        car.c23lb = detail[135].IndexOf("2-3喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.c45lb = detail[135].IndexOf("4-5喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.c67lb = detail[135].IndexOf("6-7喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.c8lb = detail[135].IndexOf("≥8喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                        car.cXqdd = detail[136];
                        car.cLed = detail[137];
                        car.cRjxcd = detail[138];
                        car.cZdtd = detail[139];
                        car.cZxtd = detail[140];
                        car.cQwd = detail[141];
                        car.cGdkt = detail[142];
                        car.cQxzz = detail[143];
                        car.cCnfwd = detail[144];
                        car.cQddcc = "-";
                        car.cHddcc = "-";
                        if (detail[145].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                        {
                            car.cQddcc = detail[145].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                            car.cHddcc = detail[145].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                        }
                        car.cFjs = detail[146];
                        car.cFzwx = detail[147];
                        car.cHsjtj = detail[148];
                        car.cHsjjr = detail[149];
                        car.cFxm = detail[150];
                        car.cZdzd = detail[151];
                        car.cHsjjy = detail[152];
                        car.cHfdz = detail[153];
                        car.cHpcz = detail[154];
                        car.cHzj = detail[156];
                        car.cHys = detail[157];
                        car.cGyys = detail[158];
                        car.cSdkt = detail[159] == "手动●" ? "●" : "-";
                        car.cZdkt = detail[159] == "自动●" ? "●" : "-";
                        car.cDlkt = detail[160];
                        car.cHzcfk = detail[161];
                        car.cWdkz = detail[162];
                        car.cKqtj = detail[163];
                        car.cCzbx = detail[164];
                        car.cPcrw = detail[165];
                        car.cBxfz = detail[167];
                        car.cZdsc = detail[169];
                        car.cZtzx = detail[170];
                        car.cYsxt = detail[171];
                        car.cFpxs = detail[172];
                        car.cZsyxh = detail[173];
                        car.cQjsxt = detail[174];

                        car.cQcys = color;

                        string sql = @"
                        IF NOT EXISTS(SELECT ID FROM t_QcCs WHERE cCxmc = @cCxmc)
                        BEGIN
                            INSERT INTO t_QcCs(
                            [cCxmc],[fZdj],[cChangs],[cJibie],[cFdj],[cBsx],[cCkg],[cCsjg],[cZgcs],[cGfjs],[cScjs],[cSczd],[cScyh],[cGxbyh],[cZczb],[cChang],[cKuan],[cGao],[cZhouju],[cQnju],[cHnju],[cLdjx],[cZbzl],[cChesjg]
                            ,[cCms],[cZws],[cYxrj],[cXlxrj],[cFdjxh],[fPail],[cJqxs],[cQgpl],[fQgs],[cQms],[cYsb],[cPqjg],[cGangj],[cChongc],[cZdml],[cZdgl],[cZhuans],[cZdlz],[cLzzs],[cTyjs],[cRlxs],[cRybh],[cGyfs],[cGgcl],[cGtcl],[cHbbz]
                            ,[cJianc],[cDwgs],[cBsxlx],[cQdfs],[cQxglx],[cHxglx],[cZllx],[cCtjg],[cQzdq],[cHzdq],[cZczd],[cQnt],[cHnt],[cBetai]
                            ,[cJszqls],[cFjsqls],[cQpcql],[cHpcql],[cQptb],[cHptb],[cQbql],[cTyjc],[cLty],[cHqdts],[cIso],[cLatch],[cFdjfd],[cCnzks]
                            ,[cYkys],[cWysqd],[cAbs],[cCbc],[cBa],[cTrc],[cVsc],[cZdzc],[cDphj],[cKbxg],[cKqxg],[cKbzxb],[cZdtc],[cQjtc],[cYdwg],[cLhjn]
                            ,[cDdxhm],[cZpfxp],[cSxtj],[cQhtj],[cDdtj],[cDgnfxp],[cFxphd],[cDsxh],[cBcfz],[cDcsp],[cDnxsp],[cHud],[cZpzy],[cYdzy],[cZygd]
                            ,[cYbzc],[cJbzc],[cQpddtj],[cEpjdtj],[cEpzyyd],[cHptj],[cDdjy],[cQpzyjr],[cHpzyjr],[cZytf],[cZyam],[cHpztpf],[cHpblpf],[cSpzy]
                            ,[cQzfs],[cHzfs],[cHphj],[cDdhbx],[cGps],[cDwfw],[cCsdp],[cNzyp],[cCzdh],[cCzds],[cHpyjp],[cIpod],[cMp3],[cDdcd],[cXndd],[cDuodcd]
                            ,[cDddvd],[cDuodvd],[c23lb],[c45lb],[c67lb],[c8lb],[cXqdd],[cLed],[cRjxcd],[cZdtd],[cZxtd],[cQwd],[cGdkt],[cQxzz],[cCnfwd]
                            ,[cQddcc],[cHddcc],[cFjs],[cFzwx],[cHsjtj],[cHsjjr],[cFxm],[cZdzd],[cHsjjy],[cHfdz],[cHpcz],[cHzj],[cHys],[cGyys],[cSdkt]
                            ,[cZdkt],[cDlkt],[cHzcfk],[cWdkz],[cKqtj],[cCzbx],[cPcrw],[cBxfz],[cZdsc],[cZtzx],[cYsxt],[cFpxs],[cZsyxh],[cQjsxt],[cQcys]
                            )VALUES(
                            @cCxmc,@fZdj,@cChangs,@cJibie,@cFdj,@cBsx,@cCkg,@cCsjg,@cZgcs,@cGfjs,@cScjs,@cSczd,@cScyh,@cGxbyh,@cZczb,@cChang,@cKuan,@cGao,@cZhouju,@cQnju,@cHnju,@cLdjx,@cZbzl,@cChesjg
                            ,@cCms,@cZws,@cYxrj,@cXlxrj,@cFdjxh,@fPail,@cJqxs,@cQgpl,@fQgs,@cQms,@cYsb,@cPqjg,@cGangj,@cChongc,@cZdml,@cZdgl,@cZhuans,@cZdlz,@cLzzs,@cTyjs,@cRlxs,@cRybh,@cGyfs,@cGgcl,@cGtcl,@cHbbz
                            ,@cJianc,@cDwgs,@cBsxlx,@cQdfs,@cQxglx,@cHxglx,@cZllx,@cCtjg,@cQzdq,@cHzdq,@cZczd,@cQnt,@cHnt,@cBetai
                            ,@cJszqls,@cFjsqls,@cQpcql,@cHpcql,@cQptb,@cHptb,@cQbql,@cTyjc,@cLty,@cHqdts,@cIso,@cLatch,@cFdjfd,@cCnzks
                            ,@cYkys,@cWysqd,@cAbs,@cCbc,@cBa,@cTrc,@cVsc,@cZdzc,@cDphj,@cKbxg,@cKqxg,@cKbzxb,@cZdtc,@cQjtc,@cYdwg,@cLhjn
                            ,@cDdxhm,@cZpfxp,@cSxtj,@cQhtj,@cDdtj,@cDgnfxp,@cFxphd,@cDsxh,@cBcfz,@cDcsp,@cDnxsp,@cHud,@cZpzy,@cYdzy,@cZygd
                            ,@cYbzc,@cJbzc,@cQpddtj,@cEpjdtj,@cEpzyyd,@cHptj,@cDdjy,@cQpzyjr,@cHpzyjr,@cZytf,@cZyam,@cHpztpf,@cHpblpf,@cSpzy
                            ,@cQzfs,@cHzfs,@cHphj,@cDdhbx,@cGps,@cDwfw,@cCsdp,@cNzyp,@cCzdh,@cCzds,@cHpyjp,@cIpod,@cMp3,@cDdcd,@cXndd,@cDuodcd
                            ,@cDddvd,@cDuodvd,@c23lb,@c45lb,@c67lb,@c8lb,@cXqdd,@cLed,@cRjxcd,@cZdtd,@cZxtd,@cQwd,@cGdkt,@cQxzz,@cCnfwd
                            ,@cQddcc,@cHddcc,@cFjs,@cFzwx,@cHsjtj,@cHsjjr,@cFxm,@cZdzd,@cHsjjy,@cHfdz,@cHpcz,@cHzj,@cHys,@cGyys,@cSdkt
                            ,@cZdkt,@cDlkt,@cHzcfk,@cWdkz,@cKqtj,@cCzbx,@cPcrw,@cBxfz,@cZdsc,@cZtzx,@cYsxt,@cFpxs,@cZsyxh,@cQjsxt,@cQcys
                            )
                        END
                        ";
                        SqlParameter[] p = 
                        { 
                            new SqlParameter("@fPp",car.fPp),
                            new SqlParameter("@fXhid",car.fXhid),
                            new SqlParameter("@fYh",car.fYh),
                            new SqlParameter("@fZx",car.fZx),
                            new SqlParameter("@fTj",car.fTj),
                            new SqlParameter("@fPx",car.fPx),
                            new SqlParameter("@cYhnr",car.cYhnr),
                            new SqlParameter("@cJzsj",car.cJzsj),
                            new SqlParameter("@xPic",car.xPic),
                            new SqlParameter("@dPic",car.dPic),
                            new SqlParameter("@fSscs",car.fSscs),
                            new SqlParameter("@cKsjc",car.cKsjc),
                            new SqlParameter("@cXh",car.cXh),
                            new SqlParameter("@cQcys",car.cQcys),
                            new SqlParameter("@cXntd",car.cXntd),
                            new SqlParameter("@cCxmc",car.cCxmc),
                            new SqlParameter("@cJgqj",car.cJgqj),
                            new SqlParameter("@fZdj",car.fZdj),
                            new SqlParameter("@cChangs",car.cChangs),
                            new SqlParameter("@cJibie",car.cJibie),
                            new SqlParameter("@cFdj",car.cFdj),
                            new SqlParameter("@cBsx",car.cBsx),
                            new SqlParameter("@cCkg",car.cCkg),
                            new SqlParameter("@cCsjg",car.cCsjg),
                            new SqlParameter("@cZgcs",car.cZgcs),
                            new SqlParameter("@cGfjs",car.cGfjs),
                            new SqlParameter("@cScjs",car.cScjs),
                            new SqlParameter("@cSczd",car.cSczd),
                            new SqlParameter("@cScyh",car.cScyh),
                            new SqlParameter("@cGxbyh",car.cGxbyh),
                            new SqlParameter("@cZczb",car.cZczb),
                            new SqlParameter("@cChang",car.cChang),
                            new SqlParameter("@cKuan",car.cKuan),
                            new SqlParameter("@cGao",car.cGao),
                            new SqlParameter("@cZhouju",car.cZhouju),
                            new SqlParameter("@cQnju",car.cQnju),
                            new SqlParameter("@cHnju",car.cHnju),
                            new SqlParameter("@cLdjx",car.cLdjx),
                            new SqlParameter("@cZbzl",car.cZbzl),
                            new SqlParameter("@cChesjg",car.cChesjg),
                            new SqlParameter("@cCms",car.cCms),
                            new SqlParameter("@cZws",car.cZws),
                            new SqlParameter("@cYxrj",car.cYxrj),
                            new SqlParameter("@cXlxrj",car.cXlxrj),
                            new SqlParameter("@cFdjxh",car.cFdjxh),
                            new SqlParameter("@fPail",car.fPail),
                            new SqlParameter("@cJqxs",car.cJqxs),
                            new SqlParameter("@cQgpl",car.cQgpl),
                            new SqlParameter("@fQgs",car.fQgs),
                            new SqlParameter("@cQms",car.cQms),
                            new SqlParameter("@cYsb",car.cYsb),
                            new SqlParameter("@cPqjg",car.cPqjg),
                            new SqlParameter("@cGangj",car.cGangj),
                            new SqlParameter("@cChongc",car.cChongc),
                            new SqlParameter("@cZdml",car.cZdml),
                            new SqlParameter("@cZdgl",car.cZdgl),
                            new SqlParameter("@cZhuans",car.cZhuans),
                            new SqlParameter("@cZdlz",car.cZdlz),
                            new SqlParameter("@cLzzs",car.cLzzs),
                            new SqlParameter("@cTyjs",car.cTyjs),
                            new SqlParameter("@cRlxs",car.cRlxs),
                            new SqlParameter("@cRybh",car.cRybh),
                            new SqlParameter("@cGyfs",car.cGyfs),
                            new SqlParameter("@cGgcl",car.cGgcl),
                            new SqlParameter("@cGtcl",car.cGtcl),
                            new SqlParameter("@cHbbz",car.cHbbz),
                            new SqlParameter("@cJianc",car.cJianc),
                            new SqlParameter("@cDwgs",car.cDwgs),
                            new SqlParameter("@cBsxlx",car.cBsxlx),
                            new SqlParameter("@cQdfs",car.cQdfs),
                            new SqlParameter("@cQxglx",car.cQxglx),
                            new SqlParameter("@cHxglx",car.cHxglx),
                            new SqlParameter("@cZllx",car.cZllx),
                            new SqlParameter("@cCtjg",car.cCtjg),
                            new SqlParameter("@cQzdq",car.cQzdq),
                            new SqlParameter("@cHzdq",car.cHzdq),
                            new SqlParameter("@cZczd",car.cZczd),
                            new SqlParameter("@cQnt",car.cQnt),
                            new SqlParameter("@cHnt",car.cHnt),
                            new SqlParameter("@cBetai",car.cBetai),
                            new SqlParameter("@cJszqls",car.cJszqls),
                            new SqlParameter("@cFjsqls",car.cFjsqls),
                            new SqlParameter("@cQpcql",car.cQpcql),
                            new SqlParameter("@cHpcql",car.cHpcql),
                            new SqlParameter("@cQptb",car.cQptb),
                            new SqlParameter("@cHptb",car.cHptb),
                            new SqlParameter("@cQbql",car.cQbql),
                            new SqlParameter("@cTyjc",car.cTyjc),
                            new SqlParameter("@cLty",car.cLty),
                            new SqlParameter("@cHqdts",car.cHqdts),
                            new SqlParameter("@cIso",car.cIso),
                            new SqlParameter("@cLatch",car.cLatch),
                            new SqlParameter("@cFdjfd",car.cFdjfd),
                            new SqlParameter("@cCnzks",car.cCnzks),
                            new SqlParameter("@cYkys",car.cYkys),
                            new SqlParameter("@cWysqd",car.cWysqd),
                            new SqlParameter("@cAbs",car.cAbs),
                            new SqlParameter("@cCbc",car.cCbc),
                            new SqlParameter("@cBa",car.cBa),
                            new SqlParameter("@cTrc",car.cTrc),
                            new SqlParameter("@cVsc",car.cVsc),
                            new SqlParameter("@cZdzc",car.cZdzc),
                            new SqlParameter("@cDphj",car.cDphj),
                            new SqlParameter("@cKbxg",car.cKbxg),
                            new SqlParameter("@cKqxg",car.cKqxg),
                            new SqlParameter("@cKbzxb",car.cKbzxb),
                            new SqlParameter("@cZdtc",car.cZdtc),
                            new SqlParameter("@cQjtc",car.cQjtc),
                            new SqlParameter("@cYdwg",car.cYdwg),
                            new SqlParameter("@cLhjn",car.cLhjn),
                            new SqlParameter("@cDdxhm",car.cDdxhm),
                            new SqlParameter("@cZpfxp",car.cZpfxp),
                            new SqlParameter("@cSxtj",car.cSxtj),
                            new SqlParameter("@cQhtj",car.cQhtj),
                            new SqlParameter("@cDdtj",car.cDdtj),
                            new SqlParameter("@cDgnfxp",car.cDgnfxp),
                            new SqlParameter("@cFxphd",car.cFxphd),
                            new SqlParameter("@cDsxh",car.cDsxh),
                            new SqlParameter("@cBcfz",car.cBcfz),
                            new SqlParameter("@cDcsp",car.cDcsp),
                            new SqlParameter("@cDnxsp",car.cDnxsp),
                            new SqlParameter("@cHud",car.cHud),
                            new SqlParameter("@cZpzy",car.cZpzy),
                            new SqlParameter("@cYdzy",car.cYdzy),
                            new SqlParameter("@cZygd",car.cZygd),
                            new SqlParameter("@cYbzc",car.cYbzc),
                            new SqlParameter("@cJbzc",car.cJbzc),
                            new SqlParameter("@cQpddtj",car.cQpddtj),
                            new SqlParameter("@cEpjdtj",car.cEpjdtj),
                            new SqlParameter("@cEpzyyd",car.cEpzyyd),
                            new SqlParameter("@cHptj",car.cHptj),
                            new SqlParameter("@cDdjy",car.cDdjy),
                            new SqlParameter("@cQpzyjr",car.cQpzyjr),
                            new SqlParameter("@cHpzyjr",car.cHpzyjr),
                            new SqlParameter("@cZytf",car.cZytf),
                            new SqlParameter("@cZyam",car.cZyam),
                            new SqlParameter("@cHpztpf",car.cHpztpf),
                            new SqlParameter("@cHpblpf",car.cHpblpf),
                            new SqlParameter("@cSpzy",car.cSpzy),
                            new SqlParameter("@cQzfs",car.cQzfs),
                            new SqlParameter("@cHzfs",car.cHzfs),
                            new SqlParameter("@cHphj",car.cHphj),
                            new SqlParameter("@cDdhbx",car.cDdhbx),
                            new SqlParameter("@cGps",car.cGps),
                            new SqlParameter("@cDwfw",car.cDwfw),
                            new SqlParameter("@cCsdp",car.cCsdp),
                            new SqlParameter("@cRjjh",car.cRjjh),
                            new SqlParameter("@cNzyp",car.cNzyp),
                            new SqlParameter("@cCzdh",car.cCzdh),
                            new SqlParameter("@cCzds",car.cCzds),
                            new SqlParameter("@cHpyjp",car.cHpyjp),
                            new SqlParameter("@cIpod",car.cIpod),
                            new SqlParameter("@cMp3",car.cMp3),
                            new SqlParameter("@cDdcd",car.cDdcd),
                            new SqlParameter("@cXndd",car.cXndd),
                            new SqlParameter("@cDuodcd",car.cDuodcd),
                            new SqlParameter("@cDddvd",car.cDddvd),
                            new SqlParameter("@cDuodvd",car.cDuodvd),
                            new SqlParameter("@c23lb",car.c23lb),
                            new SqlParameter("@c45lb",car.c45lb),
                            new SqlParameter("@c67lb",car.c67lb),
                            new SqlParameter("@c8lb",car.c8lb),
                            new SqlParameter("@cXqdd",car.cXqdd),
                            new SqlParameter("@cLed",car.cLed),
                            new SqlParameter("@cRjxcd",car.cRjxcd),
                            new SqlParameter("@cZdtd",car.cZdtd),
                            new SqlParameter("@cZxtd",car.cZxtd),
                            new SqlParameter("@cQwd",car.cQwd),
                            new SqlParameter("@cGdkt",car.cGdkt),
                            new SqlParameter("@cQxzz",car.cQxzz),
                            new SqlParameter("@cCnfwd",car.cCnfwd),
                            new SqlParameter("@cQddcc",car.cQddcc),
                            new SqlParameter("@cHddcc",car.cHddcc),
                            new SqlParameter("@cFjs",car.cFjs),
                            new SqlParameter("@cFzwx",car.cFzwx),
                            new SqlParameter("@cHsjtj",car.cHsjtj),
                            new SqlParameter("@cHsjjr",car.cHsjjr),
                            new SqlParameter("@cFxm",car.cFxm),
                            new SqlParameter("@cZdzd",car.cZdzd),
                            new SqlParameter("@cHsjjy",car.cHsjjy),
                            new SqlParameter("@cHfdz",car.cHfdz),
                            new SqlParameter("@cHpcz",car.cHpcz),
                            new SqlParameter("@cHzj",car.cHzj),
                            new SqlParameter("@cHys",car.cHys),
                            new SqlParameter("@cGyys",car.cGyys),
                            new SqlParameter("@cSdkt",car.cSdkt),
                            new SqlParameter("@cZdkt",car.cZdkt),
                            new SqlParameter("@cDlkt",car.cDlkt),
                            new SqlParameter("@cHzcfk",car.cHzcfk),
                            new SqlParameter("@cWdkz",car.cWdkz),
                            new SqlParameter("@cKqtj",car.cKqtj),
                            new SqlParameter("@cCzbx",car.cCzbx),
                            new SqlParameter("@cPcrw",car.cPcrw),
                            new SqlParameter("@cBxfz",car.cBxfz),
                            new SqlParameter("@cZdsc",car.cZdsc),
                            new SqlParameter("@cZtzx",car.cZtzx),
                            new SqlParameter("@cYsxt",car.cYsxt),
                            new SqlParameter("@cFpxs",car.cFpxs),
                            new SqlParameter("@cZsyxh",car.cZsyxh),
                            new SqlParameter("@cQjsxt",car.cQjsxt),
                            new SqlParameter("@fHit",car.fHit),
                            new SqlParameter("@fDel",car.fDel)
                        };


                        chtExe(sql, p);

                        #endregion

                        insertcount++;
                        lblInsertCount.Text = "已录入数：" + insertcount ;
                        lblLast.Text = " last:" + k;
                    }
                    else
                    {
                        int tcount = 0;
                        tcount++;
                    }

                    docount++;
                    pbStatus.Value = docount * 100 / allnum;

                }
                catch
                {
                    docount++;
                    pbStatus.Value = docount * 100 / allnum;
                    continue;
                }
            }
            button1.Enabled = true;
            lblStatus.Text = "抓取完成";
            pbStatus.Value = 0;
            MessageBox.Show("抓取完成!");
        }

        //获取远程html代码
        string gethttp(string Url, int timeout, int times = 0, int tryTimes = 1, bool useutf8 = false, int delay = 0)
        {
            times++;
            string s = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; zh-CN; rv:1.8.1.8) Gecko/20071008 Firefox/2.0.0.8";
                request.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
                request.AllowAutoRedirect = true;
                request.Headers.Add(HttpRequestHeader.AcceptCharset, "gb2312,utf-8;q=0.7,*;q=0.7");
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                request.Timeout = timeout;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    s = Decompress(stream, response.ContentEncoding, useutf8 ? "utf-8" : "GB2312");
                }
                stream.Close();
                response.Close();
                Thread.Sleep(500);
            }
            catch
            {
                s = "";
            }
            finally
            {
                //判断是否需要重试
                if (string.IsNullOrEmpty(s) && times < tryTimes)
                {
                    //每次重试间隔1秒
                    System.Threading.Thread.Sleep(1000);
                    s = gethttp(Url, timeout, times, tryTimes, useutf8, delay);
                }
            }
            //每次采集间隔delay
            if (delay > 0)
                Thread.Sleep(delay);
            return s;
        }

        /// <summary>
        /// gzip解压
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <param name="characterSet"></param>
        /// <returns></returns>
        public static string Decompress(Stream stream, string encoding, string characterSet)
        {
            Stream s = null;
            if (string.Compare(encoding, "gzip", true) == 0)
            {
                s = new GZipStream(stream, CompressionMode.Decompress, true);
            }
            else if (string.Compare(encoding, "deflate", true) == 0)
            {
                s = new DeflateStream(stream, CompressionMode.Decompress);
            }
            else
            {
                s = stream;
            }
            StreamReader sr = new StreamReader(s, Encoding.GetEncoding(characterSet), true);
            string result = sr.ReadToEnd();
            sr.Close();
            s.Close();
            return result;
        }

        /// <summary>
        /// 写入数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        int chtExe(string sql, SqlParameter[] p)
        {
            SqlCommand com1 = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(datastr1))
            {
                OpenConn(conn, com1, sql, p);
                int ct = com1.ExecuteNonQuery();
                com1.Dispose();
                return ct;
            }
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="com"></param>
        /// <param name="sql"></param>
        /// <param name="p"></param>
        void OpenConn(SqlConnection conn, SqlCommand com, string sql, SqlParameter[] p)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            com.CommandText = sql;
            com.CommandTimeout = 90;
            com.Connection = conn;
            com.Parameters.AddRange(p);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                t.Abort();
            }
            catch { }
            Application.ExitThread();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("330301", "市辖区");
            dic.Add("330302", "鹿城区");
            dic.Add("330303", "龙湾区");
            dic.Add("330304", "瓯海区");
            dic.Add("330322", "洞头县");
            dic.Add("330324", "永嘉县");
            dic.Add("330326", "平阳县");
            dic.Add("330327", "苍南县");
            dic.Add("330328", "文成县");
            dic.Add("330329", "泰顺县");
            dic.Add("330381", "瑞安市");
            dic.Add("330382", "乐清市");

            foreach (string key in dic.Keys)
            {
                string databasestr = string.Format("Server={0};UID={1};PWD={2};database={3};", textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text);
                string sql = string.Format("SELECT [Name] AS 姓名,CASE WHEN LEN([CtfId]) = 18 THEN (115 - CAST(SUBSTRING([CtfId],9,2) AS INT)) WHEN LEN([CtfId]) = 15 THEN (115 - CAST(SUBSTRING([CtfId],7,2) AS INT)) ELSE 0 END AS 年龄,[Mobile] AS 手机,[Email] AS 电子邮箱,[Address] AS 地址,[CtfId] AS 身份证,[Birthday] AS 生日,[Tel] AS 电话 FROM dbo.cdsgus_wz WHERE CHARINDEX('{0}',[CtfId]) = 1",key);
                string fileName = AppDomain.CurrentDomain.BaseDirectory + "shifen_wz_{0}.xlsx";
                //int pagesize = 8000;
                DataTable t = SqlHelper.ExecuteDataset(databasestr, CommandType.Text, sql).Tables[0];
                //int pagecount = t.Rows.Count / 8000 + (t.Rows.Count % 8000 > 0 ? 1 : 0);

                //DataRow[] rows = t.Select("1=1");
                //for (int i = 0; i < pagecount; i++)
                //{
                //    DataTable newt = t.Clone();
                //    for (int j = i * pagesize; j < (i < (pagecount - 1) ? (i + 1) * pagesize : rows.Length); j++)
                //    {
                //        newt.ImportRow((DataRow)rows[j]);
                //    }
                //    DataTabletoExcel(newt, string.Format(fileName, i + 1));
                //}

                DataTabletoExcel(t, string.Format(fileName, dic[key]));
            }
        }

        public void DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName)
        {
            if (tmpDataTable == null)
                return;
            int rowNum = tmpDataTable.Rows.Count;
            int columnNum = tmpDataTable.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;
            //要添加引用using Microsoft.Office.Interop.Excel
            Microsoft.Office.Interop.Excel.Application xIAPP = new Microsoft.Office.Interop.Excel.Application();
            xIAPP.DefaultFilePath = "";
            xIAPP.SheetsInNewWorkbook = 1;
            Microsoft.Office.Interop.Excel.Workbook xIBOOK = xIAPP.Workbooks.Add(true);
            foreach (DataColumn dc in tmpDataTable.Columns)
            {
                columnIndex++;
                xIAPP.Cells[rowIndex, columnIndex] = dc.ColumnName;
            }
            for (int i = 0; i < rowNum; i++)
            {
                rowIndex++;
                columnIndex = 0;
                for (int j = 0; j < columnNum; j++)
                {
                    columnIndex++;
                    try
                    {
                        xIAPP.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();
                    }
                    catch { }
                }
            }
            xIBOOK.SaveCopyAs(strFileName);
        }
    }
}
