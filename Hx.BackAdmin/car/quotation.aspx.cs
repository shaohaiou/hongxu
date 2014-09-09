using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components;
using Hx.Car.Entity;
using Hx.Tools;
using Hx.Car;

namespace Hx.BackAdmin.car
{
    public partial class quotation : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator 
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private CarQuotationInfo cq = null;
        protected CarQuotationInfo CQ
        {
            get
            {
                if (cq == null)
                {
                    if (GetInt("id") > 0)
                        cq = CarQuotations.Instance.GetModel(GetInt("id"));
                    else
                        cq = MangaCache.GetLocal(GlobalKey.QUOTATION_KEY) as CarQuotationInfo;
                    if (cq == null)
                    {
                        Response.Redirect("carquotation.aspx");
                    }
                }
                return cq;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
            }
        }

        protected void BindControler()
        {
            if (string.IsNullOrEmpty(FromUrl))
                btnBack.Visible = false;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.IsNullOrEmpty(FromUrl) ? "carquotation.aspx" : FromUrl);
        }

        /// <summary>
        /// 保险费用
        /// </summary>
        /// <returns></returns>
        protected string GetBxfy()
        {
            string result = string.Empty;

            if (CQ != null)
            {
                double bxfy = DataConvert.SafeDouble(CQ.cCsx)
                    + DataConvert.SafeDouble(CQ.cDszrx)
                    + DataConvert.SafeDouble(CQ.cJqs)
                    + DataConvert.SafeDouble(CQ.cDqx)
                    + DataConvert.SafeDouble(CQ.cBjmp);
                if (CQ.IscZdwx) bxfy += DataConvert.SafeDouble(CQ.cZdwx);
                if (CQ.IscSj) bxfy += DataConvert.SafeDouble(CQ.cSj);
                if (CQ.IscCk) bxfy += DataConvert.SafeDouble(CQ.cCk);
                if (CQ.IscZrx) bxfy += DataConvert.SafeDouble(CQ.cZrx);
                if (CQ.IscBlx) bxfy += DataConvert.SafeDouble(CQ.cBlx);
                //if (CQ.IscHhx) bxfy += DataConvert.SafeInt(CQ.cHhx);
                //if (CQ.IscSsx) bxfy += DataConvert.SafeInt(CQ.cSsx);
                result = bxfy == 0 ? string.Empty : bxfy.ToString();
            }

            return result;
        }

        /// <summary>
        /// 司乘费用
        /// </summary>
        /// <returns></returns>
        protected string GetSc()
        {
            string result = string.Empty;

            if (CQ != null)
            {
                double fy = 0;
                if (CQ.IscSj) fy += DataConvert.SafeDouble(CQ.cSj);
                if (CQ.IscCk) fy += DataConvert.SafeDouble(CQ.cCk);
                result = fy == 0 ? string.Empty : fy.ToString();
            }

            return result;
        }
    }
}