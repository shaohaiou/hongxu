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
    public partial class scenecodelist : AdminBase
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
            rptData.DataSource = WeixinActs.Instance.GetScenecodeList(sid, true);
            rptData.DataBind();
            rpcg.DataSource = WeixinActs.Instance.GetScenecodeSettingList(true);
            rpcg.DataBind();
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScenecodeInfo info = (ScenecodeInfo)e.Item.DataItem;
                System.Web.UI.WebControls.TextBox txtSceneName = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtSceneName");
                System.Web.UI.WebControls.TextBox txtCodeAddress = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtCodeAddress");
                System.Web.UI.WebControls.TextBox txtRedirectAddress = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtRedirectAddress");

                if (info != null)
                {
                    txtSceneName.Text = info.SceneName;
                    txtCodeAddress.Text = "http://" + CurrentDomain + "/weixin/scenecodevisit.aspx?sid=" + GetInt("sid") + "&id=" + info.ID;
                    txtRedirectAddress.Text = info.RedirectAddress;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int sid = GetInt("sid");
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                WeixinActs.Instance.DeleteScenecodeInfo(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string scenename = Request["txtSceneName" + i];
                    string redirectaddress = Request["txtRedirectAddress" + i];
                    if (!string.IsNullOrEmpty(scenename + redirectaddress))
                    {
                        ScenecodeInfo entity = new ScenecodeInfo
                        {
                            SID = sid,
                            SceneName = scenename,
                            RedirectAddress = redirectaddress,
                            ScanNum = 0,
                        };
                        WeixinActs.Instance.AddScenecodeInfo(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.WebControls.TextBox txtSceneName = (System.Web.UI.WebControls.TextBox)item.FindControl("txtSceneName");
                    System.Web.UI.WebControls.TextBox txtRedirectAddress = (System.Web.UI.WebControls.TextBox)item.FindControl("txtRedirectAddress");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            ScenecodeInfo entity = new ScenecodeInfo
                            {
                                SID = sid,
                                ID = id,
                                SceneName = txtSceneName.Text,
                                RedirectAddress = txtRedirectAddress.Text
                            };
                            WeixinActs.Instance.UpdateScenecodeInfo(entity);
                        }
                    }
                }
            }
            WeixinActs.Instance.ReloadScenecodeListCache(sid);

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? ("~/weixin/scenecodelist.aspx?sid=" + GetInt("sid")) : FromUrl);
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