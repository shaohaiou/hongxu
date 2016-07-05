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
                cbxIsRepeatOneday.Checked = CurrentSetting.IsrepeatOnday == 1;
                cbxIsRepeat.Checked = CurrentSetting.Isrepeat == 1;
                txtAppID.Text = CurrentSetting.AppID;
                txtAppSecret.Text = CurrentSetting.AppSecret;
                txtAppNumber.Text = CurrentSetting.AppNumber;
                txtAppName.Text = CurrentSetting.AppName;
                cbxMustAttention.Checked = CurrentSetting.MustAttention == 1;
                txtAttentionUrl.Text = CurrentSetting.AttentionUrl;
                cbxShowAppImg.Checked = CurrentSetting.ShowAppImg == 1;
                if (!string.IsNullOrEmpty(CurrentSetting.AppImgUrl))
                    imgAppImg.Src = ImgServer + CurrentSetting.AppImgUrl;
                hdnAppImg.Value = CurrentSetting.AppImgUrl;
                if (!string.IsNullOrEmpty(CurrentSetting.PageHeadImg))
                    imgPageHeadImg.Src = ImgServer + CurrentSetting.PageHeadImg;
                hdnPageHeadImg.Value = CurrentSetting.PageHeadImg;
                txtMustKnow.Text = CurrentSetting.MustKnow;
                if (!string.IsNullOrEmpty(CurrentSetting.ColorMustKnow))
                    txtColorMustKnow.Text = CurrentSetting.ColorMustKnow;
                txtOverdueMinutes.Text = CurrentSetting.OverdueMinutes.ToString();
                txtVoteTimes.Text = CurrentSetting.VoteTimes.ToString();
                txtVoteTimesMax.Text = CurrentSetting.VoteTimesMax.ToString();
                if (!string.IsNullOrEmpty(CurrentSetting.ShareImgUrl))
                    imgpic.Src = ImgServer + CurrentSetting.ShareImgUrl;
                hdimage_pic.Value = CurrentSetting.ShareImgUrl;
                txtShareLinkUrl.Text = CurrentSetting.ShareLinkUrl;
                txtShareTitle.Text = CurrentSetting.ShareTitle;
                txtShareDesc.Text = CurrentSetting.ShareDesc;
                hdnPowerUser.Value = CurrentSetting.PowerUser;
                hdnName.Value = CurrentSetting.Name;
                if (!string.IsNullOrEmpty(CurrentSetting.AD1Path))
                    imgAD1Path.Src = ImgServer + CurrentSetting.AD1Path;
                hdnAD1Path.Value = CurrentSetting.AD1Path;
                txtAD1Url.Text = CurrentSetting.AD1Url;
                if (!string.IsNullOrEmpty(CurrentSetting.AD2Path))
                    imgAD2Path.Src = ImgServer + CurrentSetting.AD2Path;
                hdnAD2Path.Value = CurrentSetting.AD2Path;
                txtAD2Url.Text = CurrentSetting.AD2Url;
                if (!string.IsNullOrEmpty(CurrentSetting.AD3Path))
                    imgAD3Path.Src = ImgServer + CurrentSetting.AD3Path;
                hdnAD3Path.Value = CurrentSetting.AD3Path;
                txtAD3Url.Text = CurrentSetting.AD3Url;
                if (!string.IsNullOrEmpty(CurrentSetting.AD4Path))
                    imgAD4Path.Src = ImgServer + CurrentSetting.AD4Path;
                hdnAD4Path.Value = CurrentSetting.AD4Path;
                txtAD4Url.Text = CurrentSetting.AD4Url;
                if (!string.IsNullOrEmpty(CurrentSetting.AD5Path))
                    imgAD5Path.Src = ImgServer + CurrentSetting.AD5Path;
                hdnAD5Path.Value = CurrentSetting.AD5Path;
                txtAD5Url.Text = CurrentSetting.AD5Url;
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
            entity.IsrepeatOnday = cbxIsRepeatOneday.Checked ? 1 : 0;
            entity.Isrepeat = cbxIsRepeat.Checked ? 1 : 0;
            entity.AppID = txtAppID.Text;
            entity.AppSecret = txtAppSecret.Text;
            entity.AppNumber = txtAppNumber.Text;
            entity.AppName = txtAppName.Text;
            entity.MustAttention = cbxMustAttention.Checked ? 1 : 0;
            entity.AttentionUrl = txtAttentionUrl.Text;
            entity.ShowAppImg = cbxShowAppImg.Checked ? 1 : 0;
            entity.AppImgUrl = hdnAppImg.Value;
            entity.PageHeadImg = hdnPageHeadImg.Value;
            entity.MustKnow = txtMustKnow.Text;
            entity.ColorMustKnow = txtColorMustKnow.Text;
            entity.OverdueMinutes = DataConvert.SafeInt(txtOverdueMinutes.Text);
            entity.VoteTimes = DataConvert.SafeInt(txtVoteTimes.Text);
            entity.VoteTimesMax = DataConvert.SafeInt(txtVoteTimesMax.Text);

            entity.ShareImgUrl = hdimage_pic.Value;
            entity.ShareLinkUrl = txtShareLinkUrl.Text;
            entity.ShareTitle = txtShareTitle.Text;
            entity.ShareDesc = txtShareDesc.Text;

            entity.AD1Path = hdnAD1Path.Value;
            entity.AD1Url = txtAD1Url.Text;
            entity.AD2Path = hdnAD2Path.Value;
            entity.AD2Url = txtAD2Url.Text;
            entity.AD3Path = hdnAD3Path.Value;
            entity.AD3Url = txtAD3Url.Text;
            entity.AD4Path = hdnAD4Path.Value;
            entity.AD4Url = txtAD4Url.Text;
            entity.AD5Path = hdnAD5Path.Value;
            entity.AD5Url = txtAD5Url.Text;

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