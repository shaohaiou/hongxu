using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Tools;
using Hx.Components.Enumerations;
using Hx.Components.Entity;
using Hx.Components;
using System.Data;
using Hx.Components.BasePage;

namespace Hx.BackAdmin.weixin
{
    public partial class wjdczg : WeixinBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Code { get; set; }
        public string Openid { get; set; }
        private QuestionCompanyInfo currentcompany = null;
        public string appid = "wx0c9b37c9d5ddf8a8";
        public string appsecret = "f6e1f096a7e847e9775b1cc64e713a33";
        protected QuestionCompanyInfo CurrentCompany
        {
            get
            {
                if (!string.IsNullOrEmpty(GetString("cid")))
                {
                    List<QuestionCompanyInfo> list = WeixinActs.Instance.GetQuestionCompanyList();
                    currentcompany = list.Find(l => l.ID == DataConvert.SafeInt(GetString("cid")));
                }

                return currentcompany;
            }
        }
        private string signature = string.Empty;
        public string CurrentSignature
        {
            get
            {
                if (string.IsNullOrEmpty(signature))
                    signature = Hx.Tools.EncryptString.SHA1_Hash(string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", WeixinActs.Instance.GetJsapiTicket(appid, appsecret), NonceStr, Timestamp, Request.Url.AbsoluteUri));
                return signature;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Code = GetString("code");
                Openid = Session["session-questionopenid"] as string;
                if (string.IsNullOrEmpty(Openid) && !string.IsNullOrEmpty(Code))
                {
                    Openid = WeixinActs.Instance.GetOpenid(appid, appsecret, Code);
                    if (!string.IsNullOrEmpty(Openid))
                    {
                        Session["session-questionopenid"] = Openid;
                    }
                }
                if (!string.IsNullOrEmpty(Openid))
                {
                    string accesstoken = WeixinActs.Instance.GetAccessToken(appid, appsecret);
                    Dictionary<string, string> openinfo = WeixinActs.Instance.GetOpeninfo(accesstoken, Openid);
                    Session["session-questionnickname"] = openinfo["nickname"];
                }
                LoadData();
            }
            else
                btnSubmit_Click();
        }

        private void LoadData()
        {
            rptProject.DataSource = EnumExtensions.ToTable<QuestionItem>();
            rptProject.DataBind();
        }

        protected void rptProject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Repeater rptQuestion = (Repeater)e.Item.FindControl("rptQuestion");
                DataRowView row = (DataRowView)e.Item.DataItem;
                List<QuestionInfo> listQuestion = WeixinActs.Instance.GetQuestionList();
                listQuestion = listQuestion.FindAll(l => l.QuestionType == QuestionType.主管 && l.QuestionItem.ToString() == row["Name"].ToString()).OrderBy(l => l.ID).ToList();
                rptQuestion.DataSource = listQuestion;
                rptQuestion.DataBind();
            }
        }

        protected void rptQuestion_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Web.UI.HtmlControls.HtmlInputHidden hdnQuestionID = (System.Web.UI.HtmlControls.HtmlInputHidden)e.Item.FindControl("hdnQuestionID");
                System.Web.UI.HtmlControls.HtmlInputHidden hdnQuestionFactor = (System.Web.UI.HtmlControls.HtmlInputHidden)e.Item.FindControl("hdnQuestionFactor");
                QuestionInfo question = (QuestionInfo)e.Item.DataItem;
                hdnQuestionID.Value = question.ID.ToString();
                hdnQuestionFactor.Value = question.QuestionFacor;
            }
        }

        protected string GetNumCH(int index)
        {
            string result = "";
            switch (index)
            {
                case 1: result = "一";
                    break;
                case 2: result = "二";
                    break;
                case 3: result = "三";
                    break;
                case 4: result = "四";
                    break;
                case 5: result = "五";
                    break;
                default:
                    break;
            }

            return result;
        }

        protected void btnSubmit_Click()
        {
            if (CurrentCompany == null)
            {
                Page.ClientScript.RegisterStartupScript(typeof(string), "aa", "alert(\"非法访问，类型1\")", true);
                return;
            }

            Openid = Session["session-questionopenid"] as string;
            if (!string.IsNullOrEmpty(Openid))
            {
                if (WeixinActs.Instance.CheckQuestionPostUser(Openid))
                {
                    QuestionRecordInfo record = new QuestionRecordInfo();
                    record.PostTime = DateTime.Now;
                    record.PostUser = Openid;
                    record.PostUserName = Session["session-questionnickname"] as string;
                    record.QuestionCompanyID = GetInt("cid");
                    record.QuestionType = QuestionType.主管;
                    record.QuestionScoreList = new List<QuestionScoreInfo>();
                    foreach (RepeaterItem pitem in rptProject.Items)
                    {
                        if (pitem.ItemType == ListItemType.Item || pitem.ItemType == ListItemType.AlternatingItem)
                        {
                            Repeater rptQuestion = (Repeater)pitem.FindControl("rptQuestion");
                            foreach (RepeaterItem qitem in rptQuestion.Items)
                            {
                                System.Web.UI.HtmlControls.HtmlInputHidden hdnQuestionID = (System.Web.UI.HtmlControls.HtmlInputHidden)qitem.FindControl("hdnQuestionID");
                                System.Web.UI.HtmlControls.HtmlInputHidden hdnQuestionFactor = (System.Web.UI.HtmlControls.HtmlInputHidden)qitem.FindControl("hdnQuestionFactor");
                                RadioButtonList rblScore = (RadioButtonList)qitem.FindControl("rblScore");

                                QuestionScoreInfo score = new QuestionScoreInfo()
                                {
                                    QuestionFactor = hdnQuestionFactor.Value,
                                    QuestionID = DataConvert.SafeInt(hdnQuestionID.Value),
                                    Score = DataConvert.SafeDecimal(rblScore.SelectedValue)
                                };
                                record.QuestionScoreList.Add(score);
                            }
                        }
                    }
                    record.QuestionScoreInfoListJson = json.Serialize(record.QuestionScoreList);

                    if (WeixinActs.Instance.AddQuestionRecordInfo(record) > 0)
                    {
                        Page.ClientScript.RegisterStartupScript(typeof(string), "aa", "alert(\"提交成功!\")", true);
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(typeof(string), "aa", "alert(\"提交失败!\")", true);
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(typeof(string), "aa", "alert(\"您已经提交过问卷，谢谢您的参与!\")", true);
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(typeof(string), "aa", "alert(\"非法访问，类型2\")", true);
            }
        }
    }
}