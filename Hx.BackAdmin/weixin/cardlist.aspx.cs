using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools;
using Hx.Components.Entity;
using Hx.Components.Web;

namespace Hx.BackAdmin.weixin
{
    public partial class cardlist : AdminBase
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            int sid = GetInt("sid");
            rptData.DataSource = WeixinActs.Instance.GetCardidInfolist(sid,true);
            rptData.DataBind();
            rpcg.DataSource = WeixinActs.Instance.GetCardSettingList(true);
            rpcg.DataBind();
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CardidInfo info = (CardidInfo)e.Item.DataItem;
                System.Web.UI.WebControls.TextBox txtCardid = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtCardid");
                System.Web.UI.WebControls.TextBox txtCardtitle = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtCardtitle");
                System.Web.UI.WebControls.TextBox txtAward = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtAward");
                System.Web.UI.WebControls.TextBox txtNum = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtNum");

                if (info != null)
                {
                    txtCardid.Text = info.Cardid;
                    txtCardtitle.Text = info.Cardtitle;
                    txtAward.Text = info.Award;
                    txtNum.Text = info.Num.ToString();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int sid = GetInt("sid");
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                WeixinActs.Instance.DeleteCardidInfo(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string cardid = Request["txtCardid" + i];
                    string cardtitle = Request["txtCardtitle" + i];
                    string award = Request["txtAward" + i];
                    int num = DataConvert.SafeInt(Request["txtNum" + i]);
                    string imgurl = Request["hdnImgUrl" + i];
                    if (!string.IsNullOrEmpty(cardid + cardtitle + award))
                    {
                        CardidInfo entity = new CardidInfo
                        {
                            SID = sid,
                            Cardid = cardid,
                            Cardtitle = cardtitle,
                            Award = award,
                            Num = num,
                            ImgUrl = imgurl
                        };
                        WeixinActs.Instance.AddCardidInfo(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.WebControls.TextBox txtCardid = (System.Web.UI.WebControls.TextBox)item.FindControl("txtCardid");
                    System.Web.UI.WebControls.TextBox txtCardtitle = (System.Web.UI.WebControls.TextBox)item.FindControl("txtCardtitle");
                    System.Web.UI.WebControls.TextBox txtAward = (System.Web.UI.WebControls.TextBox)item.FindControl("txtAward");
                    System.Web.UI.WebControls.TextBox txtNum = (System.Web.UI.WebControls.TextBox)item.FindControl("txtNum");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnImgUrl = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnImgUrl");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            CardidInfo entity = new CardidInfo
                            {
                                SID = sid,
                                ID = id,
                                Cardid = txtCardid.Text,
                                Cardtitle = txtCardtitle.Text,
                                Award = txtAward.Text,
                                Num = DataConvert.SafeInt(txtNum.Text),
                                ImgUrl = hdnImgUrl.Value
                            };
                            WeixinActs.Instance.UpdateCardidInfo(entity);
                        }
                    }
                }
            }
            WeixinActs.Instance.ReloadCardidListCache(sid);
            WeixinActs.Instance.ReloadCardlist(sid);

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? ("~/weixin/cardlist.aspx?sid=" + GetInt("sid")) : FromUrl);
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