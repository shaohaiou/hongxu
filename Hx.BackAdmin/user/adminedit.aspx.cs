using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components;
using System.Text;
using Hx.Car;
using Hx.Tools;
using Hx.Car.Entity;

namespace Hx.BackAdmin.user
{
    public partial class adminedit : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
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
            txtMobile.Text = Admin.Mobile;
            txtName.Text = Admin.Name;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            AdminInfo entity = HXContext.Current.AdminUser;
            if (entity != null)
            {
                entity.Mobile = txtMobile.Text;
                entity.Name = txtName.Text;

                Admins.Instance.UpdateAdmin(entity);

                Admin = entity;

                StringBuilder sb = new StringBuilder();
                sb.Append("<span class=\"dalv\">信息保存成功！</span><br />");
                WriteMessage("~/message/showmessage.aspx", "保存成功！", sb.ToString(), "", string.IsNullOrEmpty(FromUrl) ? "~/user/adminedit.aspx" : FromUrl);
            }
        }
    }

}