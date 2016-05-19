using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using System.Net;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Text;
using System.Data.SqlClient;
using Hx.Tools;
using Hx.Car.Entity;
using Hx.Car;
using System.Text.RegularExpressions;
using System.Data;

namespace Hx.BackAdmin.car
{
    public partial class carcollect : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.总经理) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCollectInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtUrl.Text))
                {
                    lblMsg.Text = "请输入采集地址";
                    return;
                }


                string url = txtUrl.Text;
                string strDetail = string.Empty;
                string strContent = gethttp(url, 3000, 0, 2, false, 1000);
                string k = string.Empty;
                Regex regk = new Regex(@"http://car.autohome.com.cn/config/spec/(\d+).html[\s\S]*");
                if (!regk.IsMatch(url))
                {
                    lblMsg.Text = "获取车辆标示失败";
                    return;
                }
                k = regk.Match(url).Groups[1].Value;

                //基本信息
                List<KeyValuePair<string, string>> listInfo = new List<KeyValuePair<string, string>>();
                //车身颜色
                List<KeyValuePair<string, string>> listColor = new List<KeyValuePair<string, string>>();
                //内饰颜色
                List<KeyValuePair<string, string>> listInnerColor = new List<KeyValuePair<string, string>>();

                //获取数值
                Regex r1 = new Regex(string.Format(@"""name"":""((?:(?!name)[\s\S])*?)"",""valueitems"":\[\{{""specid"":{0},""value"":""([\s\S]*?)""", k), RegexOptions.None);
                foreach (Match m in r1.Matches(strContent))
                {
                    listInfo.Add(new KeyValuePair<string, string>(m.Groups[1].Value, m.Groups[2].Value));
                    string Str = m.Groups[1].Value;
                    strContent = strContent.Replace(m.Groups[0].Value, Str);
                }
                if (listInfo.Count == 0)
                {
                    lblMsg.Text = "获取车辆信息失败";
                    return;
                }

                //获取车身颜色
                string strColor = string.Empty;
                Regex rcolorstr = new Regex(@"var color[\s\S]*?;");
                strColor = rcolorstr.Match(strContent).Groups[0].Value;
                Regex r2 = new Regex(string.Format(string.Format(@"""specid"":{0},""coloritems"":\[([\s\S]*?)\]", k), RegexOptions.None));
                Regex r3 = new Regex(@"""name"":""([\s\S]*?)"",""value"":""([\s\S]*?)""", RegexOptions.None);
                if (r2.IsMatch(strColor))
                {
                    strColor = r2.Match(strColor).Groups[1].Value;
                    foreach (Match m in r3.Matches(strColor))
                    {
                        listColor.Add(new KeyValuePair<string, string>(m.Groups[1].Value, m.Groups[2].Value));
                    }
                }

                //获取内饰颜色
                string strInnerColor = string.Empty;
                Regex rinnercolorstr = new Regex(@"var innerColor[\s\S]*?;");
                strInnerColor = rinnercolorstr.Match(strContent).Groups[0].Value;
                Regex r4 = new Regex(string.Format(string.Format(@"""specid"":{0},""coloritems"":\[(?:(?!specid)[\s\S])*]", k), RegexOptions.None));
                Regex r5 = new Regex(@"""name"":""([\s\S]*?)"",""values"":\[([\s\S]*?)\]", RegexOptions.None);
                if (r4.IsMatch(strInnerColor))
                {
                    strInnerColor = r4.Match(strInnerColor).Groups[0].Value;
                    foreach (Match m in r5.Matches(strInnerColor))
                    {
                        listInnerColor.Add(new KeyValuePair<string, string>(m.Groups[1].Value, m.Groups[2].Value));
                    }
                }

                CarInfo car = new CarInfo();
                car.cCxmc = listInfo[listInfo.FindIndex(l => l.Key == "车型名称")].Value;
                car.fZdj = DataConvert.SafeDecimal(listInfo[listInfo.FindIndex(l => l.Key == "厂商指导价(元)")].Value.Replace("万", string.Empty));
                car.cChangs = listInfo[listInfo.FindIndex(l => l.Key == "厂商")].Value;
                if (listColor.Count > 0)
                    car.cQcys = string.Join("|", listColor.Select(l => l.Key + "," + l.Value));
                if (listInnerColor.Count > 0)
                    car.cNsys = string.Join("|", listInnerColor.Select(l => l.Key + "," + l.Value));



                #region 绑定数据

                txtcChangs.Text = car.cChangs;
                txtcCxmc.Text = car.cCxmc;
                txtfZdj.Text = car.fZdj.ToString();
                hdncQcys.Value = car.cQcys;
                hdnInnerColor.Value = car.cNsys;
                if (!string.IsNullOrEmpty(car.cQcys))
                {
                    DataTable t = new DataTable();
                    t.Columns.Add("Name");
                    t.Columns.Add("Color");

                    foreach (string colorinfo in car.cQcys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] colors = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (colors.Length == 2)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = colors[0];
                            row["Color"] = colors[1];
                            t.Rows.Add(row);
                        }
                    }

                    rptcQcys.DataSource = t;
                    rptcQcys.DataBind();
                }
                else
                {
                    rptcQcys.DataSource = null;
                    rptcQcys.DataBind();
                }
                if (!string.IsNullOrEmpty(car.cNsys))
                {
                    DataTable t = new DataTable();
                    t.Columns.Add("Name");
                    t.Columns.Add("Color");

                    foreach (string colorinfo in car.cNsys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] color = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (color.Length == 2)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = color[0];
                            row["Color"] = color[1].Replace("\"", string.Empty);
                            t.Rows.Add(row);
                        }
                        else if (color.Length == 3)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = color[0];
                            row["Color"] = color[1].Replace("\"", string.Empty) + "," + color[2].Replace("\"", string.Empty);
                            t.Rows.Add(row);
                        }
                    }

                    rptcNsys.DataSource = t;
                    rptcNsys.DataBind();
                }
                else
                {
                    rptcNsys.DataSource = null;
                    rptcNsys.DataBind();
                }

                #endregion


                #region 已删除

                //if (!string.IsNullOrEmpty(strDetail))
                //{
                //    string[] detail = strDetail.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                //    #region 获取车辆信息

                //    CarInfo car = new CarInfo();
                //    car.cCxmc = detail[0];
                //    car.fZdj = DataConvert.SafeDecimal(detail[1].Replace("万", string.Empty));
                //    car.cChangs = detail[2];
                //    car.cJibie = detail[3];
                //    car.cFdj = detail[4];
                //    car.cBsx = detail[5];
                //    car.cCkg = detail[6];
                //    car.cCsjg = detail[7];
                //    car.cZgcs = detail[8];
                //    car.cGfjs = detail[9];
                //    car.cScjs = detail[10];
                //    car.cSczd = detail[11];
                //    car.cScyh = detail[12];
                //    car.cGxbyh = detail[13];
                //    car.cZczb = detail[14];
                //    car.cChang = detail[15];
                //    car.cKuan = detail[16];
                //    car.cGao = detail[17];
                //    car.cZhouju = detail[18];
                //    car.cQnju = detail[19];
                //    car.cHnju = detail[20];
                //    car.cLdjx = detail[21];
                //    car.cZbzl = detail[22];
                //    car.cChesjg = detail[23];
                //    car.cCms = detail[24];
                //    car.cZws = detail[25];
                //    car.cYxrj = detail[26];
                //    car.cXlxrj = detail[27];
                //    car.cFdjxh = detail[28];
                //    car.fPail = detail[29];
                //    car.cJqxs = detail[31];
                //    car.cQgpl = detail[32];
                //    car.fQgs = DataConvert.SafeInt(detail[33]);
                //    car.cQms = detail[34];
                //    car.cYsb = detail[35];
                //    car.cPqjg = detail[36];
                //    car.cGangj = detail[37];
                //    car.cChongc = detail[38];
                //    car.cZdml = detail[39];
                //    car.cZdgl = detail[40];
                //    car.cZhuans = detail[41];
                //    car.cZdlz = detail[42];
                //    car.cLzzs = detail[43];
                //    car.cTyjs = detail[44];
                //    car.cRlxs = detail[45];
                //    car.cRybh = detail[46];
                //    car.cGyfs = detail[47];
                //    car.cGgcl = detail[48];
                //    car.cGtcl = detail[49];
                //    car.cHbbz = detail[50];
                //    car.cJianc = detail[51];
                //    car.cDwgs = detail[52];
                //    car.cBsxlx = detail[53];
                //    car.cQdfs = detail[54];
                //    car.cQxglx = detail[55];
                //    car.cHxglx = detail[56];
                //    car.cZllx = detail[57];
                //    car.cCtjg = detail[58];
                //    car.cQzdq = detail[59];
                //    car.cHzdq = detail[60];
                //    car.cZczd = detail[61];
                //    car.cQnt = detail[62];
                //    car.cHnt = detail[63];
                //    car.cBetai = detail[64];
                //    car.cJszqls = "-";
                //    car.cFjsqls = "-";
                //    if (detail[65].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                //    {
                //        car.cJszqls = detail[65].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("主", string.Empty).Trim();
                //        car.cFjsqls = detail[65].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("副", string.Empty).Trim();
                //    }
                //    car.cQpcql = "-";
                //    car.cHpcql = "-";
                //    if (detail[66].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                //    {
                //        car.cQpcql = detail[66].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                //        car.cHpcql = detail[66].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                //    }
                //    car.cQptb = "-";
                //    car.cHptb = "-";
                //    if (detail[67].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                //    {
                //        car.cQptb = detail[67].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                //        car.cHptb = detail[67].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                //    }
                //    car.cQbql = detail[68];
                //    car.cTyjc = detail[69];
                //    car.cLty = detail[70];
                //    car.cHqdts = detail[71];
                //    car.cIso = detail[72].IndexOf("iso", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cLatch = detail[72].IndexOf("latch", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cFdjfd = detail[73];
                //    car.cCnzks = detail[74];
                //    car.cYkys = detail[75];
                //    car.cWysqd = detail[76];
                //    car.cAbs = detail[78];
                //    car.cCbc = detail[79];
                //    car.cBa = detail[80];
                //    car.cTrc = detail[81];
                //    car.cVsc = detail[82];
                //    car.cZdzc = detail[83];
                //    car.cDphj = detail[84];
                //    car.cKbxg = detail[85];
                //    car.cKqxg = detail[86];
                //    car.cKbzxb = detail[87];
                //    car.cZdtc = detail[91];
                //    car.cQjtc = detail[92];
                //    car.cYdwg = detail[93];
                //    car.cLhjn = detail[94];
                //    car.cDdxhm = detail[95];
                //    car.cZpfxp = detail[96];
                //    car.cSxtj = detail[97].IndexOf("上下", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cQhtj = detail[97].IndexOf("前后", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cDdtj = detail[98];
                //    car.cDgnfxp = detail[99];
                //    car.cFxphd = detail[100];
                //    car.cDsxh = detail[102];
                //    car.cBcfz = detail[103];
                //    car.cDcsp = detail[104];
                //    car.cDnxsp = detail[105];
                //    car.cHud = detail[106];
                //    car.cZpzy = detail[107];
                //    car.cYdzy = detail[108];
                //    car.cZygd = detail[109];
                //    car.cYbzc = detail[110];
                //    car.cJbzc = detail[111];
                //    car.cQpddtj = detail[112];
                //    car.cEpjdtj = detail[113];
                //    car.cEpzyyd = detail[114];
                //    car.cHptj = detail[115];
                //    car.cDdjy = detail[116];
                //    car.cQpzyjr = "-";
                //    car.cHpzyjr = "-";
                //    if (detail[117].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                //    {
                //        car.cQpzyjr = detail[117].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                //        car.cHpzyjr = detail[117].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                //    }
                //    car.cZytf = detail[118];
                //    car.cZyam = detail[119];
                //    car.cHpztpf = detail[120].Trim() == "整体放倒" ? "●" : "-";
                //    car.cHpblpf = detail[120].Trim() == "比例放倒" ? "●" : "-";
                //    car.cSpzy = detail[121];
                //    car.cQzfs = "-";
                //    car.cHzfs = "-";
                //    if (detail[122].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                //    {
                //        car.cQzfs = detail[122].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                //        car.cHzfs = detail[122].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                //    }
                //    car.cHphj = detail[123];
                //    car.cDdhbx = detail[124];
                //    car.cGps = detail[125];
                //    car.cDwfw = detail[126];
                //    car.cCsdp = detail[127];
                //    car.cNzyp = detail[128];
                //    car.cCzdh = detail[129];
                //    car.cCzds = detail[130];
                //    car.cHpyjp = detail[131];
                //    car.cIpod = detail[132];
                //    car.cMp3 = detail[133];
                //    car.cDdcd = detail[134].IndexOf("单碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cDdcd = detail[134].IndexOf("选装单碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDdcd;
                //    car.cXndd = detail[134].IndexOf("虚拟多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cXndd = detail[134].IndexOf("选装虚拟多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cXndd;
                //    car.cDuodcd = (detail[134].IndexOf("多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 && detail[134].IndexOf("虚拟多碟CD", StringComparison.OrdinalIgnoreCase) < 0 && detail[134].IndexOf("选装虚拟多碟CD", StringComparison.OrdinalIgnoreCase) < 0) ? "●" : "-";
                //    car.cDuodcd = detail[134].IndexOf("选装多碟CD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDuodcd;
                //    car.cDddvd = detail[134].IndexOf("单碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cDddvd = detail[134].IndexOf("选装单碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDddvd;
                //    car.cDuodvd = detail[134].IndexOf("多碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cDuodvd = detail[134].IndexOf("选装多碟DVD", StringComparison.OrdinalIgnoreCase) >= 0 ? "○" : car.cDuodvd;
                //    car.c23lb = detail[135].IndexOf("2-3喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.c45lb = detail[135].IndexOf("4-5喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.c67lb = detail[135].IndexOf("6-7喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.c8lb = detail[135].IndexOf("≥8喇叭", StringComparison.OrdinalIgnoreCase) >= 0 ? "●" : "-";
                //    car.cXqdd = detail[136];
                //    car.cLed = detail[137];
                //    car.cRjxcd = detail[138];
                //    car.cZdtd = detail[139];
                //    car.cZxtd = detail[140];
                //    car.cQwd = detail[141];
                //    car.cGdkt = detail[142];
                //    car.cQxzz = detail[143];
                //    car.cCnfwd = detail[144];
                //    car.cQddcc = "-";
                //    car.cHddcc = "-";
                //    if (detail[145].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                //    {
                //        car.cQddcc = detail[145].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("前", string.Empty).Trim();
                //        car.cHddcc = detail[145].Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("后", string.Empty).Trim();
                //    }
                //    car.cFjs = detail[146];
                //    car.cFzwx = detail[147];
                //    car.cHsjtj = detail[148];
                //    car.cHsjjr = detail[149];
                //    car.cFxm = detail[150];
                //    car.cZdzd = detail[151];
                //    car.cHsjjy = detail[152];
                //    car.cHfdz = detail[153];
                //    car.cHpcz = detail[154];
                //    car.cHzj = detail[156];
                //    car.cHys = detail[157];
                //    car.cGyys = detail[158];
                //    car.cSdkt = detail[159] == "手动●" ? "●" : "-";
                //    car.cZdkt = detail[159] == "自动●" ? "●" : "-";
                //    car.cDlkt = detail[160];
                //    car.cHzcfk = detail[161];
                //    car.cWdkz = detail[162];
                //    car.cKqtj = detail[163];
                //    car.cCzbx = detail[164];
                //    car.cPcrw = detail[165];
                //    car.cBxfz = detail[167];
                //    car.cZdsc = detail[169];
                //    car.cZtzx = detail[170];
                //    car.cYsxt = detail[171];
                //    car.cFpxs = detail[172];
                //    car.cZsyxh = detail[173];
                //    car.cQjsxt = detail[174];

                //    car.cQcys = color;


                //    #endregion

                //    #region 绑定数据

                //    txtcChangs.Text = car.cChangs;
                //    txtcCxmc.Text = car.cCxmc;
                //    txtfZdj.Text = car.fZdj.ToString();
                //    txtcJibie.Text = car.cJibie;
                //    txtcFdj.Text = car.cFdj;
                //    txtcBsx.Text = car.cBsx;
                //    txtcCkg.Text = car.cCkg;
                //    txtcCsjg.Text = car.cCsjg;
                //    txtcZgcs.Text = car.cZgcs;
                //    txtcGfjs.Text = car.cGfjs;
                //    txtcScjs.Text = car.cScjs;
                //    txtcSczd.Text = car.cSczd;
                //    txtcScyh.Text = car.cScyh;
                //    txtcGxbyh.Text = car.cGxbyh;
                //    txtcZczb.Text = car.cZczb;
                //    txtcChang.Text = car.cChang;
                //    txtcKuan.Text = car.cKuan;
                //    txtcGao.Text = car.cGao;
                //    txtcZhouju.Text = car.cZhouju;
                //    txtcQnju.Text = car.cQnju;
                //    txtcHnju.Text = car.cHnju;
                //    txtcLdjx.Text = car.cLdjx;
                //    txtcZbzl.Text = car.cZbzl;
                //    txtcChesjg.Text = car.cChesjg;
                //    txtcCms.Text = car.cCms;
                //    txtcZws.Text = car.cZws;
                //    txtcYxrj.Text = car.cYxrj;
                //    txtcXlxrj.Text = car.cXlxrj;
                //    txtcFdjxh.Text = car.cFdjxh;
                //    txtfPail.Text = car.fPail;
                //    txtcJqxs.Text = car.cJqxs;
                //    txtcQgpl.Text = car.cQgpl;
                //    txtfQgs.Text = car.fQgs.ToString();
                //    txtcQms.Text = car.cQms;
                //    txtcYsb.Text = car.cYsb;
                //    txtcPqjg.Text = car.cPqjg;
                //    txtcGangj.Text = car.cGangj;
                //    txtcChongc.Text = car.cChongc;
                //    txtcZdml.Text = car.cZdml;
                //    txtcZdgl.Text = car.cZdgl;
                //    txtcZhuans.Text = car.cZhuans;
                //    txtcZdlz.Text = car.cZdlz;
                //    txtcLzzs.Text = car.cLzzs;
                //    txtcTyjs.Text = car.cTyjs;
                //    txtcRlxs.Text = car.cRlxs;
                //    txtcRybh.Text = car.cRybh;
                //    txtcGyfs.Text = car.cGyfs;
                //    txtcGgcl.Text = car.cGgcl;
                //    txtcGtcl.Text = car.cGtcl;
                //    txtcHbbz.Text = car.cHbbz;
                //    txtcJianc.Text = car.cJianc;
                //    txtcDwgs.Text = car.cDwgs;
                //    txtcBsxlx.Text = car.cBsxlx;
                //    txtcQdfs.Text = car.cQdfs;
                //    txtcQxglx.Text = car.cQxglx;
                //    txtcHxglx.Text = car.cHxglx;
                //    txtcZllx.Text = car.cZllx;
                //    txtcCtjg.Text = car.cCtjg;
                //    txtcQzdq.Text = car.cQzdq;
                //    txtcHzdq.Text = car.cHzdq;
                //    txtcZczd.Text = car.cZczd;
                //    txtcQnt.Text = car.cQnt;
                //    txtcHnt.Text = car.cHnt;
                //    txtcBetai.Text = car.cBetai;
                //    txtcJszqls.Text = car.cJszqls;
                //    txtcFjsqls.Text = car.cFjsqls;
                //    txtcQpcql.Text = car.cQpcql;
                //    txtcHpcql.Text = car.cHpcql;
                //    txtcQptb.Text = car.cQptb;
                //    txtcHptb.Text = car.cHptb;
                //    txtcQbql.Text = car.cQbql;
                //    txtcTyjc.Text = car.cTyjc;
                //    txtcLty.Text = car.cLty;
                //    txtcHqdts.Text = car.cHqdts;
                //    txtcIso.Text = car.cIso;
                //    txtcLatch.Text = car.cLatch;
                //    txtcFdjfd.Text = car.cFdjfd;
                //    txtcCnzks.Text = car.cCnzks;
                //    txtcYkys.Text = car.cYkys;
                //    txtcWysqd.Text = car.cWysqd;
                //    txtcAbs.Text = car.cAbs;
                //    txtcCbc.Text = car.cCbc;
                //    txtcBa.Text = car.cBa;
                //    txtcTrc.Text = car.cTrc;
                //    txtcVsc.Text = car.cVsc;
                //    txtcZdzc.Text = car.cZdzc;
                //    txtcDphj.Text = car.cDphj;
                //    txtcKbxg.Text = car.cKbxg;
                //    txtcKqxg.Text = car.cKqxg;
                //    txtcKbzxb.Text = car.cKbzxb;
                //    txtcZdtc.Text = car.cZdtc;
                //    txtcQjtc.Text = car.cQjtc;
                //    txtcYdwg.Text = car.cYdwg;
                //    txtcLhjn.Text = car.cLhjn;
                //    txtcDdxhm.Text = car.cDdxhm;
                //    txtcZpfxp.Text = car.cZpfxp;
                //    txtcSxtj.Text = car.cSxtj;
                //    txtcQhtj.Text = car.cQhtj;
                //    txtcDdtj.Text = car.cDdtj;
                //    txtcDgnfxp.Text = car.cDgnfxp;
                //    txtcFxphd.Text = car.cFxphd;
                //    txtcDsxh.Text = car.cDsxh;
                //    txtcBcfz.Text = car.cBcfz;
                //    txtcDcsp.Text = car.cDcsp;
                //    txtcDnxsp.Text = car.cDnxsp;
                //    txtcHud.Text = car.cHud;
                //    txtcZpzy.Text = car.cZpzy;
                //    txtcYdzy.Text = car.cYdzy;
                //    txtcZygd.Text = car.cZygd;
                //    txtcYbzc.Text = car.cYbzc;
                //    txtcJbzc.Text = car.cJbzc;
                //    txtcQpddtj.Text = car.cQpddtj;
                //    txtcEpjdtj.Text = car.cEpjdtj;
                //    txtcEpzyyd.Text = car.cEpzyyd;
                //    txtcHptj.Text = car.cHptj;
                //    txtcDdjy.Text = car.cDdjy;
                //    txtcQpzyjr.Text = car.cQpzyjr;
                //    txtcHpzyjr.Text = car.cHpzyjr;
                //    txtcZytf.Text = car.cZytf;
                //    txtcZyam.Text = car.cZyam;
                //    txtcHpztpf.Text = car.cHpztpf;
                //    txtcHpblpf.Text = car.cHpblpf;
                //    txtcSpzy.Text = car.cSpzy;
                //    txtcQzfs.Text = car.cQzfs;
                //    txtcHzfs.Text = car.cHzfs;
                //    txtcHphj.Text = car.cHphj;
                //    txtcDdhbx.Text = car.cDdhbx;
                //    txtcGps.Text = car.cGps;
                //    txtcDwfw.Text = car.cDwfw;
                //    txtcCsdp.Text = car.cCsdp;
                //    txtcRjjh.Text = car.cRjjh;
                //    txtcNzyp.Text = car.cNzyp;
                //    txtcCzdh.Text = car.cCzdh;
                //    txtcCzds.Text = car.cCzds;
                //    txtcHpyjp.Text = car.cHpyjp;
                //    txtcIpod.Text = car.cIpod;
                //    txtcMp3.Text = car.cMp3;
                //    txtcDdcd.Text = car.cDdcd;
                //    txtcXndd.Text = car.cXndd;
                //    txtcDuodcd.Text = car.cDuodcd;
                //    txtcDddvd.Text = car.cDddvd;
                //    txtcDuodvd.Text = car.cDuodvd;
                //    txtc23lb.Text = car.c23lb;
                //    txtc45lb.Text = car.c45lb;
                //    txtc67lb.Text = car.c67lb;
                //    txtc8lb.Text = car.c8lb;
                //    txtcXqdd.Text = car.cXqdd;
                //    txtcLed.Text = car.cLed;
                //    txtcRjxcd.Text = car.cRjxcd;
                //    txtcZdtd.Text = car.cZdtd;
                //    txtcZxtd.Text = car.cZxtd;
                //    txtcQwd.Text = car.cQwd;
                //    txtcGdkt.Text = car.cGdkt;
                //    txtcQxzz.Text = car.cQxzz;
                //    txtcCnfwd.Text = car.cCnfwd;
                //    txtcQddcc.Text = car.cQddcc;
                //    txtcHddcc.Text = car.cHddcc;
                //    txtcFjs.Text = car.cFjs;
                //    txtcFzwx.Text = car.cFzwx;
                //    txtcHsjtj.Text = car.cHsjtj;
                //    txtcHsjjr.Text = car.cHsjjr;
                //    txtcFxm.Text = car.cFxm;
                //    txtcZdzd.Text = car.cZdzd;
                //    txtcHsjjy.Text = car.cHsjjy;
                //    txtcHpcz.Text = car.cHpcz;
                //    txtcHzj.Text = car.cHzj;
                //    txtcHys.Text = car.cHys;
                //    txtcGyys.Text = car.cGyys;
                //    txtcSdkt.Text = car.cSdkt;
                //    txtcZdkt.Text = car.cZdkt;
                //    txtcDlkt.Text = car.cDlkt;
                //    txtcHzcfk.Text = car.cHzcfk;
                //    txtcWdkz.Text = car.cWdkz;
                //    txtcKqtj.Text = car.cKqtj;
                //    txtcCzbx.Text = car.cCzbx;
                //    txtcPcrw.Text = car.cPcrw;
                //    txtcBxfz.Text = car.cBxfz;
                //    txtcZdsc.Text = car.cZdsc;
                //    txtcZtzx.Text = car.cZtzx;
                //    txtcYsxt.Text = car.cYsxt;
                //    txtcFpxs.Text = car.cFpxs;
                //    txtcZsyxh.Text = car.cZsyxh;
                //    txtcQjsxt.Text = car.cQjsxt;
                //    hdncQcys.Value = car.cQcys;

                //    if (!string.IsNullOrEmpty(car.cQcys))
                //    {
                //        DataTable t = new DataTable();
                //        t.Columns.Add("Name");
                //        t.Columns.Add("Color");

                //        foreach (string colorinfo in car.cQcys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                //        {
                //            string[] colors = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //            if (colors.Length == 2)
                //            {
                //                DataRow row = t.NewRow();
                //                row["Name"] = colors[0];
                //                row["Color"] = colors[1];
                //                t.Rows.Add(row);
                //            }
                //        }

                //        rptcQcys.DataSource = t;
                //        rptcQcys.DataBind();
                //    }
                //    else
                //    {
                //        rptcQcys.DataSource = null;
                //        rptcQcys.DataBind();
                //    }

                //    #endregion

                //    lblMsg.Text = string.Empty;
                //}

                #endregion
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                WriteErrorMessage("保存失败", ex.Message, string.IsNullOrEmpty(FromUrl) ? "~/car/carcollect.aspx" : FromUrl);
            }
            finally
            {
                ClientScript.RegisterStartupScript(typeof(string),"aa","document.getElementById(\"btnCollectInfo\").removeAttribute(\"disabled\");",true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CarInfo car = new CarInfo();

            car.cChangs = txtcChangs.Text;
            car.cCxmc = txtcCxmc.Text;
            car.fZdj = DataConvert.SafeDecimal(txtfZdj.Text);

            #region 已删除


            //car.cJibie = txtcJibie.Text;
            //car.cFdj = txtcFdj.Text;
            //car.cBsx = txtcBsx.Text;
            //car.cCkg = txtcCkg.Text;
            //car.cCsjg = txtcCsjg.Text;
            //car.cZgcs = txtcZgcs.Text;
            //car.cGfjs = txtcGfjs.Text;
            //car.cScjs = txtcScjs.Text;
            //car.cSczd = txtcSczd.Text;
            //car.cScyh = txtcScyh.Text;
            //car.cGxbyh = txtcGxbyh.Text;
            //car.cZczb = txtcZczb.Text;
            //car.cChang = txtcChang.Text;
            //car.cKuan = txtcKuan.Text;
            //car.cGao = txtcGao.Text;
            //car.cZhouju = txtcZhouju.Text;
            //car.cQnju = txtcQnju.Text;
            //car.cHnju = txtcHnju.Text;
            //car.cLdjx = txtcLdjx.Text;
            //car.cZbzl = txtcZbzl.Text;
            //car.cChesjg = txtcChesjg.Text;
            //car.cCms = txtcCms.Text;
            //car.cZws = txtcZws.Text;
            //car.cYxrj = txtcYxrj.Text;
            //car.cXlxrj = txtcXlxrj.Text;
            //car.cFdjxh = txtcFdjxh.Text;
            //car.fPail = txtfPail.Text;
            //car.cJqxs = txtcJqxs.Text;
            //car.cQgpl = txtcQgpl.Text;
            //car.fQgs = DataConvert.SafeInt(txtfQgs.Text);
            //car.cQms = txtcQms.Text;
            //car.cYsb = txtcYsb.Text;
            //car.cPqjg = txtcPqjg.Text;
            //car.cGangj = txtcGangj.Text;
            //car.cChongc = txtcChongc.Text;
            //car.cZdml = txtcZdml.Text;
            //car.cZdgl = txtcZdgl.Text;
            //car.cZhuans = txtcZhuans.Text;
            //car.cZdlz = txtcZdlz.Text;
            //car.cLzzs = txtcLzzs.Text;
            //car.cTyjs = txtcTyjs.Text;
            //car.cRlxs = txtcRlxs.Text;
            //car.cRybh = txtcRybh.Text;
            //car.cGyfs = txtcGyfs.Text;
            //car.cGgcl = txtcGgcl.Text;
            //car.cGtcl = txtcGtcl.Text;
            //car.cHbbz = txtcHbbz.Text;
            //car.cJianc = txtcJianc.Text;
            //car.cDwgs = txtcDwgs.Text;
            //car.cBsxlx = txtcBsxlx.Text;
            //car.cQdfs = txtcQdfs.Text;
            //car.cQxglx = txtcQxglx.Text;
            //car.cHxglx = txtcHxglx.Text;
            //car.cZllx = txtcZllx.Text;
            //car.cCtjg = txtcCtjg.Text;
            //car.cQzdq = txtcQzdq.Text;
            //car.cHzdq = txtcHzdq.Text;
            //car.cZczd = txtcZczd.Text;
            //car.cQnt = txtcQnt.Text;
            //car.cHnt = txtcHnt.Text;
            //car.cBetai = txtcBetai.Text;
            //car.cJszqls = txtcJszqls.Text;
            //car.cFjsqls = txtcFjsqls.Text;
            //car.cQpcql = txtcQpcql.Text;
            //car.cHpcql = txtcHpcql.Text;
            //car.cQptb = txtcQptb.Text;
            //car.cHptb = txtcHptb.Text;
            //car.cQbql = txtcQbql.Text;
            //car.cTyjc = txtcTyjc.Text;
            //car.cLty = txtcLty.Text;
            //car.cHqdts = txtcHqdts.Text;
            //car.cIso = txtcIso.Text;
            //car.cLatch = txtcLatch.Text;
            //car.cFdjfd = txtcFdjfd.Text;
            //car.cCnzks = txtcCnzks.Text;
            //car.cYkys = txtcYkys.Text;
            //car.cWysqd = txtcWysqd.Text;
            //car.cAbs = txtcAbs.Text;
            //car.cCbc = txtcCbc.Text;
            //car.cBa = txtcBa.Text;
            //car.cTrc = txtcTrc.Text;
            //car.cVsc = txtcVsc.Text;
            //car.cZdzc = txtcZdzc.Text;
            //car.cDphj = txtcDphj.Text;
            //car.cKbxg = txtcKbxg.Text;
            //car.cKqxg = txtcKqxg.Text;
            //car.cKbzxb = txtcKbzxb.Text;
            //car.cZdtc = txtcZdtc.Text;
            //car.cQjtc = txtcQjtc.Text;
            //car.cYdwg = txtcYdwg.Text;
            //car.cLhjn = txtcLhjn.Text;
            //car.cDdxhm = txtcDdxhm.Text;
            //car.cZpfxp = txtcZpfxp.Text;
            //car.cSxtj = txtcSxtj.Text;
            //car.cQhtj = txtcQhtj.Text;
            //car.cDdtj = txtcDdtj.Text;
            //car.cDgnfxp = txtcDgnfxp.Text;
            //car.cFxphd = txtcFxphd.Text;
            //car.cDsxh = txtcDsxh.Text;
            //car.cBcfz = txtcBcfz.Text;
            //car.cDcsp = txtcDcsp.Text;
            //car.cDnxsp = txtcDnxsp.Text;
            //car.cHud = txtcHud.Text;
            //car.cZpzy = txtcZpzy.Text;
            //car.cYdzy = txtcYdzy.Text;
            //car.cZygd = txtcZygd.Text;
            //car.cYbzc = txtcYbzc.Text;
            //car.cJbzc = txtcJbzc.Text;
            //car.cQpddtj = txtcQpddtj.Text;
            //car.cEpjdtj = txtcEpjdtj.Text;
            //car.cEpzyyd = txtcEpzyyd.Text;
            //car.cHptj = txtcHptj.Text;
            //car.cDdjy = txtcDdjy.Text;
            //car.cQpzyjr = txtcQpzyjr.Text;
            //car.cHpzyjr = txtcHpzyjr.Text;
            //car.cZytf = txtcZytf.Text;
            //car.cZyam = txtcZyam.Text;
            //car.cHpztpf = txtcHpztpf.Text;
            //car.cHpblpf = txtcHpblpf.Text;
            //car.cSpzy = txtcSpzy.Text;
            //car.cQzfs = txtcQzfs.Text;
            //car.cHzfs = txtcHzfs.Text;
            //car.cHphj = txtcHphj.Text;
            //car.cDdhbx = txtcDdhbx.Text;
            //car.cGps = txtcGps.Text;
            //car.cDwfw = txtcDwfw.Text;
            //car.cCsdp = txtcCsdp.Text;
            //car.cRjjh = txtcRjjh.Text;
            //car.cNzyp = txtcNzyp.Text;
            //car.cCzdh = txtcCzdh.Text;
            //car.cCzds = txtcCzds.Text;
            //car.cHpyjp = txtcHpyjp.Text;
            //car.cIpod = txtcIpod.Text;
            //car.cMp3 = txtcMp3.Text;
            //car.cDdcd = txtcDdcd.Text;
            //car.cXndd = txtcXndd.Text;
            //car.cDuodcd = txtcDuodcd.Text;
            //car.cDddvd = txtcDddvd.Text;
            //car.cDuodvd = txtcDuodvd.Text;
            //car.c23lb = txtc23lb.Text;
            //car.c45lb = txtc45lb.Text;
            //car.c67lb = txtc67lb.Text;
            //car.c8lb = txtc8lb.Text;
            //car.cXqdd = txtcXqdd.Text;
            //car.cLed = txtcLed.Text;
            //car.cRjxcd = txtcRjxcd.Text;
            //car.cZdtd = txtcZdtd.Text;
            //car.cZxtd = txtcZxtd.Text;
            //car.cQwd = txtcQwd.Text;
            //car.cGdkt = txtcGdkt.Text;
            //car.cQxzz = txtcQxzz.Text;
            //car.cCnfwd = txtcCnfwd.Text;
            //car.cQddcc = txtcQddcc.Text;
            //car.cHddcc = txtcHddcc.Text;
            //car.cFjs = txtcFjs.Text;
            //car.cFzwx = txtcFzwx.Text;
            //car.cHsjtj = txtcHsjtj.Text;
            //car.cHsjjr = txtcHsjjr.Text;
            //car.cFxm = txtcFxm.Text;
            //car.cZdzd = txtcZdzd.Text;
            //car.cHsjjy = txtcHsjjy.Text;
            //car.cHpcz = txtcHpcz.Text;
            //car.cHzj = txtcHzj.Text;
            //car.cHys = txtcHys.Text;
            //car.cGyys = txtcGyys.Text;
            //car.cSdkt = txtcSdkt.Text;
            //car.cZdkt = txtcZdkt.Text;
            //car.cDlkt = txtcDlkt.Text;
            //car.cHzcfk = txtcHzcfk.Text;
            //car.cWdkz = txtcWdkz.Text;
            //car.cKqtj = txtcKqtj.Text;
            //car.cCzbx = txtcCzbx.Text;
            //car.cPcrw = txtcPcrw.Text;
            //car.cBxfz = txtcBxfz.Text;
            //car.cZdsc = txtcZdsc.Text;
            //car.cZtzx = txtcZtzx.Text;
            //car.cYsxt = txtcYsxt.Text;
            //car.cFpxs = txtcFpxs.Text;
            //car.cZsyxh = txtcZsyxh.Text;
            //car.cQjsxt = txtcQjsxt.Text;

            #endregion

            car.cQcys = hdncQcys.Value;
            car.cNsys = hdnInnerColor.Value;

            Cars.Instance.Add(car);
            Cars.Instance.ReloadAllCarList();
            Cars.Instance.ReloadCarListBycChangs();

            WriteSuccessMessage("保存成功", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/car/carcollect.aspx" : FromUrl);
        }

        protected string GetInnerColor(string colors)
        {
            string result = string.Empty;

            if (colors.IndexOf(",") > 0)
            {
                string[] colorlist = colors.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                result += "<span>";
                result += "<em class=\"em-top\" style=\"background-color:" + colorlist[0] + ";\"></em>";
                result += "<em class=\"em-bottom\" style=\"background-color:" + colorlist[1] + ";\"></em>";
                result += "<em class=\"em-bg\"></em></span>";
            }
            else
                result = "<span><em style=\"background-color:" + colors + ";\"></em><em class=\"em-bg\"></em></span>";
            return result;
        }

        private string gethttp(string Url, int timeout, int times = 0, int tryTimes = 1, bool useutf8 = false, int delay = 0)
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
                if (string.IsNullOrEmpty(s) && times < tryTimes)
                {
                    System.Threading.Thread.Sleep(1000);
                    s = gethttp(Url, timeout, times, tryTimes, useutf8, delay);
                }
            }
            if (delay > 0)
                Thread.Sleep(delay);
            return s;
        }

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
    }
}