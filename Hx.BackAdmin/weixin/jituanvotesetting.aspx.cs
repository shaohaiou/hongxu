using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Web;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class jituanvotesetting : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.微信活动管理员) == 0)
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
            JituanvoteSettingInfo setting = WeixinActs.Instance.GetJituanvoteSetting();
            if (setting != null)
            {
                cbxSwitch.Checked = setting.Switch == 1;
                txtMustKnow.Text = setting.MustKnow.ToString();
                txtOverdueMinutes.Text = setting.OverdueMinutes.ToString();
                txtVoteTimes.Text = setting.VoteTimes.ToString();
                if (!string.IsNullOrEmpty(setting.ShareImgUrl))
                    imgpic.Src = setting.ShareImgUrl;
                hdimage_pic.Value = setting.ShareImgUrl;
                txtShareLinkUrl.Text = setting.ShareLinkUrl;
                txtShareTitle.Text = setting.ShareTitle;
                txtShareDesc.Text = setting.ShareDesc;
            }
        }

        private void FillData(JituanvoteSettingInfo entity)
        {
            entity.Switch = cbxSwitch.Checked ? 1 : 0;
            entity.MustKnow = txtMustKnow.Text;
            entity.OverdueMinutes = DataConvert.SafeInt(txtOverdueMinutes.Text);
            entity.VoteTimes = DataConvert.SafeInt(txtVoteTimes.Text);
            entity.ShareImgUrl = hdimage_pic.Value;
            entity.ShareLinkUrl = txtShareLinkUrl.Text;
            entity.ShareTitle = txtShareTitle.Text;
            entity.ShareDesc = txtShareDesc.Text;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            JituanvoteSettingInfo setting = new JituanvoteSettingInfo();
            FillData(setting);

            WeixinActs.Instance.AddJituanvoteSetting(setting);
            WeixinActs.Instance.ReloadJituanvoteSetting();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", "~/weixin/jituanvotesetting.aspx");
        }
    }
}