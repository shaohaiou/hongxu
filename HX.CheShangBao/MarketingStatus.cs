using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Hx.Tools;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Car.Enum;
using System.Threading;
using Hx.Tools.Web;
using System.IO;
using System.Text.RegularExpressions;
using HX.Tools;

namespace HX.CheShangBao
{
    public partial class MarketingStatus : Form
    {
        public int CarID { get; set; }

        public string Accounts { get; set; }

        private JcbCarInfo _currentcar = null;
        private JcbCarInfo CurrentCar
        {
            get
            {
                if (_currentcar == null)
                    _currentcar = JcbCars.Instance.GetModelRemote(CarID, true);
                return _currentcar;
            }
        }

        public HtmlDocument CurrentDoc { get; set; }

        private Dictionary<string, JcbAccountInfo> wbAccount = new Dictionary<string, JcbAccountInfo>();

        private static object sync_upload = new object();
        private static object sync_account = new object();

        public MarketingStatus()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void LoadData()
        {
            string url = "http://jcb.hongxu.cn/inventory/marketingstatus.aspx";
            if (CarID > 0 && !string.IsNullOrEmpty(Accounts))
                url += "?id=" + CarID + "&accounts=" + Accounts;
            wbcontent.Url = new Uri(url);
        }

        #region 移动窗体

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private void MarketingStatus_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF012, 0);
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MarketingStatus_Load(object sender, EventArgs e)
        {
            LoadData();

            wbcontent.Focus();
        }

