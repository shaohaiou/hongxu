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
using Hx.Components.Entity;

namespace Hx.BackAdmin.car
{
    public partial class quotationcheck : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.总经理) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售员) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.财务出纳) == 0)
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
                    {
                        cq = CarQuotations.Instance.GetModel(GetInt("id"));
                        if (!Admin.Administrator && cq.CorporationID.ToString() != Admin.Corporation)
                        {
                            Response.Clear();
                            Response.Write("您无权查看此审核单！");
                            Response.End();
                        }
                    }
                    else
                        cq = MangaCache.GetLocal(GlobalKey.QUOTATION_KEY) as CarQuotationInfo;
                    if (cq == null)
                    {
                        Response.Clear();
                        Response.Write("非法审核单！");
                        Response.End();
                    }
                }
                return cq;
            }
        }

        private class WyfwInfo
        {
            public string Name { get; set; }
            public string Price { get; set; }
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

            List<WyfwInfo> wyfwlist = new List<WyfwInfo>();
            if (CQ.IsWyfwjytc) wyfwlist.Add(new WyfwInfo() { Name = "机油套餐", Price = CQ.Wyfwjytc });
            if (CQ.IsWyfwblwyfw) wyfwlist.Add(new WyfwInfo() { Name = "玻璃无忧服务", Price = CQ.Wyfwblwyfw });
            if (CQ.IsWyfwhhwyfw) wyfwlist.Add(new WyfwInfo() { Name = "划痕无忧服务", Price = CQ.Wyfwhhwyfw });
            if (CQ.IsWyfwybwyfw) wyfwlist.Add(new WyfwInfo() { Name = "延保无忧车服务", Price = CQ.Wyfwybwyfw });
            rptWyfw.DataSource = wyfwlist;
            rptWyfw.DataBind();

            string[] goods = CQ.ChoicestGoods.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<ChoicestgoodsInfo> clist = Choicestgoods.Instance.GetList(true);
            rptQcyp.DataSource = clist.FindAll(c => goods.Contains(c.ID.ToString()));
            rptQcyp.DataBind();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(FromUrl);
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            if (CQ == null)
            {
                Response.Clear();
                Response.Write("非法审核单！");
                Response.End();
            }
            if (!Admin.Administrator && Admin.Corporation != CQ.CorporationID.ToString())
            {
                Response.Clear();
                Response.Write("您无权复核此审核单！");
                Response.End();
            }
            if (!Admin.Administrator && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.财务出纳) == 0)
            {
                Response.Clear();
                Response.Write("您无权复核此审核单！");
                Response.End();
            }
            if (CQ.CheckStatus == 1)
            {
                Response.Clear();
                Response.Write("此审核单已被复核过，复核人：" + CQ.CheckUser + ",复核时间：" + CQ.CheckTime);
                Response.End();
            }

            CQ.CheckStatus = 1;
            CQ.CheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CQ.CheckUser = string.IsNullOrEmpty(Admin.Name) ? AdminName : Admin.Name;

            CarQuotations.Instance.Check(CQ);
        }

        protected void btnJLCheck_Click(object sender, EventArgs e)
        {
            if (CQ == null)
            {
                Response.Clear();
                Response.Write("非法审核单！");
                Response.End();
            }
            if (!Admin.Administrator && Admin.Corporation != CQ.CorporationID.ToString())
            {
                Response.Clear();
                Response.Write("您无权审核此审核单！");
                Response.End();
            }
            if (!Admin.Administrator && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0)
            {
                Response.Clear();
                Response.Write("您无权审核此审核单！");
                Response.End();
            }
            if (CQ.JLCheckStatus == 1)
            {
                Response.Clear();
                Response.Write("此审核单已被审核过，审核人：" + CQ.JLCheckUser + ",审核时间：" + CQ.JLCheckTime);
                Response.End();
            }

            CQ.JLCheckStatus = 1;
            CQ.JLCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CQ.JLCheckUser = string.IsNullOrEmpty(Admin.Name) ? AdminName : Admin.Name;
            CQ.JLCheckRemark = txtJLCheckRemark.Text;

            CarQuotations.Instance.JLCheck(CQ);
        }

        protected void btnZJLCheck_Click(object sender, EventArgs e)
        {
            if (CQ == null)
            {
                Response.Clear();
                Response.Write("非法审核单！");
                Response.End();
            }
            if (!Admin.Administrator && Admin.Corporation != CQ.CorporationID.ToString())
            {
                Response.Clear();
                Response.Write("您无权审核此审核单！");
                Response.End();
            }
            if (!Admin.Administrator && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.总经理) == 0)
            {
                Response.Clear();
                Response.Write("您无权审核此审核单！");
                Response.End();
            }
            if (CQ.ZJLCheckStatus == 1)
            {
                Response.Clear();
                Response.Write("此审核单已被审核过，审核人：" + CQ.ZJLCheckUser + ",审核时间：" + CQ.ZJLCheckTime);
                Response.End();
            }

            CQ.ZJLCheckStatus = 1;
            CQ.ZJLCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CQ.ZJLCheckUser = string.IsNullOrEmpty(Admin.Name) ? AdminName : Admin.Name;
            CQ.ZJLCheckRemark = txtZJLCheckRemark.Text;

            CarQuotations.Instance.ZJLCheck(CQ);
        }
    }
}