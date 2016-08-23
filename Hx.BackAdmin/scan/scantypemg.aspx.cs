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
using Hx.Car.Entity;
using Hx.Car;
using Hx.Components.Web;
using Hx.Components.Enumerations;

namespace Hx.BackAdmin.scan
{
    public partial class scantypemg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator)
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
            List<ScanTypeInfo> list = ScanTypes.Instance.GetList(true);
            rptData.DataSource = list;
            rptData.DataBind();
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                ScanTypes.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    int CorporationID = DataConvert.SafeInt(Request["ddlCorporation" + i]);
                    if (!string.IsNullOrEmpty(name))
                    {
                        ScanTypeInfo entity = new ScanTypeInfo
                        {
                            Name = name,
                        };
                        ScanTypes.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtName");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            ScanTypeInfo entity = new ScanTypeInfo
                            {
                                ID = id,
                                Name = txtName.Value
                            };
                            ScanTypes.Instance.Update(entity);
                        }
                    }
                }
            }
            ScanTypes.Instance.ReloadScanTypeListCache();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/scan/scantypemg.aspx" : FromUrl);
        }
    }
}