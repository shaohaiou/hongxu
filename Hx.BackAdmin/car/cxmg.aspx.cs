using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Car;
using Hx.Car.Entity;
using Hx.Tools;
using System.Data;
using Hx.Tools.Web;
using Hx.Car.Enum;
using System.Web.UI.HtmlControls;
using Hx.Components;
using Hx.Components.Entity;

namespace Hx.BackAdmin.car
{
    public partial class cxmg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.车型管理员) == 0)
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
                BindData();
            }
        }

        private void BindData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1) pageindex = 1;
            int pagesize = GetInt("pagesize", 10);

            List<CarBrandInfo> carbrandlist = CarBrands.Instance.GetCarBrandListByCorporation("-1");

            int total = carbrandlist.Count();
            carbrandlist = carbrandlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<CarBrandInfo>();
            rptData.DataSource = carbrandlist;
            rptData.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;

            List<CarInfo> carlist = new List<CarInfo>();
            foreach (CarBrandInfo carbrand in carbrandlist)
            {
                carlist.AddRange(Cars.Instance.GetCarListBycChangs(carbrand.Name, true));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtNameIndex = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtNameIndex");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            CarBrandInfo entity = CarBrands.Instance.GetModel(id, true);
                            entity.NameIndex = txtNameIndex.Value;
                            CarBrands.Instance.UpdateBrand(entity);
                        }
                    }
                }
            }
            CarBrands.Instance.ReloadCarBrandCache();
            CarBrands.Instance.ReloadCarBrandCacheByCorporation();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/car/cxmg.aspx" : FromUrl);
        }
    }
}