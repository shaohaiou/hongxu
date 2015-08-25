using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class scenecodemg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator)
            {
                Response.Redirect("~/weixin/scenecodelist.aspx?sid=" + GetInt("sid"));
                Response.End();
            }
        }

        private ScenecodeSettingInfo currentsetting = null;
        protected ScenecodeSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                {
                    int sid = GetInt("sid");
                    currentsetting = WeixinActs.Instance.GetScenecodeSetting(sid);
                }
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
            if (CurrentSetting != null)
            {
                hdnPowerUser.Value = CurrentSetting.PowerUser;
                hdnName.Value = CurrentSetting.Name;
            }
            if (HXContext.Current.AdminUser.Administrator)
            {
                List<AdminInfo> adminlist = Admins.Instance.GetAllAdmins();
                adminlist = adminlist.FindAll(a => !a.Administrator && (a.UserRole & Components.Enumerations.UserRoleType.场景二维码) > 0);
                rptPowerUser.DataSource = adminlist;
                rptPowerUser.DataBind();
            }
        }

        private void FillData(ScenecodeSettingInfo entity)
        {
            entity.ID = GetInt("sid");
            entity.PowerUser = hdnPowerUser.Value;
            entity.Name = hdnName.Value;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            ScenecodeSettingInfo setting = new ScenecodeSettingInfo();
            FillData(setting);

            WeixinActs.Instance.AddScenecodeSetting(setting);
            WeixinActs.Instance.ReloadScenecodeSetting();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", "~/weixin/scenecodemg.aspx?sid=" + GetInt("sid"));
        }

        protected string SetPowerUser(string id)
        {
            string result = string.Empty;

            if (CurrentSetting != null)
            {
                string[] powerusers = CurrentSetting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (powerusers.Contains(id))
                    result = "checked=\"checked\"";
            }

            return result;
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