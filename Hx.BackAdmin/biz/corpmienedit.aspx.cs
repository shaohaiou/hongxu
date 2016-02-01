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
    public partial class corpmienedit : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.人事专员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private CorpMienInfo corpmien = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id = GetInt("id");
                if (id > 0)
                {
                    corpmien = CorpMiens.Instance.GetCorpMien(id);
                    if (corpmien != null)
                    {
                        LoadData(corpmien);
                    }
                    else
                    {
                        WriteErrorMessage("操作出错！", "该记录不存在，可能已经被删除！", "~/biz/corpmien.aspx");
                    }
                }
            }
        }

        private void LoadData(CorpMienInfo corpmien)
        {
            hdnID.Value = corpmien.ID.ToString();
            if (!string.IsNullOrEmpty(corpmien.Pic))
            {
                imgpic.Src = ImgServer + corpmien.Pic;
                hdimage_pic.Value = corpmien.Pic;
            }
            txtIntroduce.Text = corpmien.Introduce;
            txtContent.Text = corpmien.Content;
        }

        private void FillData(CorpMienInfo corpmien)
        {
            corpmien.Pic = hdimage_pic.Value;
            corpmien.Introduce = txtIntroduce.Text;
            corpmien.Content = txtContent.Text;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            int id = DataConvert.SafeInt(hdnID.Value);

            if (id > 0)
            {
                corpmien = CorpMiens.Instance.GetCorpMien(id);
                FillData(corpmien);
                CorpMiens.Instance.Update(corpmien);
            }
            else
            {
                corpmien = new CorpMienInfo();
                FillData(corpmien);
                CorpMiens.Instance.Add(corpmien);
            }

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/biz/corpmienedit.aspx" : FromUrl);
        }
    }
}