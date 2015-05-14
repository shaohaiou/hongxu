using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Providers;
using Hx.Components;
using Hx.Car.Entity;
using Hx.Car.Enum;
using Hx.Tools.Web;
using System.Web.Script.Serialization;
using Hx.Components.Entity;
using System.Collections.Specialized;
using System.Windows.Forms;
using Hx.Tools.ValidationCode;
using System.Drawing;

namespace Hx.Car
{
    public class Jcbs
    {
        private static JavaScriptSerializer json = new JavaScriptSerializer();
        #region 单例
        private static object sync_creater = new object();

        private static Jcbs _instance;
        public static Jcbs Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Jcbs();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 帐号管理

        public int CreateAndUpdateAccount(JcbAccountInfo entity)
        {
            return CarDataProvider.Instance().AddJcbAccount(entity);
        }

        public List<JcbAccountInfo> GetAccountList(bool fromCache = false)
        {
            if (!fromCache)
                return CarDataProvider.Instance().GetJcbAccountList();

            string key = GlobalKey.JCBACCOUNT_LIST;
            List<JcbAccountInfo> list = MangaCache.Get(key) as List<JcbAccountInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetJcbAccountList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public JcbAccountInfo GetAccountModel(int id, bool fromCache = false)
        {
            List<JcbAccountInfo> list = GetAccountList(fromCache);
            return list.Find(c => c.ID == id);
        }

        public void DeleteAccount(string ids)
        {
            CarDataProvider.Instance().DeleteAccount(ids);
        }

        public void ReloadAccountListCache()
        {
            string key = GlobalKey.JCBACCOUNT_LIST;
            MangaCache.Remove(key);
            GetAccountList(true);
        }

        public JcbAccountInfo GetAccountModelRemote(int id, bool fromCache = false)
        {
            string url = "http://jcb.hongxu.cn/jcbapi.axd?action=getaccountinfo&id=" + id;
            string jsonstr = Http.GetPage(url);
            return json.Deserialize<JcbAccountInfo>(jsonstr);
        }

        #endregion

        #region 营销记录

        public int CreateAndUpdateMarketrecord(JcbMarketrecordInfo entity)
        {
            return CarDataProvider.Instance().AddJcbMarketrecord(entity);
        }

        public List<JcbMarketrecordInfo> GetMarketrecordList(bool fromCache = false)
        {
            if (!fromCache)
                return CarDataProvider.Instance().GetJcbMarketrecordList();

            string key = GlobalKey.JCBMARKETRECORD_LIST;
            List<JcbMarketrecordInfo> list = MangaCache.Get(key) as List<JcbMarketrecordInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetJcbMarketrecordList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public JcbMarketrecordInfo GetMarketrecordModel(int id, bool fromCache = false)
        {
            List<JcbMarketrecordInfo> list = GetMarketrecordList(fromCache);
            return list.Find(c => c.ID == id);
        }

        public void ReloadMarketrecordListCache()
        {
            string key = GlobalKey.JCBMARKETRECORD_LIST;
            MangaCache.Remove(key);
            GetMarketrecordList(true);
        }

        public void CreateAndUpdateMarketrecordRemote(JcbMarketrecordInfo entity)
        {
            string url = "http://jcb.hongxu.cn/jcbapi.axd";
            NameValueCollection postVars = new NameValueCollection();
            postVars.Add("action", "createmarketrecord");
            postVars.Add("datastr", json.Serialize(entity));
            Http.PostData(postVars, new string[] { url });
        }

        #endregion

        #region 用户管理

        public JcbUserInfo GetJcbUserRemote(string username, string password)
        {
            string url = "http://jcb.hongxu.cn/jcbapi.axd?action=getjcbuserinfo&username=" + username + "&password=" + password;
            string jsonstr = Http.GetPage(url);
            return json.Deserialize<JcbUserInfo>(jsonstr);
        }

        #endregion

        #region 其他

        public string GetLoginUrl(JcbAccountInfo account)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://account.che168.com/login";
                    else if (account.JcbAccountType == JcbAccountType.商户帐号)
                        result = "http://dealer.che168.com/login.html";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://passport.58.com/login";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                case JcbSiteType.搜狐二手车:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://2sc.sohu.com/usercenter/login/";
                    else if (account.JcbAccountType == JcbAccountType.商户帐号)
                        result = "http://2sc.sohu.com/ctb/";
                    break;
                default:
                    break;
            }

            return result;
        }

        public string GetLoginedUrl(JcbAccountInfo account)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://account.che168.com/login";
                    else if (account.JcbAccountType == JcbAccountType.商户帐号)
                        result = "http://dealer.che168.com/index.html";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://passport.58.com/login";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                case JcbSiteType.搜狐二手车:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://2sc.sohu.com/usercenter/car/";
                    else if (account.JcbAccountType == JcbAccountType.商户帐号)
                        result = "http://2sc.sohu.com/ctb/";
                    break;
                default:
                    break;
            }

