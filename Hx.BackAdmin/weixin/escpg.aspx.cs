using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car;
using Hx.Car.Entity;
using System.Text.RegularExpressions;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.weixin
{
    public partial class escpg : WeixinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
            }
            else
            {
                if (hdnIspostdata.Value == "1")
                {
                    PostForm();
                }
            }
        }

        private void BindControler()
        {
            ddlBrand.DataSource = CarBrands.Instance.GetCarBrandList(true);
            ddlBrand.DataTextField = "BindName";
            ddlBrand.DataValueField = "Name";
            ddlBrand.DataBind();
            ddlBrand.Items.Insert(0, new ListItem("请选择......", "-1"));

            ddlChexi.Items.Insert(0, new ListItem("请选择......", "-1"));
            ddlNianfen.Items.Insert(0, new ListItem("请选择......", "-1"));
            ddlKuanshi.Items.Insert(0, new ListItem("请选择......", "-1"));
        }

        protected void ddlBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlChexi.Items.Clear();
            ddlChexi.Items.Insert(0, new ListItem("请选择......", "-1"));
            ddlNianfen.Items.Clear();
            ddlNianfen.Items.Insert(0, new ListItem("请选择......", "-1"));
            ddlKuanshi.Items.Clear();
            ddlKuanshi.Items.Insert(0, new ListItem("请选择......", "-1"));
            if (ddlBrand.SelectedIndex == 0)
                return;

            Regex r = new Regex(@"([\s\S]+?)(\d+?)款([\s\S]*)");
            string brand = ddlBrand.SelectedValue;
            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(brand, true);
            ddlChexi.DataSource = carlist.FindAll(c => r.IsMatch(c.cCxmc)).Select(c => new { Name = r.Match(c.cCxmc).Groups[1].Value }).Distinct().ToList();
            ddlChexi.DataTextField = "Name";
            ddlChexi.DataValueField = "Name";
            ddlChexi.DataBind();
            ddlChexi.Items.Insert(0, new ListItem("请选择......", "-1"));

        }

        protected void ddlChexi_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlNianfen.Items.Clear();
            ddlNianfen.Items.Insert(0, new ListItem("请选择......", "-1"));
            ddlKuanshi.Items.Clear();
            ddlKuanshi.Items.Insert(0, new ListItem("请选择......", "-1"));
            if (ddlChexi.SelectedIndex == 0)
                return;

            Regex r = new Regex(ddlChexi.SelectedValue + @"(\d+?)款([\s\S]*)");
            string brand = ddlBrand.SelectedValue;
            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(brand, true);
            ddlNianfen.DataSource = carlist.FindAll(c => r.IsMatch(c.cCxmc)).Select(c => new { Name = r.Match(c.cCxmc).Groups[1].Value }).Distinct().ToList();
            ddlNianfen.DataTextField = "Name";
            ddlNianfen.DataValueField = "Name";
            ddlNianfen.DataBind();
            ddlNianfen.Items.Insert(0, new ListItem("请选择......", "-1"));
        }

        protected void ddlNianfen_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlKuanshi.Items.Clear();
            ddlKuanshi.Items.Insert(0, new ListItem("请选择......", "-1"));
            if (ddlNianfen.SelectedIndex == 0)
                return;

            Regex r = new Regex(ddlChexi.SelectedValue + ddlNianfen.SelectedValue + @"款([\s\S]*)");
            string brand = ddlBrand.SelectedValue;
            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(brand, true);
            ddlKuanshi.DataSource = carlist.FindAll(c => r.IsMatch(c.cCxmc)).Select(c => new { Name = r.Match(c.cCxmc).Groups[1].Value.Trim() }).Distinct().ToList();
            ddlKuanshi.DataTextField = "Name";
            ddlKuanshi.DataValueField = "Name";
            ddlKuanshi.DataBind();
            ddlKuanshi.Items.Insert(0, new ListItem("请选择......", "-1"));
        }

        private void PostForm()
        {
            EscpgInfo entity = new EscpgInfo() 
            {
                Brand = ddlBrand.SelectedValue,
                Chexi = ddlChexi.SelectedValue,
                Nianfen = ddlNianfen.SelectedValue,
                Kuanshi = ddlKuanshi.SelectedIndex > 0 ? ddlKuanshi.SelectedValue : string.Empty,
                Licheng = txtLicheng.Value,
                Phone = txtPhone.Value,
                AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Restore = false
            };

            WeixinActs.Instance.AddEscpgInfo(entity);

            Response.Redirect("escpgsuc.aspx");
        }

        /// <summary>
        /// 登录页面不需要验证
        /// </summary>
        protected override void Check()
        {

        }
    }
}