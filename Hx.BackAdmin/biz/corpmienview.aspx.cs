using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools.Web;
using Hx.Components.BasePage;

namespace Hx.BackAdmin.biz
{
    public partial class corpmienview : AdminBase
    {
        protected override void Check()
        {

        }
        private CorpMienInfo _currentCorpMien;
        protected CorpMienInfo CurrentCorpMien
        {
            get
            {
                if (_currentCorpMien == null && WebHelper.GetInt("id") > 0)
                {
                    _currentCorpMien = CorpMiens.Instance.GetCorpMien(WebHelper.GetInt("id"), true);
                }

                return _currentCorpMien;
            }
        }

        protected string NextUrl
        {
            get
            {
                string url = "javascript:void(0)";

                if (CurrentCorpMien != null)
                {
                    List<CorpMienInfo> list = CorpMiens.Instance.GetList(true);
                    if (list.Exists(l => l.ID == CurrentCorpMien.ID))
                    {
                        int nextindex = 0;
                        int currentindex = list.FindIndex(l => l.ID == CurrentCorpMien.ID);
                        if (currentindex == list.Count - 1)
                        {
                            nextindex = 0;
                        }
                        else
                            nextindex = currentindex + 1;
                        url = "?id=" + list[nextindex].ID;
                    }
                }
                return url;
            }
        }

        protected string PrevUrl
        {
            get
            {
                string url = "javascript:void(0)";

                if (CurrentCorpMien != null)
                {
                    List<CorpMienInfo> list = CorpMiens.Instance.GetList(true);
                    if (list.Exists(l => l.ID == CurrentCorpMien.ID))
                    {
                        int previndex = 0;
                        int currentindex = list.FindIndex(l => l.ID == CurrentCorpMien.ID);
                        if (currentindex == 0)
                        {
                            previndex = list.Count - 1;
                        }
                        else
                            previndex = currentindex - 1;
                        url = "?id=" + list[previndex].ID;
                    }
                }
                return url;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}