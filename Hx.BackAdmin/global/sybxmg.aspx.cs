using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car;
using Hx.Tools;
using Hx.Car.Entity;

namespace Hx.BackAdmin.global
{
    public partial class sybxmg : AdminBase
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
            rptsybx.DataSource = Sybxs.Instance.GetList();
            rptsybx.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                Sybxs.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    if (!string.IsNullOrEmpty(name))
                    {
                        SybxInfo entity = new SybxInfo();
                        entity.Name = name;

                        Sybxs.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptsybx.Items)
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
                            SybxInfo entity = new SybxInfo();
                            entity.Name = txtName.Value;
                            entity.ID = id;

                            Sybxs.Instance.Update(entity);
                        }
                    }
                }
            }

            WriteSuccessMessage("保存成功！", "数据已经成功保存！",string.IsNullOrEmpty(FromUrl) ? "~/global/sybxmg.aspx" : FromUrl);
        }
    }
}