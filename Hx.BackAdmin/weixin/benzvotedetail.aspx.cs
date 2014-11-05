using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools.Web;

namespace Hx.BackAdmin.weixin
{
    public partial class benzvotedetail : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Openid { get; set; }
        protected BenzvotePothunterInfo CurrentPothunterInfo = null;
        private BenzvoteSettingInfo currentsetting = null;
        protected BenzvoteSettingInfo CurrentSetting 
        {
            get
            {
                if (currentsetting == null)
                    currentsetting = WeixinActs.Instance.GetBenzvoteSetting(true);
                return currentsetting;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Openid = GetString("openid");
                LoadData();
            }
        }

        private void LoadData()
        {
            int id = GetInt("id");
            CurrentPothunterInfo = WeixinActs.Instance.GetBenzvotePothunterInfo(id,true);
            if (CurrentPothunterInfo == null)
            {
                Response.Write("<script>alert(\"选手信息错误\");location.href=\"" + (string.IsNullOrEmpty(FromUrl) ? "benzvotepage.aspx" : FromUrl) + "\"</script>");
                Response.End();
            }
        }

        /// <summary>
        /// 登录页面不需要验证
        /// </summary>
        protected override void Check()
        {

        }
    }
}