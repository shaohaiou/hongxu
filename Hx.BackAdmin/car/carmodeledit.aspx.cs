using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using System.Net;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Text;
using System.Data.SqlClient;
using Hx.Tools;
using Hx.Car.Entity;
using Hx.Car;
using System.Text.RegularExpressions;
using System.Data;

namespace Hx.BackAdmin.car
{
    public partial class carmodeledit : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.车型管理员) == 0)
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
            int id = GetInt("id");
            if (id > 0)
            {
                CarInfo car = Cars.Instance.GetCarInfo(id,true);
                if (car == null)
                {
                    WriteErrorMessage("错误", "错误车辆标识", string.IsNullOrEmpty(FromUrl) ? "~/car/cxmg.aspx" : FromUrl);
                    return;
                }
                txtcChangs.Text = car.cChangs;
                txtcCxmc.Text = car.cCxmc;
                txtfZdj.Text = car.fZdj.ToString();
                hdncQcys.Value = car.cQcys;
                hdnInnerColor.Value = car.cNsys;

                if (!string.IsNullOrEmpty(car.cQcys))
                {
                    DataTable t = new DataTable();
                    t.Columns.Add("Name");
                    t.Columns.Add("Color");

                    foreach (string colorinfo in car.cQcys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] colors = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (colors.Length == 2)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = colors[0];
                            row["Color"] = colors[1];
                            t.Rows.Add(row);
                        }
                    }

                    rptcQcys.DataSource = t;
                    rptcQcys.DataBind();
                }
                else
                {
                    rptcQcys.DataSource = null;
                    rptcQcys.DataBind();
                }
                if (!string.IsNullOrEmpty(car.cNsys))
                {
                    DataTable t = new DataTable();
                    t.Columns.Add("Name");
                    t.Columns.Add("Color");

                    foreach (string colorinfo in car.cNsys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] color = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (color.Length == 2)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = color[0];
                            row["Color"] = color[1].Replace("\"", string.Empty);
                            t.Rows.Add(row);
                        }
                        else if (color.Length == 3)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = color[0];
                            row["Color"] = color[1].Replace("\"", string.Empty) + "," + color[2].Replace("\"", string.Empty);
                            t.Rows.Add(row);
                        }
                    }

                    rptcNsys.DataSource = t;
                    rptcNsys.DataBind();
                }
                else
                {
                    rptcNsys.DataSource = null;
                    rptcNsys.DataBind();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int id = GetInt("id");
            if (id > 0)
            {
                bool haschangebrand = false;
                CarInfo car = Cars.Instance.GetCarInfo(id);

                haschangebrand = car.cChangs != txtcChangs.Text;
                car.cChangs = txtcChangs.Text;
                car.cCxmc = txtcCxmc.Text;
                car.fZdj = DataConvert.SafeDecimal(txtfZdj.Text);
                car.cQcys = hdncQcys.Value;
                car.cNsys = hdnInnerColor.Value;

                Cars.Instance.Update(car, haschangebrand);

                WriteSuccessMessage("保存成功", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/car/cxmg.aspx" : FromUrl);
            }
        }

        protected string GetInnerColor(string colors)
        {
            string result = string.Empty;

            if (colors.IndexOf(",") > 0)
            {
                string[] colorlist = colors.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                result += "<span>";
                result += "<em class=\"em-top\" style=\"background-color:" + colorlist[0] + ";\"></em>";
                result += "<em class=\"em-bottom\" style=\"background-color:" + colorlist[1] + ";\"></em>";
                result += "<em class=\"em-bg\"></em></span>";
            }
            else
                result = "<span><em style=\"background-color:" + colors + ";\"></em><em class=\"em-bg\"></em></span>";
            return result;
        }
    }
}