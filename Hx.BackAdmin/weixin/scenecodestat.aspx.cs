using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components.Web;
using Hx.Components;
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class scenecodestat : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.场景二维码) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator)
            {
                int sid = GetInt("sid");
                ScenecodeSettingInfo setting = WeixinActs.Instance.GetScenecodeSetting(sid, true);
                if (!setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(AdminID.ToString()))
                {
                    Response.Clear();
                    Response.Write("您没有权限操作！");
                    Response.End();
                }
            }

        }

        public string LabelX
        {
            get
            {
                string result = string.Empty;
                int sid = GetInt("sid");
                List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(sid, true);
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        result += (!string.IsNullOrEmpty(result) ? "," : string.Empty) + "'" + list[i].SceneName + "'";
                    }
                }
                return result;
            }
        }

        public string Data
        {
            get
            {
                string result = string.Empty;
                int sid = GetInt("sid");
                List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(sid, true);
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        result += (!string.IsNullOrEmpty(result) ? "," : string.Empty)  + list[i].ScanNum;
                    }
                }
                return result;
            }
        }

        public string Max
        {
            get
            {
                string result = string.Empty;
                int sid = GetInt("sid");
                List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(sid, true);
                if (list.Count > 0)
                {
                    int max = list.Max(l => l.ScanNum);
                    result = (Math.Ceiling(decimal.Parse(max.ToString()) / 100) * 100).ToString();
                }
                else
                    result = "100";
                return result;
            }
        }

        public string Gap
        {
            get
            {
                return (int.Parse(Max) / 10).ToString();
            }
        }

        private ScenecodeSettingInfo currentsetting = null;
        protected ScenecodeSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                    currentsetting = WeixinActs.Instance.GetScenecodeSetting(GetInt("sid", 1), true);
                return currentsetting;
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