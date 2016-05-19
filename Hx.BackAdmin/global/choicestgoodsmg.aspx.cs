using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools;
using Hx.Components.Entity;
using Hx.Car;
using Hx.Car.Entity;

namespace Hx.BackAdmin.global
{
    public partial class choicestgoodsmg : AdminBase
    {
        private List<CorporationInfo> corplist = null;
        private List<CorporationInfo> Corplist
        {
            get
            {
                if (corplist == null)
                    corplist = Corporations.Instance.GetList(true);
                return corplist;
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
            ddlCorporationFilter.DataSource = Corplist;
            ddlCorporationFilter.DataTextField = "Name";
            ddlCorporationFilter.DataValueField = "ID";
            ddlCorporationFilter.DataBind();
            ddlCorporationFilter.Items.Insert(0, new ListItem("系统通用", "0"));
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int pagesize = GetInt("pagesize", search_fy.PageSize);
            int total = 0;

            SetSelectedByValue(ddlCorporationFilter, GetInt("corpid").ToString());

            List<ChoicestgoodsInfo> list = Choicestgoods.Instance.GetList(DataConvert.SafeInt(ddlCorporationFilter.SelectedValue), true);
            list = list.FindAll(l => l.CorporationID == GetInt("corpid"));
            total = list.Count();
            list = list.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<ChoicestgoodsInfo>();

            rptData.DataSource = list;
            rptData.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ChoicestgoodsInfo info = (ChoicestgoodsInfo)e.Item.DataItem;

                System.Web.UI.WebControls.DropDownList ddlCorporation = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCorporation");
                if (ddlCorporation != null)
                {
                    ddlCorporation.DataSource = Corplist;
                    ddlCorporation.DataTextField = "Name";
                    ddlCorporation.DataValueField = "ID";
                    ddlCorporation.DataBind();
                    ddlCorporation.Items.Insert(0, new ListItem("系统通用", "0"));

                    SetSelectedByValue(ddlCorporation, info.CorporationID.ToString());
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("choicestgoodsmg.aspx?corpid=" + ddlCorporationFilter.SelectedValue);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                Choicestgoods.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    string Price = Request["txtPrice" + i];
                    string ProductType = Request["txtProductType" + i];
                    string Inpoint = Request["txtInpoint" + i];
                    string Remark = Request["txtRemark" + i];
                    int CorporationID = DataConvert.SafeInt(Request["ddlCorporation" + i]);
                    if (!string.IsNullOrEmpty(name))
                    {
                        ChoicestgoodsInfo entity = new ChoicestgoodsInfo
                        {
                            CorporationID = CorporationID,
                            Name = name,
                            Price = Price,
                            ProductType = ProductType,
                            Inpoint = Inpoint,
                            Remark = Remark
                        };
                        Choicestgoods.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtName");
                    System.Web.UI.HtmlControls.HtmlInputText txtPrice = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtPrice");
                    System.Web.UI.HtmlControls.HtmlInputText txtProductType = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtProductType");
                    System.Web.UI.HtmlControls.HtmlInputText txtInpoint = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtInpoint");
                    System.Web.UI.HtmlControls.HtmlInputText txtRemark = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtRemark");
                    System.Web.UI.WebControls.DropDownList ddlCorporation = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCorporation");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            ChoicestgoodsInfo entity = new ChoicestgoodsInfo
                            {
                                ID = id,
                                CorporationID = DataConvert.SafeInt(ddlCorporation.SelectedValue),
                                Name = txtName.Value,
                                Price = txtPrice.Value,
                                ProductType = txtProductType.Value,
                                Inpoint = txtInpoint.Value,
                                Remark = txtRemark.Value,
                            };
                            Choicestgoods.Instance.Update(entity);
                        }
                    }
                }
            }
            Choicestgoods.Instance.ReloadChoicestgoodsListCache();


            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? ("~/global/choicestgoodsmg.aspx?corpid=" + ddlCorporationFilter.SelectedValue)  : FromUrl);
        }
    }
}