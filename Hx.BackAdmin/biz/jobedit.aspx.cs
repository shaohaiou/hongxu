using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Entity;
using Hx.Components.Web;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools;

namespace Hx.BackAdmin.biz
{
    public partial class jobedit : AdminBase
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

        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.人事专员)  == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            if (CurrentJobOffer != null)
            {
                txtContent.Text = CurrentJobOffer.Content;
                if (!string.IsNullOrEmpty(CurrentJobOffer.PicPath))
                {
                    imgpic.Src = CurrentJobOffer.PicPath;
                    hdimage_pic.Value = CurrentJobOffer.PicPath;
                }
                hdnID.Value = CurrentJobOffer.ID.ToString();
            }
        }

        private void FillData(JobOfferInfo entity)
        {
            entity.PicPath = hdimage_pic.Value;
            entity.Content = txtContent.Text;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int id = DataConvert.SafeInt(hdnID.Value);

            if (id > 0)
            {
                FillData(CurrentJobOffer);
                JobOffers.Instance.Update(CurrentJobOffer);
            }
            else
            {
                JobOfferInfo entity = new JobOfferInfo();
                FillData(entity);
                JobOffers.Instance.Add(entity);
            }

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/biz/jobedit.aspx" : FromUrl);
        }
    }
}