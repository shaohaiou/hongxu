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

namespace Hx.BackAdmin.car
{
    public partial class automotivetypemg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private CorporationInfo corp = null;
        private CorporationInfo Corp
        {
            get
            {
                if (corp == null)
                    corp = Corporations.Instance.GetModel(DataConvert.SafeInt(Admin.Corporation), true);
                return corp;
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

            List<CarBrandInfo> carbrandlist = CarBrands.Instance.GetCarBrandListByCorporation(Admin.Corporation);

            int total = carbrandlist.Count();
            carbrandlist = carbrandlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<CarBrandInfo>();
            rptCarbrand.DataSource = carbrandlist;
            rptCarbrand.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;

            List<CarInfo> carlist = new List<CarInfo>();
            foreach (CarBrandInfo carbrand in carbrandlist)
            {
                carlist.AddRange(Cars.Instance.GetCarListBycChangs(carbrand.Name, true));
            }

            rptAutomotivetype.DataSource = carlist;
            rptAutomotivetype.DataBind();
        }

        protected string GetAutomotivetype(string carbrand)
        {
            string result = string.Empty;

            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(carbrand, true);

            if (Corp != null && !string.IsNullOrEmpty(Corp.AutomotivetypeSetting))
            {
                string[] settings = Corp.AutomotivetypeSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string settingstr in settings)
                {
                    string[] setting = settingstr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (setting.Length == 2)
                    {
                        string _carbrand = setting[0];
                        string _automotivetypes = setting[1];
                        if (_carbrand.Equals(carbrand, StringComparison.OrdinalIgnoreCase))
                        {
                            carlist = carlist.FindAll(c => _automotivetypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(c.id.ToString()));
                            break;
                        }
                    }
                }
            }

            result = string.Join("|", carlist.Select(c => c.id).ToArray());
            return result;
        }
    }
}