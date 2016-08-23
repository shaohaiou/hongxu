using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using System.Data;
using Hx.Components.Enumerations;
using Hx.Tools;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Web;
using Hx.Car;

namespace Hx.BackAdmin.scan
{
    public partial class scantypesetting : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private ScanTypeInfo currentscantype = null;
        private ScanTypeInfo CurrentScanType
        {
            get
            {
                if (currentscantype == null)
                {
                    currentscantype = ScanTypes.Instance.GetModel(GetInt("id"), true);
                }

                return currentscantype;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
                LoadData();
            }
        }

        private void BindControler()
        {
            rptCorpPower.DataSource = Corporations.Instance.GetList(true);
            rptCorpPower.DataBind();
        }

        private void LoadData()
        { 
            if(CurrentScanType != null)
            {
                txtName.Text = CurrentScanType.Name;
                txtValidAreaXTop.Text = CurrentScanType.ValidAreaXTop.ToString();
                txtValidAreaYTop.Text = CurrentScanType.ValidAreaYTop.ToString();
                txtValidAreaXBottom.Text = CurrentScanType.ValidAreaXBottom.ToString();
                txtValidAreaYBottom.Text = CurrentScanType.ValidAreaYBottom.ToString();
            }
            else
            {
                WriteErrorMessage("错误提示", "非法ID", string.IsNullOrEmpty(FromUrl) ? "~/scan/scantypemg.aspx" : FromUrl);
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ScanTypeInfo entity = CurrentScanType;
            entity.Name = txtName.Text;
            entity.ValidAreaXTop = DataConvert.SafeInt(txtValidAreaXTop.Text);
            entity.ValidAreaYTop = DataConvert.SafeInt(txtValidAreaYTop.Text);
            entity.ValidAreaXBottom = DataConvert.SafeInt(txtValidAreaXTop.Text);
            entity.ValidAreaYBottom = DataConvert.SafeInt(txtValidAreaYBottom.Text);
            entity.CorpPower = hdnCorpPower.Value;

            ScanTypes.Instance.Update(entity);

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/scan/scantypemg.aspx" : FromUrl);
        }

        protected string SetCorpPower(string v)
        {
            string result = string.Empty;

            if (CurrentScanType != null)
            {
                string[] deps = CurrentScanType.CorpPower.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (deps.Contains(v))
                    result = "checked=\"checked\"";
            }

            return result;
        }
    }
}