        private void wbcontent_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString() != wbcontent.Url.ToString())
                return;
            if (wbcontent.ReadyState != WebBrowserReadyState.Complete)
                return;

            CurrentDoc = wbcontent.Document;

            string[] ids = Accounts.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            WebBrowser[] wbs = new WebBrowser[] { wbJob1, wbJob2, wbJob3, wbJob4, wbJob5 };
            List<MarketJob> jobs = new List<MarketJob>();
            foreach (string account in ids)
            {
                MarketJob jobinfo = new MarketJob();
                jobinfo.AccountID = DataConvert.SafeInt(account);
                jobinfo.Wb = wbs[jobs.Count % 5];
                jobs.Add(jobinfo);
            }

            foreach (WebBrowser wb in wbs)
            {
                if (jobs.Exists(j => j.Wb.Name == wb.Name))
                {
                    Thread t = new Thread(new ParameterizedThreadStart(DoMarketing));
                    t.Start(jobs.FindAll(j => j.Wb.Name == wb.Name));
                }
            }
        }

        private void DoMarketing(object obj)
        {
            List<MarketJob> jobs = (List<MarketJob>)obj;
            foreach (MarketJob jobInfo in jobs)
            {
                if (CurrentCar != null)
                {
                    JcbAccountInfo account = Jcbs.Instance.GetAccountModelRemote(jobInfo.AccountID, true);
                    if (account != null)
                    {
                        lock (sync_account)
                        {
                            wbAccount.Add(jobInfo.Wb.Name, account);
                        }
                        WriteMsg(jobInfo.AccountID, "正在登录营销网站");
                        string url = Jcbs.Instance.GetLoginUrl(account);
                        LoadPage(url, jobInfo.Wb);
                    }
                    else
                        WriteMsg(jobInfo.AccountID, "帐号信息错误");
                }
                else
                {
                    WriteMsg(jobInfo.AccountID, "车辆信息错误");
                }
            }
        }

        #region 页面交互

        private void WriteMsg(int accountid, string msg)
        {
            if (CurrentDoc != null)
            {
                HtmlElement txtMsg = CurrentDoc.All["txtMsg" + accountid];
                txtMsg.InnerText = msg;
            }
        }

        private void WriteViewUrl(int accountid, string url)
        {
            if (CurrentDoc != null)
            {
                HtmlElement linkView = CurrentDoc.All["linkView" + accountid];
                linkView.InnerText = "查看";
                linkView.SetAttribute("href", url);
                HtmlElement txtMsg = CurrentDoc.All["txtMsg" + accountid];
                txtMsg.SetAttribute("class", "green");
            }
        }

        private void LoadPage(string url, WebBrowser wb)
        {
            wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_LoadPageDocumentCompleted);
            wb.Navigate(url);
        }

        private void wb_LoadPageDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            if (wb.ReadyState != WebBrowserReadyState.Complete)
                return;
            if (e.Url.ToString() != wb.Url.ToString())
                return;

            HtmlDocument HtmlDoc = wb.Document;
            if (wbAccount.ContainsKey(wb.Name))
            {
                JcbAccountInfo account = wbAccount[wb.Name];
                try
                {
                    switch (account.JcbSiteType)
                    {
                        case JcbSiteType.t_二手车之家:
                            if (account.JcbAccountType == JcbAccountType.个人帐号)
                            {
                                #region 自动提交信息
                                //登录
                                if (wb.Url.ToString() == Jcbs.Instance.GetLoginUrl(account))
                                {
                                    HtmlElement txtUserName = HtmlDoc.All["UserName"];
                                    HtmlElement txtPassWord = HtmlDoc.All["PassWord"];
                                    HtmlElement SubmitLogin = HtmlDoc.All["SubmitLogin"];
                                    txtUserName.SetAttribute("value", account.AccountName);
                                    txtPassWord.SetAttribute("value", account.Password);
                                    SubmitLogin.InvokeMember("click");
                                    wb.Stop();
                                    Utils.DelayRun(1000, delegate()
                                    {
                                        wb.Navigate(Jcbs.Instance.GetPublicUrl(account));
                                    });
                                }
                                //发布信息
                                else if (wb.Url.ToString() == Jcbs.Instance.GetPublicUrl(account))
                                {
                                    WriteMsg(account.ID, "正在发布车辆信息...");
                                    if (CurrentCar.cCxmc.Length > 24)
                                    {
                                        WriteMsg(account.ID, "该车辆信息不适合快速发布");
                                        lock (sync_account)
                                        {
                                            wbAccount.Remove(wb.Name);
                                        }
                                        return;
                                    }
                                    HtmlElement selectBSYS = HtmlDoc.All["selectBSYS"];
                                    foreach (HtmlElement h in selectBSYS.GetElementsByTagName("a"))
                                    {
                                        if (h.InnerHtml == "填写一个")
                                        {
                                            h.InvokeMember("click");
                                            break;
                                        }
                                    }
                                    HtmlElement carName = HtmlDoc.All["carName"];
                                    carName.SetAttribute("value", CurrentCar.cCxmc);
                                    HtmlElement selectDisplaDIV = HtmlDoc.All["selectDisplaDIV"];
                                    foreach (HtmlElement h in selectDisplaDIV.GetElementsByTagName("a"))
                                    {
                                        if (h.InnerHtml == "填写一个")
                                        {
                                            h.InvokeMember("click");
                                            break;
                                        }
                                    }
                                    HtmlElement wirteDispla = HtmlDoc.All["wirteDispla"];
                                    wirteDispla.SetAttribute("value", CurrentCar.Pailiang);
                                    if (CurrentCar.Bsx == "自动")
                                    {
                                        HtmlElement gearbox1 = HtmlDoc.All["gearbox1"];
                                        gearbox1.InvokeMember("click");
                                    }
                                    else
                                    {
                                        HtmlElement gearbox2 = HtmlDoc.All["gearbox2"];
                                        gearbox2.InvokeMember("click");
                                    }
                                    HtmlElement caroption1 = HtmlDoc.All["caroption1"];
                                    HtmlElement caroption2 = HtmlDoc.All["caroption2"];
                                    string[] cardeployqt = CurrentCar.Cardeployqt.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] cardeploydd = CurrentCar.Cardeploydd.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] cardeployyx = CurrentCar.Cardeployyx.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] cardeployzy = CurrentCar.Cardeployzy.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (cardeployqt.Contains("无钥匙启动系统"))
                                        caroption1.GetElementsByTagName("input")[5].SetAttribute("checked", "checked");
                                    if (cardeployqt.Contains("车身稳定控制(ESP/DSC/VSC等)"))
                                        caroption1.GetElementsByTagName("input")[7].SetAttribute("checked", "checked");
                                    if (cardeployqt.Contains("空气悬挂"))
                                        caroption1.GetElementsByTagName("input")[8].SetAttribute("checked", "checked");
                                    if (cardeployqt.Contains("电动天窗"))
                                        caroption2.GetElementsByTagName("input")[0].SetAttribute("checked", "checked");
                                    if (cardeployqt.Contains("定速巡航"))
                                        caroption2.GetElementsByTagName("input")[3].SetAttribute("checked", "checked");
                                    if (cardeployzy.Contains("真皮/仿皮座椅"))
                                        caroption2.GetElementsByTagName("input")[5].SetAttribute("checked", "checked");
                                    if (cardeployqt.Contains("GPS导航系统"))
                                        caroption2.GetElementsByTagName("input")[6].SetAttribute("checked", "checked");
                                    if (cardeploydd.Contains("氙气大灯"))
                                        caroption2.GetElementsByTagName("input")[7].SetAttribute("checked", "checked");
                                    if (cardeployqt.Contains("自动空调"))
                                        caroption2.GetElementsByTagName("input")[8].SetAttribute("checked", "checked");
                                    if (cardeployqt.Contains("倒车影像"))
                                        caroption2.GetElementsByTagName("input")[10].SetAttribute("checked", "checked");
                                    if (cardeployzy.Contains("前排座椅加热"))
                                        caroption2.GetElementsByTagName("input")[11].SetAttribute("checked", "checked");
                                    HtmlElement sh_province_div = HtmlDoc.All["sh_province_div"];
                                    string province = CurrentCar.CarPromary.Substring(0, 2);
                                    foreach (HtmlElement elem in sh_province_div.GetElementsByTagName("a"))
                                    {
                                        if (elem.InnerHtml.IndexOf(province) > 0)
                                        {
                                            elem.InvokeMember("click");
                                            break;
                                        }
                                    }
                                    HtmlElement sh_city_div = HtmlDoc.All["sh_city_div"];
                                    string city = CurrentCar.CarCity.Substring(0, 2);
                                    foreach (HtmlElement elem in sh_city_div.GetElementsByTagName("a"))
                                    {
                                        if (elem.InnerHtml.IndexOf(city) > 0)
                                        {
                                            elem.InvokeMember("click");
                                            break;
                                        }
                                    }
                                    if (!CurrentCar.Iswsp)
                                    {
                                        DateTime scsprq = DataConvert.SafeDate(CurrentCar.Scsprq);
                                        HtmlElement sh_RegistDateYear_div = HtmlDoc.All["sh_RegistDateYear_div"];
                                        foreach (HtmlElement elem in sh_RegistDateYear_div.GetElementsByTagName("a"))
                                        {
                                            if (elem.InnerHtml == scsprq.Year.ToString())
                                            {
                                                elem.InvokeMember("click");
                                                break;
                                            }
                                        }
                                        HtmlElement sh_RegistDateMonth_div = HtmlDoc.All["sh_RegistDateMonth_div"];
                                        foreach (HtmlElement elem in sh_RegistDateMonth_div.GetElementsByTagName("a"))
                                        {
                                            if (elem.InnerHtml == scsprq.Month.ToString())
                                            {
                                                elem.InvokeMember("click");
                                                break;
                                            }
                                        }
                                    }
                                    HtmlElement txt_mileage = HtmlDoc.All["txt_mileage"];
                                    txt_mileage.SetAttribute("value", CurrentCar.Bxlc);
                                    if (!CurrentCar.Isnjygq)
                                    {
                                        DateTime njyxq = DataConvert.SafeDate(CurrentCar.Njyxq);
                                        HtmlElement sh_AuditYear_div = HtmlDoc.All["sh_AuditYear_div"];
                                        foreach (HtmlElement elem in sh_AuditYear_div.GetElementsByTagName("a"))
                                        {
                                            if (elem.InnerHtml == njyxq.Year.ToString())
                                            {
                                                elem.InvokeMember("click");
                                                break;
                                            }
                                        }
                                        HtmlElement sh_AuditMonth_div = HtmlDoc.All["sh_AuditMonth_div"];
                                        foreach (HtmlElement elem in sh_AuditMonth_div.GetElementsByTagName("a"))
                                        {
                                            if (elem.InnerHtml == njyxq.Month.ToString())
                                            {
                                                elem.InvokeMember("click");
                                                break;
                                            }
                                        }
                                    }
                                    if (!CurrentCar.Isjqxygq)
                                    {
                                        DateTime jqxyxq = DataConvert.SafeDate(CurrentCar.Jqxyxq);
                                        HtmlElement sh_InsuranceYear_div = HtmlDoc.All["sh_InsuranceYear_div"];
                                        foreach (HtmlElement elem in sh_InsuranceYear_div.GetElementsByTagName("a"))
                                        {
                                            if (elem.InnerHtml == jqxyxq.Year.ToString())
                                            {
                                                elem.InvokeMember("click");
                                                break;
                                            }
                                        }
                                        HtmlElement sh_InsuranceMoth_div = HtmlDoc.All["sh_InsuranceMoth_div"];
                                        foreach (HtmlElement elem in sh_InsuranceMoth_div.GetElementsByTagName("a"))
                                        {
                                            if (elem.InnerHtml == jqxyxq.Month.ToString())
                                            {
                                                elem.InvokeMember("click");
                                                break;
                                            }
                                        }
                                    }
                                    string ysjbhghf = CurrentCar.IsYsjbhghf ? "包过户费" : "不包过户费";
                                    HtmlElement sh_istrans_div = HtmlDoc.All["sh_istrans_div"];
                                    foreach (HtmlElement elem in sh_istrans_div.GetElementsByTagName("a"))
                                    {
                                        if (elem.InnerHtml == ysjbhghf)
                                        {
                                            elem.InvokeMember("click");
                                            break;
                                        }
                                    }
                                    HtmlElement txt_price = HtmlDoc.All["txt_price"];
                                    txt_price.SetAttribute("value", CurrentCar.Ysj.ToString());
                                    HtmlElement colorall = HtmlDoc.All["colorall"];
                                    foreach (HtmlElement elem in colorall.GetElementsByTagName("li"))
                                    {
                                        if (elem.InnerHtml.IndexOf(CurrentCar.Wgys) > 0 || elem.InnerHtml.IndexOf("其它") > 0)
                                        {
                                            elem.InvokeMember("click");
                                            break;
                                        }
                                    }
                                    if (CurrentCar.Dqby == "4s店定期保养")
                                    {
                                        HtmlElement Maintenance1 = HtmlDoc.All["Maintenance1"];
                                        Maintenance1.InvokeMember("click");
                                    }
                                    else
                                    {
                                        HtmlElement Maintenance2 = HtmlDoc.All["Maintenance2"];
                                        Maintenance2.InvokeMember("click");
                                    }
                                    HtmlElement in_linkname = HtmlDoc.All["in_linkname"];
                                    in_linkname.SetAttribute("value", Global.CurrentUser.Name);
                                    HtmlElement in_phone = HtmlDoc.All["in_phone"];
                                    in_phone.SetAttribute("value", Global.CurrentUser.Mobile);
                                    HtmlElement txtRemarkContent = HtmlDoc.All["txtRemarkContent"];
                                    txtRemarkContent.SetAttribute("value", CurrentCar.Czzs);

                                    carName.InvokeMember("focus");
                                    carName.InvokeMember("blur");
                                    wirteDispla.InvokeMember("focus");
                                    wirteDispla.InvokeMember("blur");
                                    txt_mileage.InvokeMember("focus");
                                    txt_mileage.InvokeMember("blur");
                                    in_linkname.InvokeMember("focus");
                                    in_linkname.InvokeMember("blur");
                                    in_phone.InvokeMember("focus");
                                    in_phone.InvokeMember("blur");

                                    #region 图片上传

                                    bool hascarpics = false;
                                    bool hasdjzupload = true;
                                    bool hasxszupload = true;
                                    bool hasgcfpupload = true;
                                    bool hascarpicsupload = false;

                                    AsyncCallback uploadpic = delegate(IAsyncResult ar)
                                    {
                                        if (CurrentCar.Djz == "有")
                                        {
                                            HtmlElement dj_yes = HtmlDoc.All["dj_yes"];
                                            dj_yes.InvokeMember("click");
                                            if (!string.IsNullOrEmpty(CurrentCar.DjzPic))
                                            {
                                                string picurl = string.Format(Global.Host + CurrentCar.DjzPic);
                                                UploadPic(picurl, HtmlDoc.All["dj_cert"]);
                                                Utils.DelayRun(100, delegate()
                                                {
                                                    DateTime begintime = DateTime.Now;
                                                    HtmlElement dj_certmsg = HtmlDoc.All["dj_certmsg"];
                                                    string classname = dj_certmsg.GetAttribute("className").ToLower();
                                                    while (classname != "twright" && DateTime.Now.Subtract(begintime).TotalMilliseconds < 5000)
                                                    {
                                                        Thread.Sleep(100);
                                                        dj_certmsg = HtmlDoc.All["dj_certmsg"];
                                                        classname = dj_certmsg.GetAttribute("className").ToLower();
                                                    }
                                                    hasdjzupload = classname == "twright";
                                                });
                                            }
                                        }
                                        else if (CurrentCar.Djz == "丢失")
                                        {
                                            HtmlElement dj_lost = HtmlDoc.All["dj_lost"];
                                            dj_lost.InvokeMember("click");
                                        }
                                        else if (CurrentCar.Djz == "补办中")
                                        {
                                            HtmlElement dj_doing = HtmlDoc.All["dj_doing"];
                                            dj_doing.InvokeMember("click");
                                        }
                                        if (CurrentCar.Xsz == "有")
                                        {
                                            HtmlElement xs_yes = HtmlDoc.All["xs_yes"];
                                            xs_yes.InvokeMember("click");
                                            if (!string.IsNullOrEmpty(CurrentCar.XszPic))
                                            {
                                                string picurl = string.Format(Global.Host + CurrentCar.XszPic);
                                                UploadPic(picurl, HtmlDoc.All["xs_cert"]);
                                                Utils.DelayRun(100, delegate()
                                                {
                                                    DateTime begintime = DateTime.Now;
                                                    HtmlElement xs_certmsg = HtmlDoc.All["xs_certmsg"];
                                                    string classname = xs_certmsg.GetAttribute("className").ToLower();
                                                    while (classname != "twright" && DateTime.Now.Subtract(begintime).TotalMilliseconds < 5000)
                                                    {
                                                        Thread.Sleep(100);
                                                        xs_certmsg = HtmlDoc.All["xs_certmsg"];
                                                        classname = xs_certmsg.GetAttribute("className").ToLower();
                                                    }
                                                    hasxszupload = classname == "twright";
                                                });
                                            }
                                        }
                                        else if (CurrentCar.Xsz == "丢失")
                                        {
                                            HtmlElement xs_lost = HtmlDoc.All["xs_lost"];
                                            xs_lost.InvokeMember("click");
                                        }
                                        else if (CurrentCar.Xsz == "补办中")
                                        {
                                            HtmlElement xs_doing = HtmlDoc.All["xs_doing"];
                                            xs_doing.InvokeMember("click");
                                        }
                                        if (CurrentCar.Gcfp == "有")
                                        {
                                            HtmlElement gc_yes = HtmlDoc.All["gc_yes"];
                                            gc_yes.InvokeMember("click");
                                            if (!string.IsNullOrEmpty(CurrentCar.GcfpPic))
                                            {
                                                string picurl = string.Format(Global.Host + CurrentCar.GcfpPic);
                                                UploadPic(picurl, HtmlDoc.All["gc_cert"]);
                                                Utils.DelayRun(100, delegate()
                                                {
                                                    DateTime begintime = DateTime.Now;
                                                    HtmlElement gc_certmsg = HtmlDoc.All["gc_certmsg"];
                                                    string classname = gc_certmsg.GetAttribute("className").ToLower();
                                                    while (classname != "twright" && DateTime.Now.Subtract(begintime).TotalMilliseconds < 5000)
                                                    {
                                                        Thread.Sleep(100);
                                                        gc_certmsg = HtmlDoc.All["gc_certmsg"];
                                                        classname = gc_certmsg.GetAttribute("className").ToLower();
                                                    }
                                                    hasgcfpupload = classname == "twright";
                                                });
                                            }
                                        }
                                        else if (CurrentCar.Gcfp == "丢失")
                                        {
                                            HtmlElement gc_lost = HtmlDoc.All["gc_lost"];
                                            gc_lost.InvokeMember("click");
                                        }
                                        else if (CurrentCar.Gcfp == "补办中")
                                        {
                                            HtmlElement gc_doing = HtmlDoc.All["gc_doing"];
                                            gc_doing.InvokeMember("click");
                                        }

                                        if (hascarpics)
                                        {
                                            Utils.DelayRun(100, delegate()
                                            {
                                                DateTime begintime = DateTime.Now;
                                                HtmlElement uploadProgress = HtmlDoc.All["uploadProgress"];
                                                string style = uploadProgress.Style.ToLower();
                                                while (style != "display: none" && DateTime.Now.Subtract(begintime).TotalMilliseconds < 5000)
                                                {
                                                    Thread.Sleep(100);
                                                    uploadProgress = HtmlDoc.All["uploadProgress"];
                                                    style = uploadProgress.Style.ToLower();
                                                }
                                                hascarpicsupload = style == "display: none";
                                            });
                                        }
                                        else
                                            hascarpicsupload = true;

                                        Utils.DelayRun(100, delegate()
                                        {
                                            HtmlElement CarSubmit = HtmlDoc.All["CarSubmit"];
                                            DateTime begintime = DateTime.Now;

                                            if (hascarpics)
                                            {
                                                while (!(hasdjzupload && hasxszupload && hasgcfpupload && hascarpicsupload) && DateTime.Now.Subtract(begintime).TotalMilliseconds < 5000)
                                                {
                                                    Thread.Sleep(100);
                                                }
                                                if (hasdjzupload && hasxszupload && hasgcfpupload && hascarpicsupload)
                                                    CarSubmit.InvokeMember("click");
                                                else
                                                {
                                                    WriteMsg(account.ID, "该车辆信息不适合快速发布");
                                                    lock (sync_account)
                                                    {
                                                        wbAccount.Remove(wb.Name);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (!(hasdjzupload && hasxszupload && hasgcfpupload) && DateTime.Now.Subtract(begintime).TotalMilliseconds < 5000)
                                                {
                                                    Thread.Sleep(100);
                                                }
                                                if (hasdjzupload && hasxszupload && hasgcfpupload)
                                                    CarSubmit.InvokeMember("click");
                                                else
                                                {
                                                    WriteMsg(account.ID, "该车辆信息不适合快速发布");
                                                    lock (sync_account)
                                                    {
                                                        wbAccount.Remove(wb.Name);
                                                    }
                                                }
                                            }
                                        });
                                    };

                                    if (CurrentCar.Picslist.Count > 0)
                                    {
                                        WriteMsg(account.ID, "正在上传车辆图片...");
                                        hascarpics = true;
                                        string picurls = string.Join(" ", CurrentCar.Picslist.OrderByDescending(p => p.IsFirstpic).ToList().Select(p => Global.Host + p.PicUrl).ToList());
                                        HtmlElement uploadModel1 = HtmlDoc.All["uploadModel1"];
                                        UploadPicByMouseLeftClick(picurls, uploadModel1, wb, uploadpic);
                                    }
                                    else
                                    {
                                        uploadpic(null);
                                    }

                                    #endregion
                                }
                                else if (wb.Url.ToString().StartsWith(Jcbs.Instance.GetSuccessUrl(account)))
                                {
                                    Regex r_id = new Regex(@"InfoId=([\d]+)");
                                    if (r_id.IsMatch(wb.Url.ToString()))
                                    {
                                        string viewurl = Jcbs.Instance.GetViewUrl(account, r_id.Match(wb.Url.ToString()).Groups[1].Value);
                                        WriteMsg(account.ID, "信息发布成功");
                                        WriteViewUrl(account.ID, viewurl);
                                        lock (sync_account)
                                        {
                                            wbAccount.Remove(wb.Name);
                                        }

                                        JcbMarketrecordInfo entity = new JcbMarketrecordInfo()
                                        {
                                            CarID = CurrentCar.ID,
                                            AccountID = account.ID,
                                            IsDel = false,
                                            IsSale = false,
                                            JcbSiteType = account.JcbSiteType,
                                            UploadTime = DateTime.Now,
                                            ViewUrl = viewurl
                                        };
                                        Jcbs.Instance.CreateAndUpdateMarketrecordRemote(entity);
                                    }
                                }

                                #endregion
                            }
                            else if (account.JcbAccountType == JcbAccountType.商户帐号)
                            {
                                #region 自动提交信息



                                #endregion
                            }
                            break;
                        case JcbSiteType.t_58同城:
                            return;
                            #region 自动提交信息

                            if (wb.Url.ToString() == Jcbs.Instance.GetPublicUrl(account))
                            {
                                string pinpai = CurrentCar.cChangs.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                                HtmlElement fenLeiCon = HtmlDoc.All["fenLeiCon"];
                                foreach (HtmlElement h in fenLeiCon.GetElementsByTagName("a"))
                                {
                                    if (h.InnerHtml == pinpai)
                                        h.InvokeMember("click");
                                }
                                HtmlElement chexi = null;
                                Utils.DelayRun(500, delegate()
                                {
                                    HtmlDoc = wb.Document;
                                    HtmlElement chexiFidercon = HtmlDoc.All["chexiFidercon"];
                                    int pcount = 0;
                                    while (chexiFidercon.GetElementsByTagName("a").Count == 0 && pcount < 50)
                                    {
                                        HtmlDoc = wb.Document;
                                        chexiFidercon = HtmlDoc.All["chexiFidercon"];
                                        Thread.Sleep(100);
                                        pcount++;
                                    }
                                    if (chexiFidercon.GetElementsByTagName("a").Count == 0)
                                        return;
                                    foreach (HtmlElement h in chexiFidercon.GetElementsByTagName("a"))
                                    {
                                        if (CurrentCar.cCxmc.IndexOf(h.InnerHtml) >= 0)
                                        {
                                            h.InvokeMember("click");
                                            chexi = h;
                                            break;
                                        }
                                    }
                                    Utils.DelayRun(500, delegate()
                                    {
                                        pcount = 0;
                                        HtmlDoc = wb.Document;
                                        HtmlElement chexingFidercon = HtmlDoc.All["chexingFidercon"];
                                        while (chexingFidercon.GetElementsByTagName("a").Count == 0 && pcount < 50)
                                        {
                                            HtmlDoc = wb.Document;
                                            chexingFidercon = HtmlDoc.All["cheXing"];
                                            Thread.Sleep(100);
                                            pcount++;
                                        }
                                        if (chexingFidercon.GetElementsByTagName("a").Count == 0)
                                            return;
                                        //string strchexi = chexi == null ? string.Empty : chexi.InnerHtml;
                                        //string chexing = CurrentCar.cCxmc.Replace(CurrentCar.cChangs, string.Empty).Replace(chexi.InnerHtml, string.Empty);
                                        //foreach (HtmlElement h in chexingFidercon.GetElementsByTagName("a"))
                                        //{
                                        //    if (chexing.IndexOf(h.InnerHtml) >= 0)
                                        //    {
                                        //        h.InvokeMember("click");
                                        //        break;
                                        //    }
                                        //}
                                        HtmlDoc = wb.Document.Window.Frames["carframe_id"].Document;
                                        HtmlElement y_shangpai = HtmlDoc.All.GetElementsByName("y_shangpai")[0];
                                        bool hassel = false;
                                        DateTime scspsj = DataConvert.SafeDate(CurrentCar.Scsprq);
                                        foreach (HtmlElement h in y_shangpai.GetElementsByTagName("option"))
                                        {
                                            if (DataConvert.SafeInt(h.InnerHtml) == scspsj.Year)
                                            {
                                                h.SetAttribute("selected", "selected");
                                                y_shangpai.InvokeMember("focus");
                                                SendKeys.Send("Space");
                                                hassel = true;
                                                break;
                                            }
                                        }
                                        return;
                                        Utils.DelayRun(500, delegate()
                                        {
                                            #region 车辆颜色

                                            pcount = 0;
                                            HtmlDoc = wb.Document.Window.Frames["carframe_id"].Document;
                                            HtmlElement yanse_xianshi = HtmlDoc.All.GetElementsByName("yanse_xianshi").Count > 0 ? HtmlDoc.All.GetElementsByName("yanse_xianshi")[0] : null;
                                            while ((yanse_xianshi == null || yanse_xianshi.GetElementsByTagName("a").Count == 0) && pcount < 50)
                                            {
                                                HtmlDoc = wb.Document.Window.Frames["carframe_id"].Document;
                                                yanse_xianshi = HtmlDoc.All.GetElementsByName("yanse_xianshi").Count > 0 ? HtmlDoc.All.GetElementsByName("yanse_xianshi")[0] : null;
                                                Thread.Sleep(100);
                                                pcount++;
                                            }
                                            if (yanse_xianshi.GetElementsByTagName("a").Count == 0)
                                                return;
                                            Regex r = new Regex(@"<[\s\S]+?>");
                                            foreach (HtmlElement h in yanse_xianshi.GetElementsByTagName("a"))
                                            {
                                                string ys = r.Replace(h.InnerHtml, string.Empty);
                                                if (CurrentCar.Wgys.IndexOf(ys) >= 0 || ys == "其他")
                                                {
                                                    h.InvokeMember("click");
                                                    break;
                                                }
                                            }

                                            #endregion

                                            #region 首次上牌时间

                                            Utils.DelayRun(1000, delegate()
                                            {
                                                //HtmlDoc = wb.Document.Window.Frames["carframe_id"].Document;
                                                //HtmlElement y_shangpai = HtmlDoc.All.GetElementsByName("y_shangpai")[0];
                                                //bool hassel = false;
                                                //DateTime scspsj = DataConvert.SafeDate(CurrentCar.Scsprq);
                                                //foreach (HtmlElement h in y_shangpai.GetElementsByTagName("option"))
                                                //{
                                                //    if (DataConvert.SafeInt(h.InnerHtml) == scspsj.Year)
                                                //    {
                                                //        h.SetAttribute("selected", "selected");
                                                //        y_shangpai.InvokeMember("change");
                                                //        hassel = true;
                                                //    }
                                                //}
                                                //if (!hassel)
                                                //{
                                                //    y_shangpai.SetAttribute("value", y_shangpai.GetElementsByTagName("option")[y_shangpai.GetElementsByTagName("option").Count - 1].GetAttribute("value"));
                                                //    y_shangpai.RaiseEvent("onchange");
                                                //}
                                                Utils.DelayRun(500, delegate()
                                                {
                                                    pcount = 0;
                                                    HtmlDoc = wb.Document.Window.Frames["carframe_id"].Document;
                                                    HtmlElement m_shangpai = HtmlDoc.All.GetElementsByName("m_shangpai").Count > 0 ? HtmlDoc.All.GetElementsByName("m_shangpai")[0] : null;
                                                    while ((m_shangpai == null || m_shangpai.GetElementsByTagName("option").Count == 0) && pcount < 20)
                                                    {
                                                        HtmlDoc = wb.Document.Window.Frames["carframe_id"].Document;
                                                        m_shangpai = HtmlDoc.All.GetElementsByName("m_shangpai").Count > 0 ? HtmlDoc.All.GetElementsByName("m_shangpai")[0] : null;
                                                        Thread.Sleep(100);
                                                        pcount++;
                                                    }
                                                    if (m_shangpai == null || m_shangpai.GetElementsByTagName("option").Count == 0)
                                                        return;
                                                    foreach (HtmlElement h in m_shangpai.GetElementsByTagName("option"))
                                                    {
                                                        if (DataConvert.SafeInt(h.InnerHtml) == scspsj.Month)
                                                        {
                                                            m_shangpai.SetAttribute("value", h.GetAttribute("value"));
                                                        }
                                                    }
                                                });
                                            });
                                            #endregion


                                        });
                                    });
                                });
                            }
                            else
                            {
                                HtmlElement username = HtmlDoc.All["username"];
                                if (username != null)
                                {
                                    HtmlElement password = HtmlDoc.All["password"];
                                    HtmlElement btnSubmit = HtmlDoc.All["btnSubmit"];
                                    username.SetAttribute("value", account.AccountName);
                                    password.SetAttribute("value", account.Password);
                                    btnSubmit.InvokeMember("click");
                                }
                                else
                                {
                                    HtmlElement login_name = HtmlDoc.All["login-name"];
                                    if (!string.IsNullOrEmpty(login_name.InnerHtml))
                                        wb.Navigate(Jcbs.Instance.GetPublicUrl(account));
                                }
                            }

                            #endregion
                            break;
                        case JcbSiteType.赶集网:
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    WriteMsg(account.ID, "车辆信息不符合快速营销方式");
                    lock (sync_account)
                    {
                        wbAccount.Remove(wb.Name);
                    }
                }
            }
        }

        private void UploadPic(string picurl, HtmlElement fileinput)
        {
            lock (sync_upload)
            {
                string savepath = Utils.GetMapPath("temp\\") + DateTime.Now.ToString("mmss") + DateTime.Now.Millisecond + picurl.Substring(picurl.LastIndexOf("."), picurl.Length - picurl.LastIndexOf("."));
                Http.DownloadFile(picurl, savepath, 500);
                if (File.Exists(savepath))
                {
                    bool windowopen = false;
                    BackgroundWorker b = new BackgroundWorker();
                    b.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                    {
                        while (!windowopen)
                            Thread.Sleep(100);
                        Thread.Sleep(200);
                        bool hasfindhandle = true;
                        DateTime timestart = DateTime.Now;
                        IntPtr hwndCalc = CSharpWindowsAPI.FindWindow("#32770", null);
                        while (hwndCalc == IntPtr.Zero)
                        {
                            hwndCalc = CSharpWindowsAPI.FindWindow("#32770", null);
                            if (DateTime.Now.Subtract(timestart).TotalMilliseconds > 2000)
                            {
                                hasfindhandle = false;
                                break;
                            }
                        }
                        if (hasfindhandle)
                        {
                            IntPtr hwndFilepath = CSharpWindowsAPI.FindWindowEx(hwndCalc, 0, "ComboBoxEx32", null);
                            if (hwndFilepath != IntPtr.Zero)
                            {
                                CSharpWindowsAPI.SendText(hwndFilepath, e.Argument.ToString());
                            }
                            IntPtr hwndSubmit = CSharpWindowsAPI.FindWindowEx(hwndCalc, 0, null, "打开(&O)");
                            if (hwndSubmit != IntPtr.Zero)
                            {
                                CSharpWindowsAPI.LeftMouseDown(hwndSubmit);
                            }
                        }
                    });
                    b.RunWorkerAsync(savepath);
                    windowopen = true;
                    fileinput.InvokeMember("click");
                }
            }
        }

        private void UploadPicByMouseLeftClick(string picurls, HtmlElement h, WebBrowser wb, AsyncCallback callback)
        {
            lock (sync_upload)
            {
                string[] pics = picurls.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> listdownloaded = new List<string>();
                foreach (string picurl in pics)
                {
                    string savepath = Utils.GetMapPath("temp\\") + DateTime.Now.ToString("mmss") + DateTime.Now.Millisecond + picurl.Substring(picurl.LastIndexOf("."), picurl.Length - picurl.LastIndexOf("."));
                    Http.DownloadFile(picurl, savepath, 500);
                    if (File.Exists(savepath))
                        listdownloaded.Add(savepath);
                }

                if (listdownloaded.Count > 0)
                {
                    bool windowopen = false;
                    BackgroundWorker b = new BackgroundWorker();
                    b.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                    {
                        while (!windowopen)
                            Thread.Sleep(100);
                        Thread.Sleep(200);
                        bool hasfindhandle = true;
                        DateTime timestart = DateTime.Now;
                        IntPtr hwndCalc = CSharpWindowsAPI.FindWindow("#32770", null);
                        while (hwndCalc == IntPtr.Zero)
                        {
                            hwndCalc = CSharpWindowsAPI.FindWindow("#32770", null);
                            if (DateTime.Now.Subtract(timestart).TotalMilliseconds > 5000)
                            {
                                hasfindhandle = false;
                                break;
                            }
                        }
                        if (hasfindhandle)
                        {
                            IntPtr hwndFilepath = CSharpWindowsAPI.FindWindowEx(hwndCalc, 0, "ComboBoxEx32", null);
                            if (hwndFilepath != IntPtr.Zero)
                            {
                                CSharpWindowsAPI.SendText(hwndFilepath, e.Argument.ToString());
                            }
                            IntPtr hwndSubmit = CSharpWindowsAPI.FindWindowEx(hwndCalc, 0, null, "打开(&O)");
                            if (hwndSubmit != IntPtr.Zero)
                            {
                                CSharpWindowsAPI.LeftMouseDown(hwndSubmit);
                            }
                        }
                        else
                            throw new Exception();
                        Utils.DelayRun(1000, delegate()
                        {
                            callback(null);
                        });
                        //List<HX.Tools.CSharpWindowsAPI.WindowInfo> windows = CSharpWindowsAPI.GetAllDesktopWindows().ToList();
                        //while (!windows.Exists(w => w.szWindowName.IndexOf("选择要") >= 0))
                        //{
                        //    windows = CSharpWindowsAPI.GetAllDesktopWindows().ToList();
                        //    Thread.Sleep(200);
                        //}
                        //HX.Tools.CSharpWindowsAPI.WindowInfo window = windows.Find(w => w.szWindowName.IndexOf("选择要") >= 0);
                        //List<HX.Tools.CSharpWindowsAPI.WindowInfo> children = CSharpWindowsAPI.GetChildWindows(window.hWnd).ToList();
                        //if (children.Exists(w => w.szClassName == "ComboBoxEx32") && children.Exists(w => w.szWindowName == "打开(&O)"))
                        //{
                        //    CSharpWindowsAPI.SendText(children.Find(w => w.szClassName == "ComboBoxEx32").hWnd, e.Argument.ToString());
                        //    CSharpWindowsAPI.LeftMouseDown(children.Find(w => w.szWindowName == "打开(&O)").hWnd);
                        //}
                    });
                    b.RunWorkerAsync(listdownloaded.Count == 1 ? listdownloaded[0] : ("\"" + string.Join("\" \"", listdownloaded) + "\""));
                    MouseLeftClick(h, wb, ref windowopen);
                }
            }
        }

        private void MouseLeftClick(HtmlElement h, WebBrowser wb, ref bool hasclick)
        {
            h.ScrollIntoView(true);

            int x = 10; // X coordinate of the click 
            int y = 30; // Y coordinate of the click 
            IntPtr handle = wb.Handle;
            StringBuilder className = new StringBuilder(100);
            while (className.ToString() != "Internet Explorer_Server") // The class control for the browser 
            {
                handle = CSharpWindowsAPI.GetWindow(handle, 5); // Get a handle to the child window 
                CSharpWindowsAPI.GetClassName(handle, className, className.Capacity);
            }

            IntPtr lParam = (IntPtr)((y << 16) | x); // The coordinates 
            IntPtr wParam = IntPtr.Zero; // Additional parameters for the click (e.g. Ctrl) 
            const uint downCode = 0x201; // Left click down code 
            const uint upCode = 0x202; // Left click up code 
            CSharpWindowsAPI.SendMessage(handle, downCode, wParam, lParam); // Mouse button down 
            CSharpWindowsAPI.SendMessage(handle, upCode, wParam, lParam); // Mouse button up 
            hasclick = true;
        }

        #endregion
    }

    #region 营销作业类

    public class MarketJob
    {
        public MarketJob() { }

        /// <summary>
        /// 帐号ID
        /// </summary>
        public int AccountID { get; set; }

        public WebBrowser Wb { get; set; }
    }

    #endregion
}
