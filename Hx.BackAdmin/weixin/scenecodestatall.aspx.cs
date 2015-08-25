using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components;
using System.Text;
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class scenecodestatall : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.微信活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }

        }

        protected string Count
        {
            get
            {
                int count = 0;
                List<ScenecodeSettingInfo> settinglist = WeixinActs.Instance.GetScenecodeSettingList(true);
                foreach (ScenecodeSettingInfo setting in settinglist)
                {
                    List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(setting.ID, true);
                    count += list.Sum(l => l.ScanNum);
                }

                return count.ToString();
            }
        }

        protected string TblStr
        {
            get
            {
                StringBuilder strb = new StringBuilder();
                List<ScenecodeSettingInfo> settinglist = WeixinActs.Instance.GetScenecodeSettingList(true);
                if (settinglist.Count > 0)
                {
                    List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(settinglist[0].ID, true);

                    strb.AppendLine("<table style=\"border-spacing: 0;\">");
                    strb.AppendLine("<tr style=\"background:#ccc;font-weight:bold;\">");
                    strb.AppendLine("<td class=\"w240\">活动名称</td>");
                    for (int i = 0; i < list.Count; i++)
                    {
                        strb.AppendLine("<td class=\"w80\">" + list[i].SceneName + "</td>");
                    }
                    strb.AppendLine("</tr>");

                    for (int i = 0; i < settinglist.Count; i++)
                    {
                        List<ScenecodeInfo> listsub = WeixinActs.Instance.GetScenecodeList(settinglist[i].ID, true);
                        strb.AppendLine("<tr>");
                        strb.AppendLine("<td>" + settinglist[i].Name + "</td>");
                        for (int j = 0; j < list.Count; j++)
                        {
                            strb.AppendLine("<td>" + (listsub.Exists(l=>l.SceneName==list[j].SceneName) ? listsub.Find(l=>l.SceneName==list[j].SceneName).ScanNum.ToString() : "&nbsp") + "</td>");
                        }
                        strb.AppendLine("</tr>");
                    }

                    strb.AppendLine("</table>");
                }

                return strb.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            rpcg.DataSource = WeixinActs.Instance.GetScenecodeSettingList(true);
            rpcg.DataBind();
        }

        protected string SetScenecodeSettingStatus(string id)
        {
            string result = string.Empty;

            if (!Admin.Administrator)
            {
                ScenecodeSettingInfo setting = WeixinActs.Instance.GetScenecodeSetting(DataConvert.SafeInt(id), true);

                if (setting != null)
                {
                    string[] powerusers = setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!powerusers.Contains(AdminID.ToString()))
                        result = "style=\"display:none;\"";
                }
            }

            return result;
        }
    }
}