using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Components.BasePage;

namespace Hx.BackAdmin.biz
{
    public partial class corpmiendetail : AdminBase
    {
        protected override void Check()
        {

        }
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
            List<CorpMienInfo> list = CorpMiens.Instance.GetList(true).Take(10).ToList();
            rptData.DataSource = list;
            rptData.DataBind();
            RecordCount = list.Count;
        }
    }
}