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

namespace Hx.BackAdmin.global
{
    public partial class bankmg : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            rptbank.DataSource = Banks.Instance.GetList();
            rptbank.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                Banks.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    string bankProfitMargin3y = Request["txtBankProfitMargin3y" + i];
                    string bankProfitMargin2y = Request["txtBankProfitMargin2y" + i];
                    string bankProfitMargin1y = Request["txtBankProfitMargin1y" + i];
                    if (!string.IsNullOrEmpty(name))
                    {
                        BankInfo entity = new BankInfo
                        {
                            Name = name,
                            BankProfitMargin3y = bankProfitMargin3y,
                            BankProfitMargin2y = bankProfitMargin2y,
                            BankProfitMargin1y = bankProfitMargin1y,
                        };
                        Banks.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptbank.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtName");
                    System.Web.UI.HtmlControls.HtmlInputText txtBankProfitMargin3y = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtBankProfitMargin3y");
                    System.Web.UI.HtmlControls.HtmlInputText txtBankProfitMargin2y = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtBankProfitMargin2y");
                    System.Web.UI.HtmlControls.HtmlInputText txtBankProfitMargin1y = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtBankProfitMargin1y");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            BankInfo entity = new BankInfo
                            {
                                ID = id,
                                Name = txtName.Value,
                                BankProfitMargin3y = txtBankProfitMargin3y.Value,
                                BankProfitMargin2y = txtBankProfitMargin2y.Value,
                                BankProfitMargin1y = txtBankProfitMargin1y.Value,
                            };
                            Banks.Instance.Update(entity);
                        }
                    }
                }
            }

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/global/bankmg.aspx" : FromUrl);
        }
    }
}