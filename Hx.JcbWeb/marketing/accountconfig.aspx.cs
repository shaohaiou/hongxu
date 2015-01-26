using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car.Enum;
using Hx.Tools;
using Hx.Car.Entity;
using Hx.Car;
using System.Data;

namespace Hx.JcbWeb.marketing
{
    public partial class accountconfig : JcbBase
    {
        private JcbSiteType? _currentsitetype = null;
        private JcbSiteType? CurrentSiteType
        {
            get
            {
                if (_currentsitetype == null)
                {
                    string sitetypeval = GetString("sitetypeval");
                    if (!string.IsNullOrEmpty(sitetypeval))
                    {
                        _currentsitetype = (JcbSiteType)DataConvert.SafeInt(sitetypeval);
                    }
                }
                return _currentsitetype;
            }
        }
        private List<JcbAccountInfo> _listaccount = null;
        private List<JcbAccountInfo> ListAccount
        {
            get
            {
                if (_listaccount == null)
                {
                    _listaccount = Jcbs.Instance.GetAccountList(true);
                }
                return _listaccount;
            }
        }
        private DataTable _jcbaccounttypelist = null;
        private DataTable JcbAccountTypeList
        {
            get
            {
                if (_jcbaccounttypelist == null)
                {
                    _jcbaccounttypelist =  EnumExtensions.ToTable<JcbAccountType>();
                }
                return _jcbaccounttypelist;
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
            ddlJcbAccountType.DataSource = JcbAccountTypeList;
            ddlJcbAccountType.DataTextField = "Name";
            ddlJcbAccountType.DataValueField = "Value";
            ddlJcbAccountType.DataBind();

            if (CurrentSiteType.HasValue)
            {
                rptData.DataSource = ListAccount.FindAll(a=>a.UserID == UserID && a.JcbSiteType == CurrentSiteType.Value).ToList();
                rptData.DataBind();
            }
            else
            {
                Response.Write("非法操作");
                Response.End();
            }
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                JcbAccountInfo info = (JcbAccountInfo)e.Item.DataItem;

                System.Web.UI.WebControls.DropDownList ddlJcbAccountType = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlJcbAccountType");
                if (ddlJcbAccountType != null)
                {
                    ddlJcbAccountType.DataSource = JcbAccountTypeList;
                    ddlJcbAccountType.DataTextField = "Name";
                    ddlJcbAccountType.DataValueField = "Value";
                    ddlJcbAccountType.DataBind();

                    SetSelectedByValue(ddlJcbAccountType, ((int)info.JcbAccountType).ToString());
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                Jcbs.Instance.DeleteAccount(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string accountName = Request["txtAccountName" + i];
                    string password = Request["txtPassword" + i];
                    JcbAccountType accounttype = (JcbAccountType)DataConvert.SafeInt(Request["ddlJcbAccountType" + i]);
                    if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(password))
                    {
                        JcbAccountInfo entity = new JcbAccountInfo
                        {
                            UserID = UserID,
                            AccountName = accountName,
                            Password = password,
                            JcbSiteType = CurrentSiteType.Value,
                            JcbAccountType = accounttype,
                            AddTime = DateTime.Now
                        };
                        Jcbs.Instance.CreateAndUpdateAccount(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtAccountName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtAccountName");
                    System.Web.UI.HtmlControls.HtmlInputPassword txtPassword = (System.Web.UI.HtmlControls.HtmlInputPassword)item.FindControl("txtPassword");
                    System.Web.UI.WebControls.DropDownList ddlJcbAccountType = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlJcbAccountType");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0 && !string.IsNullOrEmpty(txtPassword.Value) && !string.IsNullOrEmpty(txtAccountName.Value))
                        {
                            JcbAccountInfo entity = new JcbAccountInfo
                            {
                                ID = id,
                                UserID = UserID,
                                AccountName = txtAccountName.Value,
                                Password = txtPassword.Value,
                                JcbSiteType = CurrentSiteType.Value,
                                JcbAccountType = (JcbAccountType)DataConvert.SafeInt(ddlJcbAccountType.SelectedValue),
                                AddTime = DateTime.Now
                            };
                            Jcbs.Instance.CreateAndUpdateAccount(entity);
                        }
                    }
                }
            }
            Jcbs.Instance.ReloadAccountListCache();
        }
    }
}