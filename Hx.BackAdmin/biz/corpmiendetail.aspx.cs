using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components;
using Hx.Components.Entity;

namespace Hx.BackAdmin.biz
{
    public partial class corpmiendetail : System.Web.UI.Page
    {
        protected int RecordCount = 0;

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
            RecordCount = list.Count;
        }
    }
}