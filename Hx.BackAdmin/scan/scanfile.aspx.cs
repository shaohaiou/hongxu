using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Enumerations;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.scan
{
    public partial class scanfile : AdminBase
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
            if (CurrentScanType != null)
            {
                if (!Admin.Administrator)
                {
                    if (!CurrentScanType.CorpPower.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(Admin.Corporation))
                    {
                        Response.Clear();
                        Response.Write("您没有权限操作！");
                        Response.End();
                        return;
                    }
                }
            }
            else
            {
                WriteErrorMessage("错误提示", "非法ID", string.IsNullOrEmpty(FromUrl) ? "~/scan/scantypemg.aspx" : FromUrl);
            }
        }

        private ScanTypeInfo currentscantype = null;
        private ScanTypeInfo CurrentScanType
        {
            get
            {
                if (currentscantype == null)
                {
                    currentscantype = ScanTypes.Instance.GetModel(GetInt("tid"), true);
                }

                return currentscantype;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}