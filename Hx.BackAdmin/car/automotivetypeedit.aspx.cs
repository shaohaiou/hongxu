using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Tools;

namespace Hx.BackAdmin.car
{
    public partial class automotivetypeedit : AdminBase
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

        private string CurrentAutomotivetype = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CurrentCarbrand == null)
                {
                    WriteErrorMessage("错误提示", "非法ID", "~/car/automotivetypemg.aspx");
                    return;
                }
                if (Corp == null)
                {
                    WriteErrorMessage("错误提示", "您不属于任何分公司，不能进行该项设置", "~/car/automotivetypemg.aspx");
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

            string automotivetypes = string.Empty;

            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(CurrentCarbrand.Name, true);

            if (!string.IsNullOrEmpty(Corp.AutomotivetypeSetting))
            {
                string[] settings = Corp.AutomotivetypeSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string settingstr in settings)
                {
                    string[] setting = settingstr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (setting.Length == 2)
                    {
                        string _carbrand = setting[0];
                        string _automotivetypes = setting[1];
                        if (_carbrand.Equals(CurrentCarbrand.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            carlist = carlist.FindAll(c => _automotivetypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(c.id.ToString()));
                            break;
                        }
                    }
                }
            }

            automotivetypes = string.Join(",", carlist.Select(c => c.id).ToArray());

            hdnAutomotivetype.Value = automotivetypes;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CorporationInfo corporation = Corporations.Instance.GetModel(DataConvert.SafeInt(Admin.Corporation), true);

            if (string.IsNullOrEmpty(corporation.AutomotivetypeSetting))
                corporation.AutomotivetypeSetting = string.Format("{0}:{1}", CurrentCarbrand.Name, hdnAutomotivetype.Value);
            else
            {
                string automotivetypesettingstr = string.Empty;
                bool hassetsetting = false;

                string[] settings = corporation.AutomotivetypeSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string settingstr in settings)
                {
                    string[] setting = settingstr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (setting.Length == 2)
                    {
                        string _carbrand = setting[0];
                        string _automotivetypes = setting[1];
                        if (_carbrand.Equals(CurrentCarbrand.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            automotivetypesettingstr += (string.IsNullOrEmpty(automotivetypesettingstr) ? string.Empty : "|") + string.Format("{0}:{1}", _carbrand, hdnAutomotivetype.Value);
                            hassetsetting = true;
                        }
                        else
                            automotivetypesettingstr += (string.IsNullOrEmpty(automotivetypesettingstr) ? string.Empty : "|") + settingstr;
                    }
                }
                if (!hassetsetting)
                    automotivetypesettingstr += (string.IsNullOrEmpty(automotivetypesettingstr) ? string.Empty : "|") + string.Format("{0}:{1}", CurrentCarbrand.Name, hdnAutomotivetype.Value);

                corporation.AutomotivetypeSetting = automotivetypesettingstr;
            }

            
            Corporations.Instance.Update(corporation);
            Corporations.Instance.ReloadCorporationListCache();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/global/automotivetypemg.aspx" : FromUrl);


        }

        protected string SetAutomotive(string id)
        {
            string result = "";
            bool check = true;

            if (!string.IsNullOrEmpty(Corp.AutomotivetypeSetting))
            {
                string[] settings = Corp.AutomotivetypeSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string settingstr in settings)
                {
                    string[] setting = settingstr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (setting.Length == 2)
                    {
                        string _carbrand = setting[0];
                        string _automotivetypes = setting[1];
                        if (_carbrand.Equals(CurrentCarbrand.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            check = _automotivetypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(id);
                        }
                    }
                }
            }

            if (check)
                result = "checked=\"checked\"";
            return result;
        }

        protected string GetNewAutomotivetypeStr(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(CurrentAutomotivetype))
            {
                CurrentAutomotivetype = name.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries)[0];
                result += string.Format("<ul><li class=\"nh\"><input type=\"checkbox\" class=\"cbxSuball\" checked=\"checked\" />{0}</li>", CurrentAutomotivetype);
            }
            else if (!CurrentAutomotivetype.Equals(name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], StringComparison.OrdinalIgnoreCase))
            {
                CurrentAutomotivetype = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                result += string.Format("</ul><ul><li class=\"nh\"><input type=\"checkbox\" class=\"cbxSuball\" checked=\"checked\" />{0}</li>", CurrentAutomotivetype);
            }


            return result;
        }
    }
}