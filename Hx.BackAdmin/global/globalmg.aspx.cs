using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.global
{
    public partial class globalmg : AdminBase
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
            txtProfitMargin.Text = HXContext.Current.GlobalSetting.BankProfitMargin;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            GlobalSettingInfo setting = GlobalSettings.Instance.GetModel(true);

            setting.BankProfitMargin = txtProfitMargin.Text;

            GlobalSettings.Instance.Add(setting);

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/global/globalmg.aspx" : FromUrl);
        }
    }
}