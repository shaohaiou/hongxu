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

namespace Hx.BackAdmin
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Write(Server.UrlEncode("http://bj.hongxu.cn/weixin/act.aspx?wechat_card_js=1&sid=2"));
            List<CardpackInfo> cardlist = WeixinActs.Instance.GetCardlist(3);
            Response.Write(WebHelper.GetClientsIP());
            Response.End();
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