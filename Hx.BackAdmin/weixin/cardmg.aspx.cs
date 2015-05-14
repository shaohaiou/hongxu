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
    public partial class cardmg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.卡券活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
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
            CardSettingInfo setting = WeixinActs.Instance.GetCardSetting();
            if (setting != null)
            {
                cbxSwitch.Checked = setting.Switch == 1;
                txtAppID.Text = setting.AppID;
                txtAppSecret.Text = setting.AppSecret;
                txtAppName.Text = setting.AppName;
                txtActRule.Text = setting.ActRule;
                txtAwards.Text = setting.Awards;
                if (!string.IsNullOrEmpty(setting.AppImgUrl))
                    imgAppImg.Src = setting.AppImgUrl;
                txtWinRate.Text = setting.WinRate.ToString();
                if (!string.IsNullOrEmpty(setting.ShareImgUrl))
                    imgpic.Src = setting.ShareImgUrl;
                hdimage_pic.Value = setting.ShareImgUrl;
                txtShareLinkUrl.Text = setting.ShareLinkUrl;
                txtShareTitle.Text = setting.ShareTitle;
                txtShareDesc.Text = setting.ShareDesc;
            }
        }

        private void FillData(CardSettingInfo entity)
        {
            entity.Switch = cbxSwitch.Checked ? 1 : 0;
            entity.AppID = txtAppID.Text;
            entity.AppSecret = txtAppSecret.Text;
            entity.ActRule = txtActRule.Text;
            entity.Awards = txtAwards.Text;
            entity.AppImgUrl = hdnAppImg.Value;
            entity.AppName = txtAppName.Text;
            entity.WinRate = DataConvert.SafeInt(txtWinRate.Text,100);
            entity.ShareImgUrl = hdimage_pic.Value;
            entity.ShareLinkUrl = txtShareLinkUrl.Text;
            entity.ShareTitle = txtShareTitle.Text;
            entity.ShareDesc = txtShareDesc.Text;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            CardSettingInfo setting = new CardSettingInfo();
            FillData(setting);

            WeixinActs.Instance.AddCardSetting(setting);
            WeixinActs.Instance.ReloadCardSetting();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", "~/weixin/cardmg.aspx");
        }
    }
}