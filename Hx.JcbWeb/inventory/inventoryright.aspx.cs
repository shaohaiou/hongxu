using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car;

namespace Hx.JcbWeb.inventory
{
    public partial class inventoryright : JcbBase
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
            rptData.DataSource = JcbCars.Instance.GetList(true);
            rptData.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        { 
            
        }
    }
}