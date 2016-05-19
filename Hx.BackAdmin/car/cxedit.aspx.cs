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
    public partial class cxedit : AdminBase
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

        private CarBrandInfo _currentcarbrand = null;
        private CarBrandInfo CurrentCarbrand
        {
            get
            {
                int id = GetInt("id");
                if (_currentcarbrand == null && id > 0)
                {
                    _currentcarbrand = CarBrands.Instance.GetModel(id, true);
                }
                return _currentcarbrand;
            }
        }

        private string CurrentAutomotivetype = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CurrentCarbrand == null)
                {
                    WriteErrorMessage("错误提示", "非法ID", "~/car/cxmg.aspx");
                    return;
                }

                BindControler();
                LoadData();
            }
        }

        private void BindControler()
        {
            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(CurrentCarbrand.Name, true);

            rptAutomotivetype.DataSource = carlist;
            rptAutomotivetype.DataBind();
        }

        private void LoadData()
        {
            lblCarbrand.Text = CurrentCarbrand.Name;
        }

        protected string GetNewAutomotivetypeStr(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(CurrentAutomotivetype))
            {
                CurrentAutomotivetype = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                result += string.Format("<ul><li class=\"nh\"><span class=\"stext\">{0}</span><input type=\"text\" value=\"{0}\" class=\"hide sname\"><a href=\"javascript:void(0);\" class=\"ssave hide\"></a></li>", CurrentAutomotivetype);
            }
            else if (!CurrentAutomotivetype.Equals(name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], StringComparison.OrdinalIgnoreCase))
            {
                CurrentAutomotivetype = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                result += string.Format("</ul><ul><li class=\"nh\"><span class=\"stext\">{0}</span><input type=\"text\" value=\"{0}\" class=\"hide sname\"><a href=\"javascript:void(0);\" class=\"ssave hide\"></a></li>", CurrentAutomotivetype);
            }


            return result;
        }
    }
}