            return result;
        }

        public string GetSuccessUrl(JcbAccountInfo account)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://i.che168.com/car/success/";
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://passport.58.com/login";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                default:
                    break;
            }

            return result;
        }

        public string GetPublicUrl(JcbAccountInfo account)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://i.che168.com/car/add/";
                    else if (account.JcbAccountType == JcbAccountType.商户帐号)
                        result = "http://dealer.che168.com/car/publish/?s=1";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://post.58.com/330/29/s5";
                    break;
                case JcbSiteType.赶集网:
                    result = "http://www.ganji.com/pub/pub.php?act=pub&method=load&cid=6&mcid=14";
                    break;
                case JcbSiteType.搜狐二手车:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://2sc.sohu.com/sell/add/";
                    else if (account.JcbAccountType == JcbAccountType.商户帐号)
                        result = "http://2sc.sohu.com/ctb/";
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="cid">车辆ID</param>
        /// <returns></returns>
        public string GetViewUrl(JcbAccountInfo account, string cid)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://www.che168.com/Personal/CarPreview_V3.aspx?infoid=" + cid;
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://post.58.com/330/29/s5";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                default:
                    break;
            }

            return result;
        }

        public string GetListUrl(JcbAccountInfo account)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://account.che168.com/login";
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://post.58.com/330/29/s5";
                    break;
                case JcbSiteType.赶集网:
                    result = "http://www.ganji.com/pub/pub_select.php";
                    break;
                default:
                    break;
            }

            return result;
        }

        public string GetSiteUrl(JcbSiteType sitetype)
        {
            string result = string.Empty;

            switch (sitetype)
            {
                case JcbSiteType.t_二手车之家:
                    result = "http://www.che168.com/";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://www.58.com/";
                    break;
                case JcbSiteType.赶集网:
                    result = "http://www.ganji.com/";
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion

        #region 登录

        public int DoLogin(WebBrowser wb, JcbAccountInfo account)
        {
            int result = 0;
            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                    {
                        HtmlDocument HtmlDoc = wb.Document;
                        HtmlElement txtUserName = HtmlDoc.All["UserName"];
                        HtmlElement txtPassWord = HtmlDoc.All["PassWord"];
                        HtmlElement SubmitLogin = HtmlDoc.All["SubmitLogin"];
                        txtUserName.SetAttribute("value", account.AccountName);
                        txtPassWord.SetAttribute("value", account.Password);
                        SubmitLogin.InvokeMember("click");
                        wb.Stop();
                        result = 1;
                    }
                    else if (account.JcbAccountType == JcbAccountType.商户帐号)
                    {
                        HtmlDocument HtmlDoc = wb.Document;
                        HtmlElement txtUserName = HtmlDoc.All["userName"];
                        HtmlElement txtPassWord = HtmlDoc.All["userPWD"];
                        HtmlElement yzPwd = HtmlDoc.All["yzPwd"];
                        HtmlElement imgValidCode = HtmlDoc.All["imgValidCode"];
                        HtmlElement SubmitLogin = HtmlDoc.All["SubmitLogin"];
                        txtUserName.SetAttribute("value", account.AccountName);
                        txtPassWord.SetAttribute("value", account.Password);
                        //SubmitLogin.InvokeMember("click");
                        //wb.Stop();

                        //将元素绝对定位到页面左上角
                        imgValidCode.Style = "position: absolute; z-index: 9999; top: 0px; left: 0px";
                        //抓图
                        var b = new Bitmap(imgValidCode.ClientRectangle.Width, imgValidCode.ClientRectangle.Height);
                        wb.DrawToBitmap(b, new Rectangle(new Point(), imgValidCode.ClientRectangle.Size));
                        //ValidationImage img = new ValidationImage((Bitmap)b);
                        //List<double> t = img.test();
                        //int tt = NeuralNet.Recognize(t);
                        Cracker cracker = new Cracker();
                        string vcode = cracker.Read(b);
                        yzPwd.SetAttribute("value", vcode);
                    }
                    break;
                case JcbSiteType.赶集网:
                    if (true)
                    {
                        HtmlDocument HtmlDoc = wb.Document;
                        HtmlElement txtUserName = HtmlDoc.All["login_username"];
                        HtmlElement txtPassWord = HtmlDoc.All["login_password"];
                        HtmlElement SubmitLogin = null;
                        HtmlElement loginform = HtmlDoc.All["loginform"];
                        foreach (HtmlElement ele in loginform.GetElementsByTagName("input"))
                        {
                            if (ele.GetAttribute("type") == "submit")
                            {
                                SubmitLogin = ele;
                                break;
                            }
                        }

                        txtUserName.SetAttribute("value", account.AccountName);
                        txtPassWord.SetAttribute("value", account.Password);
                        if (SubmitLogin != null)
                        {
                            SubmitLogin.InvokeMember("click");
                            result = 1;
                        }
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        #endregion
    }
}
