using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Tools;
using System.Text;

namespace Hx.BackAdmin.weixin
{
    public partial class benzvotepothunteredit : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.微信活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private BenzvotePothunterInfo pothunter;

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
                Header.Title = "编辑选手信息";
                pothunter = WeixinActs.Instance.GetBenzvotePothunterInfo(id);
                if (pothunter != null)
                {
                    BindData(pothunter);
                }
                else
                {
                    WriteErrorMessage("操作出错！", "该选手不存在，可能已经被删除！", FromUrl);
                }
            }
            else
            {
                Header.Title = "添加选手信息";
            }
        }

        /// <summary>
        /// 绑定页面数据
        /// </summary>
        /// <param name="item"></param>
        protected void BindData(BenzvotePothunterInfo pothunter)
        {
            hdid.Value = pothunter.ID.ToString();
            txtName.Text = pothunter.Name;
            txtSerialNumber.Text = pothunter.SerialNumber.ToString();
            txtBallot.Text = pothunter.Ballot.ToString();
            txtIntroduce.Text = pothunter.Introduce;
            txtDeclare.Text = pothunter.Declare;
            imgpic.Src = pothunter.PicPath;
            hdimage_pic.Value = pothunter.PicPath;
            imgpics1.Src = pothunter.IntroducePic1;
            imgpics2.Src = pothunter.IntroducePic2;
            imgpics3.Src = pothunter.IntroducePic3;
            hdimage_pics.Value = string.Join("|",new string[] { pothunter.IntroducePic1, pothunter.IntroducePic2, pothunter.IntroducePic3 });
        }

        private void FillData(BenzvotePothunterInfo pothunter)
        {
            pothunter.Name = txtName.Text;
            pothunter.SerialNumber = DataConvert.SafeInt(txtSerialNumber.Text);
            pothunter.Ballot = DataConvert.SafeInt(txtBallot.Text);
            pothunter.Introduce = txtIntroduce.Text;
            pothunter.Declare = txtDeclare.Text;
            pothunter.PicPath = hdimage_pic.Value;
            pothunter.IntroducePic1 = hdimage_pics.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0];
            pothunter.IntroducePic2 = hdimage_pics.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[1];
            pothunter.IntroducePic3 = hdimage_pics.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[2];
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            string checkresult = CheckForm();
            if (!string.IsNullOrEmpty(checkresult))
            {
                lbMsg.Text = checkresult;
                lbMsg.Visible = true;
                return;
            }

            int id;
            //是否通过页面验证
            if (Page.IsValid)
            {
                id = DataConvert.SafeInt(hdid.Value);

                if (id > 0)
                {
                    pothunter = WeixinActs.Instance.GetBenzvotePothunterInfo(id);
                    if (pothunter == null)
                    {
                        WriteMessage("~/message/showmessage.aspx", "操作出错！", "该选手不存在，可能已经被删除！", "", FromUrl);
                    }
                    else
                    {
                        FillData(pothunter);
                        WeixinActs.Instance.AddBenzvotePothunterInfo(pothunter);
                    }
                }
                else
                {
                    pothunter = new BenzvotePothunterInfo();
                    FillData(pothunter);
                    WeixinActs.Instance.AddBenzvotePothunterInfo(pothunter);
                }

                WeixinActs.Instance.ReloadBenzvotePothunterListCache();
                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? UrlDecode(CurrentUrl) : FromUrl);
            }
        }

        private string CheckForm()
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(txtName.Text.Trim())) result = "姓名必须填写";
            if (string.IsNullOrEmpty(txtSerialNumber.Text.Trim())) result = "序号必须填写";
            if (string.IsNullOrEmpty(txtIntroduce.Text.Trim())) result = "个人介绍必须填写";
            if (string.IsNullOrEmpty(txtDeclare.Text.Trim())) result = "参赛宣言必须填写";
            if (string.IsNullOrEmpty(hdimage_pic.Value.Trim())) result = "请上传头像";
            if (string.IsNullOrEmpty(hdimage_pics.Value.Trim())) result = "请上传风采展示";

            return result;
        }
    }
}