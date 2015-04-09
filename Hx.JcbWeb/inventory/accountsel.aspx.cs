using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car.Entity;
using Hx.Car;

namespace Hx.JcbWeb.inventory
{
    public partial class accountsel : JcbBase
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
            string[] accounts = GetString("ids").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<JcbAccountInfo> listAccount = Jcbs.Instance.GetAccountList(true);
            rptData.DataSource = listAccount.FindAll(a => accounts.Contains(a.ID.ToString()));
            rptData.DataBind();
        }
    }
}