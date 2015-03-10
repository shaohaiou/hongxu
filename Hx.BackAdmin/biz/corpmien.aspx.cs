using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Components.Web;

namespace Hx.BackAdmin.biz
{
    public partial class corpmien : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.人事专员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        public int RecordCount { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (GetString("action") == "del")
                {
                    CorpMiens.Instance.Delete(GetString("id"));
                    ResponseRedirect(FromUrl);
                }
                else if (GetString("action") == "move")
                {
                    int toindex = GetInt("toindex");
                    int id = GetInt("id");
                    CorpMiens.Instance.Move(id, toindex);

                    ResponseRedirect(FromUrl);
                }
                else
                {
                    int pageindex = GetInt("page", 1);
                    if (pageindex < 1)
                    {
                        pageindex = 1;
                    }
                    int pagesize = GetInt("pagesize", search_fy.PageSize);
                    int total = 0;
                    List<CorpMienInfo> list = CorpMiens.Instance.GetList();

                    RecordCount = list.Count();

                    total = list.Count();
                    list = list.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<CorpMienInfo>();
                    rptData.DataSource = list;
                    rptData.DataBind();
                    search_fy.RecordCount = total;
                    search_fy.PageSize = pagesize;
                }
            }
        }
    }
}