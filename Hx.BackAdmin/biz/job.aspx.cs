using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.biz
{
    public partial class job : System.Web.UI.Page
    {

        private JobOfferInfo _currentJobOffer;
        protected JobOfferInfo CurrentJobOffer
        {
            get
            {
                if (_currentJobOffer == null)
                {
                    List<JobOfferInfo> jobofferlist = JobOffers.Instance.GetList(true);
                    if (jobofferlist.Count > 0)
                        _currentJobOffer = jobofferlist.First();
                }

                return _currentJobOffer;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}