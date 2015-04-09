using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Tools;
using Hx.Components;
using Hx.Components.Enumerations;
using Hx.Tools.Web;
using Hx.Components.BasePage;

namespace Hx.JcbWeb
{
    public partial class reg : JcbBase
    {
        protected override void Check()
        {
            return;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
            }
            else
            {
                Submit();
            }
        }

        private void BindControler()
        {
            rblCompanyType.DataSource = EnumExtensions.ToTable<JcbCompanyType>();
            rblCompanyType.DataTextField = "Text";
            rblCompanyType.DataValueField = "Value";
            rblCompanyType.DataBind();

            ddlProvince.DataSource = Areas.Instance.GetPromaryList(true);
            ddlProvince.DataTextField = "Name";
            ddlProvince.DataValueField = "ID";
            ddlProvince.DataBind();
            ddlProvince.Items.Insert(0, new ListItem("-省份-","-1"));

            ddlCity.Visible = false;
        }

        protected void ddlProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProvince.SelectedIndex == 0)
            {
                ddlCity.Items.Clear();
                ddlCity.Visible = false;
            }
            else
            {
                ddlCity.DataSource = Areas.Instance.GetCityList(true).FindAll(c => c.PID.ToString() == ddlProvince.SelectedValue).ToList();
                ddlCity.DataTextField = "Name";
                ddlCity.DataValueField = "ID";
                ddlCity.DataBind();
                ddlCity.Items.Insert(0, new ListItem("-城市-", "-1"));
                ddlCity.Visible = true;
            }
        }

        private void FillData(Components.Entity.JcbUserInfo user)
        {
            user.UserName = txtUserName.Text;
            user.Password = EncryptString.MD5(txtPassword.Text);
            user.Administrator = false;
            user.LastLoginIP = WebHelper.GetClientsIP();
            user.LastLoginTime = DateTime.Now;
            user.Name = txtName.Text;
            user.CompanyType = (JcbCompanyType)DataConvert.SafeInt(rblCompanyType.SelectedValue);
            user.CompanyName = txtCompanyName.Text;
            user.Phone = txtPhone.Text;
            user.QQ = txtQQ.Text;
            user.Province = ddlProvince.SelectedItem.Text;
            user.City = ddlCity.SelectedItem.Text;
            user.Address = txtAddress.Text;
        }

        private void Submit()
        {
            string checkresult = CheckForm();

            if (!string.IsNullOrEmpty(checkresult))
            {
                lblMsg.Text = checkresult;
                return;
            }

            Components.Entity.JcbUserInfo user = new Components.Entity.JcbUserInfo();
            FillData(user);
            if (JcbUsers.Instance.AddUser(user) > 0)
                WriteSuccessMessage("注册完成", "注册成功！", "~/index.aspx");
            else
                WriteSuccessMessage("提交失败", "注册失败，请重新提交信息！", "~/reg.aspx");
        }

        private string CheckForm()
        {
            if (string.IsNullOrEmpty(txtUserName.Text)) return "请输入登录名";
            if (string.IsNullOrEmpty(txtPassword.Text)) return "请输入登录密码";
            if (string.IsNullOrEmpty(txtPasswordConfirm.Text)) return "请输入确认密码";
            if (txtPasswordConfirm.Text != txtPassword.Text) return "两次密码不一致";
            if (rblCompanyType.SelectedIndex < 0) return "请选择公司类型";
            if (string.IsNullOrEmpty(txtCompanyName.Text)) return "请输入公司名称";
            if (string.IsNullOrEmpty(txtName.Text)) return "请输入联系人";
            if (string.IsNullOrEmpty(txtPhone.Text)) return "请输入手机号";
            if (string.IsNullOrEmpty(ddlProvince.Text)) return "请选择省份";
            if (string.IsNullOrEmpty(ddlCity.Text)) return "请选择城市";
            if (string.IsNullOrEmpty(txtAddress.Text)) return "请输入详细地址";
            return string.Empty;
        }
    }
}