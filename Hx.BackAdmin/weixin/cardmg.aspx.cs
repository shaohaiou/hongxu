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
            if (!HXContext.Current.AdminUser.Administrator)
            {
                int sid = GetInt("sid");
                CardSettingInfo setting = WeixinActs.Instance.GetCardSetting(sid, true);
                if (!setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(AdminID.ToString()))
                {
                    Response.Clear();
                    Response.Write("您没有权限操作！");
                    Response.End();
                }
            }
        }

        private CardSettingInfo currentsetting = null;
        protected CardSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                {
                    int sid = GetInt("sid");
                    currentsetting = WeixinActs.Instance.GetCardSetting(sid);
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
            rpcg.DataSource = WeixinActs.Instance.GetCardSettingList(true);
            rpcg.DataBind();
            if (CurrentSetting != null)
            {
                cbxSwitch.Checked = CurrentSetting.Switch == 1;
                cbxMustAttention.Checked = CurrentSetting.MustAttention == 1;
                txtAppID.Text = CurrentSetting.AppID;
                txtAppSecret.Text = CurrentSetting.AppSecret;
                txtAppNumber.Text = CurrentSetting.AppNumber;
                txtAppName.Text = CurrentSetting.AppName;
                txtActRule.Text = CurrentSetting.ActRule;
                if (!string.IsNullOrEmpty(CurrentSetting.ColorRule))
                    txtColorRule.Text = CurrentSetting.ColorRule;
                txtAwards.Text = CurrentSetting.Awards;
                if (!string.IsNullOrEmpty(CurrentSetting.ColorAwards))
                    txtColorAward.Text = CurrentSetting.ColorAwards;
                txtAttentionUrl.Text = CurrentSetting.AttentionUrl;
                if (!string.IsNullOrEmpty(CurrentSetting.AppImgUrl))
                    imgAppImg.Src = CurrentSetting.AppImgUrl;
                hdnAppImg.Value = CurrentSetting.AppImgUrl;
                if (!string.IsNullOrEmpty(CurrentSetting.BgImgUrl))
                    imgBgImg.Src = CurrentSetting.BgImgUrl;
                hdnBgImg.Value = CurrentSetting.BgImgUrl;
                txtWinRate.Text = CurrentSetting.WinRate.ToString();
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
                adminlist = adminlist.FindAll(a=>!a.Administrator && (a.UserRole & Components.Enumerations.UserRoleType.卡券活动管理员) > 0);
                rptPowerUser.DataSource = adminlist;
                rptPowerUser.DataBind();
            }
        }

        private void FillData(CardSettingInfo entity)
        {
            entity.ID = GetInt("sid");
            entity.Switch = cbxSwitch.Checked ? 1 : 0;
            entity.MustAttention = cbxMustAttention.Checked ? 1 : 0;
            entity.AppID = txtAppID.Text;
            entity.AppSecret = txtAppSecret.Text;
            entity.AppNumber = txtAppNumber.Text;
            entity.ActRule = txtActRule.Text;
            entity.ColorRule = txtColorRule.Text;
            entity.Awards = txtAwards.Text;
            entity.ColorAwards = txtColorAward.Text;
            entity.AppImgUrl = hdnAppImg.Value;
            entity.AttentionUrl = txtAttentionUrl.Text;
            entity.BgImgUrl = hdnBgImg.Value;
            entity.AppName = txtAppName.Text;
            entity.WinRate = DataConvert.SafeInt(txtWinRate.Text, 100);
            entity.ShareImgUrl = hdimage_pic.Value;
            entity.ShareLinkUrl = txtShareLinkUrl.Text;
            entity.ShareTitle = txtShareTitle.Text;
            entity.ShareDesc = txtShareDesc.Text;
            entity.PowerUser = hdnPowerUser.Value;
            entity.Name = hdnName.Value;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            CardSettingInfo setting = new CardSettingInfo();
            FillData(setting);

            WeixinActs.Instance.AddCardSetting(setting);
            WeixinActs.Instance.ReloadCardSetting();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", "~/weixin/cardmg.aspx?sid=" + GetInt("sid"));
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

        protected string SetCardSettingStatus(string id)
        {
            string result = string.Empty;

            if (!Admin.Administrator)
            {
                CardSettingInfo setting = WeixinActs.Instance.GetCardSetting(DataConvert.SafeInt(id), true);

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