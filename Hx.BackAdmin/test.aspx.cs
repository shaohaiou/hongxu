using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools.Web;
using Hx.Car;

namespace Hx.BackAdmin
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //List<CardpackInfo> cardlist = WeixinActs.Instance.GetCardlist(3);
            if (Request.QueryString["t"] == "1")
            {
                Response.Write(WebHelper.GetClientsIP());
                Response.End();
            }
            else if (Request.QueryString["t"] == "2") 
            { 
                Response.Write(Server.UrlEncode("http://bj.hongxu.cn/weixin/act.aspx?wechat_card_js=1&sid=2"));
            }
            else if (Request.QueryString["t"] == "3") //查询多家公司数据的用户
            {
                List<DayReportUserInfo> userlist = DayReportUsers.Instance.GetList(true);
                List<DayReportUserInfo> userlist1 = userlist.FindAll(u => u.CorporationID != 1 && u.DayReportViewCorpPowerSetting.Trim('|').Contains("|"));
                List<DayReportUserInfo> userlist2 = userlist.FindAll(u => u.CorporationID == 1 && u.DayReportViewCorpPowerSetting.Trim('|').Contains("|"));
                Response.Write("非集团总部：<br>");
                foreach (DayReportUserInfo user in userlist1)
                {
                    string[] cids = user.DayReportViewCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    Response.Write(user.UserName + ":" + string.Join(",",Corporations.Instance.GetList(true).FindAll(c=>cids.Contains(c.ID.ToString())).Select(c=>c.Name)) + "<br>");
                }
                Response.Write("集团总部：<br>");
                foreach (DayReportUserInfo user in userlist2)
                {
                    string[] cids = user.DayReportViewCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    Response.Write(user.UserName + ":" + string.Join(",", Corporations.Instance.GetList(true).FindAll(c => cids.Contains(c.ID.ToString())).Select(c => c.Name)) + "<br>");
                }
                Response.End();
            }
            //string s = string.Empty;
            //s.GetHashCode();
            //Response.Write(FormatNum(""));

            //List<DayReportUserInfo> userlist = DayReportUsers.Instance.GetList(true);
            //Response.Write(string.Join("<br>",userlist.FindAll(u=>u.AllowModify == "1" && u.DayReportDep != Components.Enumerations.DayReportDep.财务部).Select(u=>u.UserName)));


        }
        private string FormatNum(string num)
        {
            string newstr = string.Empty;
            Regex r = new Regex(@"(\d+?)(\d{3})*(\.\d+|$)");
            Match m = r.Match(num); 
            newstr += m.Groups[1].Value;
            for (int i = 0; i < m.Groups[2].Captures.Count; i++)
            {
                newstr += "," + m.Groups[2].Captures[i].Value;
            }
            newstr += m.Groups[3].Value; return newstr;
        }
    }
}