using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.weixin
{
    public partial class scenecodevisit : AdminBase
    {
        protected override void Check()
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int id = GetInt("id");
            int sid = GetInt("sid");
            ScenecodeInfo info = null;
            if (id > 0 && sid > 0)
            {
                info = WeixinActs.Instance.GetScenecodeInfo(sid, id, true);
                if (info != null)
                {
                    WeixinActs.Instance.AddScenecodeNum(id);
                    WeixinActs.Instance.ReloadScenecodeListCache(sid);
                }
            }
            if (info == null)
            {
                Response.Write("非法访问");
                Response.End();
            }
            else
                Response.Redirect(info.RedirectAddress);
        }
    }
}