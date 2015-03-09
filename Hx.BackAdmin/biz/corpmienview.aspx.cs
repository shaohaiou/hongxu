using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools.Web;

namespace Hx.BackAdmin.biz
{
    public partial class corpmienview : System.Web.UI.Page
    {

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

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}