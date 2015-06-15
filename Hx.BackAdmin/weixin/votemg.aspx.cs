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
    public partial class votemg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.投票活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator)
            {
                int sid = GetInt("sid");
                VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(sid, true);
                if (!setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(AdminID.ToString()))
                {
                    Response.Clear();
                    Response.Write("您没有权限操作！");
                    Response.End();
                }
            }
        }

        private VoteSettingInfo currentsetting = null;
        protected VoteSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                {
                    int sid = GetInt("sid");
                    currentsetting = WeixinActs.Instance.GetVoteSetting(sid);
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
            rpcg.DataSource = WeixinActs.Instance.GetVoteSettingList(true);
            rpcg.DataBind();
            if (CurrentSetting != null)
            {
                cbxSwitch.Checked = CurrentSetting.Switch == 1;
                txtAppID.Text = CurrentSetting.AppID;
                txtAppSecret.Text = CurrentSetting.AppSecret;
                txtAppNumber.Text = CurrentSetting.AppNumber;
                txtAppName.Text = CurrentSetting.AppName;
                txtAttentionUrl.Text = CurrentSetting.AttentionUrl;
                if (!string.IsNullOrEmpty(CurrentSetting.AppImgUrl))
                    imgAppImg.Src = CurrentSetting.AppImgUrl;
                hdnAppImg.Value = CurrentSetting.AppImgUrl;
                if (!string.IsNullOrEmpty(CurrentSetting.PageHeadImg))
                    imgPageHeadImg.Src = CurrentSetting.PageHeadImg;
                hdnPageHeadImg.Value = CurrentSetting.PageHeadImg;
                txtMustKnow.Text = CurrentSetting.MustKnow;
                if (!string.IsNullOrEmpty(CurrentSetting.ColorMustKnow))
                    txtColorMustKnow.Text = CurrentSetting.ColorMustKnow;
                txtOverdueMinutes.Text = CurrentSetting.OverdueMinutes.ToString();
                txtVoteTimes.Text = CurrentSetting.VoteTimes.ToString();
                if (!string.IsNullOrEmpty(CurrentSetting.ShareImgUrl))
                    imgpic.Src = CurrentSetting.ShareImgUrl;
                hdimage_pic.Value = CurrentSetting.ShareImgUrl;
                txtShareLinkUrl.Text = CurrentSetting.ShareLinkUrl;
                txtShareTitle.Text = CurrentSetting.ShareTitle;
                txtShareDesc.Text = CurrentSetting.ShareDesc;
                hdnPowerUser.Value = CurrentSetting.PowerUser;
                hdnName.Value = CurrentSetting.Name;
            }
            if (HXContext.Current.AdminUser.Administrator)
            {
                List<AdminInfo> adminlist = Admins.Instance.GetAllAdmins();
                adminlist = adminlist.FindAll(a => !a.Administrator && (a.UserRole & Components.Enumerations.UserRoleType.投票活动管理员) > 0);
                rptPowerUser.DataSource = adminlist;
                rptPowerUser.DataBind();
            }
        }

        private void FillData(VoteSettingInfo entity)
        {
            entity.ID = GetInt("sid");
            entity.Switch = cbxSwitch.Checked ? 1 : 0;
            entity.AppID = txtAppID.Text;
            entity.AppSecret = txtAppSecret.Text;
            entity.AppNumber = txtAppNumber.Text;
            entity.AppName = txtAppName.Text;
            entity.AttentionUrl = txtAttentionUrl.Text;
            entity.AppImgUrl = hdnAppImg.Value;
            entity.PageHeadImg = hdnPageHeadImg.Value;
            entity.MustKnow = txtMustKnow.Text;
            entity.ColorMustKnow = txtColorMustKnow.Text;
            entity.OverdueMinutes = DataConvert.SafeInt(txtOverdueMinutes.Text);
            entity.VoteTimes = DataConvert.SafeInt(txtVoteTimes.Text);

            entity.ShareImgUrl = hdimage_pic.Value;
            entity.ShareLinkUrl = txtShareLinkUrl.Text;
            entity.ShareTitle = txtShareTitle.Text;
            entity.ShareDesc = txtShareDesc.Text;

            entity.PowerUser = hdnPowerUser.Value;
            entity.Name = hdnName.Value;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            VoteSettingInfo setting = new VoteSettingInfo();
            FillData(setting);

            WeixinActs.Instance.AddVoteSetting(setting);
            WeixinActs.Instance.ReloadVoteSetting();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", "~/weixin/votemg.aspx?sid=" + GetInt("sid"));
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

        protected string SetVoteSettingStatus(string id)
        {
            string result = string.Empty;

            if (!Admin.Administrator)
            {
                VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(DataConvert.SafeInt(id), true);

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