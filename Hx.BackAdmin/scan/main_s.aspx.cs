using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components.Web;
using Hx.Components.Enumerations;
using Hx.Components;

namespace Hx.BackAdmin.scan
{
    public partial class main_s : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!CheckPower(UserRoleType.文件扫描))
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        protected List<ScanTypeInfo> ScanTypeList
        {
            get
            {
                List<ScanTypeInfo> list = ScanTypes.Instance.GetList(true);
                if (!Admin.Administrator)
                {
                    list = list.FindAll(l => l.CorpPower.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(Admin.Corporation));
                }
                return list;
            }
        }

        protected ScanTypeInfo FirstScanType
        {
            get
            {
                return ScanTypeList.OrderBy(s => s.ID).First();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            rptScanType.DataSource = ScanTypeList;
            rptScanType.DataBind();
        }
    }
}