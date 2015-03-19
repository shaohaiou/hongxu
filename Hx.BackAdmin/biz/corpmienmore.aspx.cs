using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.biz
{
    public partial class corpmienmore : System.Web.UI.Page
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
            List<CorpMienInfo> list = CorpMiens.Instance.GetList(true);
            rptData.DataSource = list;
            rptData.DataBind();
        }
    }
}