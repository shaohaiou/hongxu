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
    public partial class corporationmg : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            rptcarbrand.DataSource = CarBrands.Instance.GetCarBrandList(true);
            rptcarbrand.DataBind();
            rptCorporation.DataSource = Corporations.Instance.GetList(true);
            rptCorporation.DataBind();
        }

        protected void rptCorporation_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CorporationInfo info = (CorporationInfo)e.Item.DataItem;
                System.Web.UI.WebControls.TextBox txtSort = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtSort");
                txtSort.Text = info.Sort.ToString();
                txtSort.Attributes.Add("oldval", info.Sort.ToString());
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                Corporations.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    int sort = DataConvert.SafeInt(Request["txtSort" + i]);
                    if (!string.IsNullOrEmpty(name))
                    {
                        CorporationInfo entity = new CorporationInfo();
                        entity.Name = name;
                        entity.Sort = sort;
                        entity.CarBrand = string.Empty;

                        Corporations.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptCorporation.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.WebControls.TextBox txtSort = (System.Web.UI.WebControls.TextBox)item.FindControl("txtSort");
                    System.Web.UI.HtmlControls.HtmlInputText txtName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtName");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            CorporationInfo entity = Corporations.Instance.GetModel(id);
                            entity.Sort = DataConvert.SafeInt(txtSort.Text);
                            entity.Name = txtName.Value;
                            entity.ID = id;

                            Corporations.Instance.Update(entity);
                        }
                    }
                }
            }
            Corporations.Instance.ReloadCorporationListCache();
            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/global/corporationmg.aspx" : FromUrl);
        }


    }
}