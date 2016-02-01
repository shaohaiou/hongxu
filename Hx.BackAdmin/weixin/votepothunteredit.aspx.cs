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
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class votepothunteredit : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator 
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.投票活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator)
            {
                int sid = GetInt("sid");
                VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(sid, true);
                if (!setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(AdminID.ToString()))
                {
                    Response.Clear();
                    Response.Write("您没有权限操作！");
                    Response.End();
                }
            }
        }

        public VotePothunterInfo pothunter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            rpcg.DataSource = WeixinActs.Instance.GetVoteSettingList(true);
            rpcg.DataBind();

            int id = GetInt("id");
            if (id > 0)
            {
                Header.Title = "编辑选手信息";
                pothunter = WeixinActs.Instance.GetVotePothunterInfo(id);
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
        protected void BindData(VotePothunterInfo pothunter)
        {
            hdid.Value = pothunter.ID.ToString();
            txtName.Text = pothunter.Name;
            txtSerialNumber.Text = pothunter.SerialNumber.ToString();
            txtSerialNumberDetail.Text = pothunter.SerialNumberDetail;
            txtBallot.Text = pothunter.Ballot.ToString();
            txtIntroduce.Text = pothunter.Introduce;
            txtDeclare.Text = pothunter.Declare;
            imgpic.Src = pothunter.PicPath;
            hdimage_pic.Value = pothunter.PicPath; if (!string.IsNullOrEmpty(pothunter.IntroducePic1))
            {
                imgpics1.Src = ImgServer + pothunter.IntroducePic1;
                imgpics1.Attributes["val"] = ImgServer + pothunter.IntroducePic1;
            }
            if (!string.IsNullOrEmpty(pothunter.IntroducePic2))
            {
                imgpics2.Src = ImgServer + pothunter.IntroducePic2;
                imgpics2.Attributes["val"] = ImgServer + pothunter.IntroducePic2;
            }
            if (!string.IsNullOrEmpty(pothunter.IntroducePic3))
            {
                imgpics3.Src = pothunter.IntroducePic3;
                imgpics3.Attributes["val"] = ImgServer + pothunter.IntroducePic3;
            }
            if (!string.IsNullOrEmpty(pothunter.IntroducePic4))
            {
                imgpics4.Src = pothunter.IntroducePic4;
                imgpics4.Attributes["val"] = ImgServer + pothunter.IntroducePic4;
            }
            if (!string.IsNullOrEmpty(pothunter.IntroducePic5))
            {
                imgpics5.Src = ImgServer + pothunter.IntroducePic5;
                imgpics5.Attributes["val"] = ImgServer + pothunter.IntroducePic5;
            }
            if (!string.IsNullOrEmpty(pothunter.IntroducePic6))
            {
                imgpics6.Src = ImgServer + pothunter.IntroducePic6;
                imgpics6.Attributes["val"] = ImgServer + pothunter.IntroducePic6;
            }
            hdimage_pics.Value = string.Join("|", new string[] { pothunter.IntroducePic1, pothunter.IntroducePic2, pothunter.IntroducePic3, pothunter.IntroducePic4, pothunter.IntroducePic5, pothunter.IntroducePic6 });
        }

        private void FillData(VotePothunterInfo pothunter)
        {
            pothunter.SID = GetInt("sid");
            pothunter.Name = txtName.Text;
            pothunter.SerialNumber = DataConvert.SafeInt(txtSerialNumber.Text);
            pothunter.SerialNumberDetail = txtSerialNumberDetail.Text;
            pothunter.Ballot = DataConvert.SafeInt(txtBallot.Text);
            pothunter.Introduce = txtIntroduce.Text;
            pothunter.Declare = txtDeclare.Text;
            pothunter.PicPath = hdimage_pic.Value;
            if (!string.IsNullOrEmpty(hdimage_pics.Value))
            {
                pothunter.IntroducePic1 = hdimage_pics.Value.Split(new char[] { '|' })[0];
                pothunter.IntroducePic2 = hdimage_pics.Value.Split(new char[] { '|' })[1];
                pothunter.IntroducePic3 = hdimage_pics.Value.Split(new char[] { '|' })[2];
                pothunter.IntroducePic4 = hdimage_pics.Value.Split(new char[] { '|' })[3];
                pothunter.IntroducePic5 = hdimage_pics.Value.Split(new char[] { '|' })[4];
                pothunter.IntroducePic6 = hdimage_pics.Value.Split(new char[] { '|' })[5];
            }
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
                    pothunter = WeixinActs.Instance.GetVotePothunterInfo(id);
                    if (pothunter == null)
                    {
                        WriteMessage("~/message/showmessage.aspx", "操作出错！", "该选手不存在，可能已经被删除！", "", FromUrl);
                    }
                    else
                    {
                        FillData(pothunter);
                        WeixinActs.Instance.AddVotePothunterInfo(pothunter);
                    }
                }
                else
                {
                    pothunter = new VotePothunterInfo();
                    FillData(pothunter);
                    WeixinActs.Instance.AddVotePothunterInfo(pothunter);
                }

                WeixinActs.Instance.ReloadVotePothunterList(GetInt("sid"));
                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? UrlDecode(CurrentUrl) : FromUrl);
            }
        }

        private string CheckForm()
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(txtName.Text.Trim())) result = "姓名必须填写";

            return result;
        }

        protected string SetVoteSettingStatus(string id)
        {
            string result = string.Empty;

            if (!Admin.Administrator)
            {
                VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(DataConvert.SafeInt(id), true);

                if (setting != null)
                {
                    string[] powerusers = setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!powerusers.Contains(AdminID.ToString()))
                        result = "style=\"display:none;\"";
                }
            }

            return result;
        }
    }
}