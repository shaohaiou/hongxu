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
    public partial class cardsettinglist : AdminBase
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
                if (!HXContext.Current.AdminUser.Administrator)
                {
                    int id = 0;
                    List<CardSettingInfo> list = WeixinActs.Instance.GetCardSettingList(true);
                    if (list.Exists(c=>c.PowerUser.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries).Contains(AdminID.ToString())))
                    {
                        id = list.Find(c => c.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(AdminID.ToString())).ID;
                    }

                    Response.Redirect("~/weixin/cardmg.aspx?sid=" + id);
                    Response.End();
                }
                LoadData();
            }
        }

        private void LoadData()
        {
            rptData.DataSource = WeixinActs.Instance.GetCardSettingList(true);
            rptData.DataBind();
            rpcg.DataSource = WeixinActs.Instance.GetCardSettingList(true);
            rpcg.DataBind();
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CardSettingInfo info = (CardSettingInfo)e.Item.DataItem;
                System.Web.UI.WebControls.TextBox txtName = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtName");

                if (info != null)
                {
                    txtName.Text = info.Name;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                WeixinActs.Instance.DeleteCardSetting(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    if (!string.IsNullOrEmpty(name))
                    {
                        CardSettingInfo entity = new CardSettingInfo
                        {
                            Name = name
                        };
                        WeixinActs.Instance.AddCardSetting(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.WebControls.TextBox txtName = (System.Web.UI.WebControls.TextBox)item.FindControl("txtName");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {

                            CardSettingInfo entity = WeixinActs.Instance.GetCardSetting(id,true);
                            entity.Name = txtName.Text;
                            WeixinActs.Instance.AddCardSetting(entity);
                        }
                    }
                }
            }
            WeixinActs.Instance.ReloadCardSetting();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/weixin/cardsettinglist.aspx" : FromUrl);
        }
    }
}