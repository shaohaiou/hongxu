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

namespace Hx.BackAdmin.global
{
    public partial class choicestgoodsmg : AdminBase
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
            rptData.DataSource = Choicestgoods.Instance.GetList();
            rptData.DataBind();
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
                    if (!string.IsNullOrEmpty(name))
                    {
                        ChoicestgoodsInfo entity = new ChoicestgoodsInfo
                        {
                            Name = name,
                            Price = Price,
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
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            ChoicestgoodsInfo entity = new ChoicestgoodsInfo
                            {
                                ID = id,
                                Name = txtName.Value,
                                Price = txtPrice.Value,
                            };
                            Choicestgoods.Instance.Update(entity);
                        }
                    }
                }
            }
            Choicestgoods.Instance.ReloadChoicestgoodsListCache();


            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/global/choicestgoodsmg.aspx" : FromUrl);
        }
    }
}