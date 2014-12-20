using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Query;
using Hx.Components.Enumerations;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Tools;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;

namespace Hx.BackAdmin.dayreport
{
    public partial class crmreportcustomerflow : AdminBase
    {
        protected override void Check()
        {
            if (string.IsNullOrEmpty(Nm) || Id == 0 || Mm == 0 || CurrentUser == null || !CheckUser())
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
            if (Mm != (DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day) * Id * 3)
            {
                Response.Clear();
                Response.Write("非法操作！");
                Response.End();
                return;
            }
        }

        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        private DayReportUserInfo currentuser = null;
        protected DayReportUserInfo CurrentUser
        {
            get
            {
                if (currentuser == null)
                    currentuser = DayReportUsers.Instance.GetModel(Id.ToString(), true);
                return currentuser;
            }
        }

        private string nm = string.Empty;
        protected string Nm
        {
            get
            {
                if (string.IsNullOrEmpty(nm))
                    nm = GetString("Nm");
                return nm;
            }
        }

        private int id = 0;
        protected int Id
        {
            get
            {
                if (id == 0)
                    id = GetInt("Id");
                return id;
            }
        }

        private int mm = 0;
        protected int Mm
        {
            get
            {
                if (mm == 0)
                    mm = GetInt("Mm");
                return mm;
            }
        }

        private DataTable cfvisitwaytable = null;
        public DataTable CFVisitwayTable
        {
            get
            {
                if (cfvisitwaytable == null)
                    cfvisitwaytable = EnumExtensions.ToTable<CFVisitway>();
                return cfvisitwaytable;
            }
        }

        private DataTable cfvisitnaturetable = null;
        public DataTable CFVisitNatureTable
        {
            get
            {
                if (cfvisitnaturetable == null)
                    cfvisitnaturetable = EnumExtensions.ToTable<CFVisitNature>();
                return cfvisitnaturetable;
            }
        }

        private DataTable cfvisitchanneltable = null;
        public DataTable CFVisitChannelTable
        {
            get
            {
                if (cfvisitchanneltable == null)
                    cfvisitchanneltable = EnumExtensions.ToTable<CFVisitChannel>();
                return cfvisitchanneltable;
            }
        }

        private DataTable cfbuytypetable = null;
        public DataTable CFBuyTypeTable
        {
            get
            {
                if (cfbuytypetable == null)
                    cfbuytypetable = EnumExtensions.ToTable<CFBuyType>();
                return cfbuytypetable;
            }
        }

        private DataTable cfleveltable = null;
        public DataTable CFLevelTable
        {
            get
            {
                if (cfleveltable == null)
                    cfleveltable = EnumExtensions.ToTable<CFLevel>();
                return cfleveltable;
            }
        }

        private DataTable cfcardinfotable = null;
        public DataTable CFCardInfoTable
        {
            get
            {
                if (cfcardinfotable == null)
                    cfcardinfotable = EnumExtensions.ToTable<CFCardInfo>();
                return cfcardinfotable;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
                LoadData();
            }
        }

        private void BindControler()
        {
            txtDate.Text = DateTime.Today.ToString("yyyy-MM");

            string[] corppowers = CurrentUser.DayReportCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<CorporationInfo> corps = Corporations.Instance.GetList(true);
            ddlCorp.DataSource = corps.FindAll(c => corppowers.Contains(c.ID.ToString()) || (c.ID == CurrentUser.CorporationID && corppowers.Length == 0));
            ddlCorp.DataTextField = "Name";
            ddlCorp.DataValueField = "ID";
            ddlCorp.DataBind();
            SetSelectedByValue(ddlCorp, CurrentUser.CorporationID.ToString());

            ddlCFVisitway.DataSource = CFVisitwayTable;
            ddlCFVisitway.DataTextField = "Name";
            ddlCFVisitway.DataValueField = "Value";
            ddlCFVisitway.DataBind();

            ddlCFVisitNature.DataSource = CFVisitNatureTable;
            ddlCFVisitNature.DataTextField = "Name";
            ddlCFVisitNature.DataValueField = "Value";
            ddlCFVisitNature.DataBind();

            ddlCFVisitChannel.DataSource = CFVisitChannelTable;
            ddlCFVisitChannel.DataTextField = "Name";
            ddlCFVisitChannel.DataValueField = "Value";
            ddlCFVisitChannel.DataBind();

            ddlCFBuyType.DataSource = CFBuyTypeTable;
            ddlCFBuyType.DataTextField = "Name";
            ddlCFBuyType.DataValueField = "Value";
            ddlCFBuyType.DataBind();

            ddlCFLevel.DataSource = CFLevelTable;
            ddlCFLevel.DataTextField = "Name";
            ddlCFLevel.DataValueField = "Value";
            ddlCFLevel.DataBind();

            ddlCFCardInfo.DataSource = CFCardInfoTable;
            ddlCFCardInfo.DataTextField = "Name";
            ddlCFCardInfo.DataValueField = "Value";
            ddlCFCardInfo.DataBind();
        }

        private void LoadData()
        {
            rptNav.DataSource = EnumExtensions.ToTable<CRMReportType>();
            rptNav.DataBind();

            CRMReportQuery query = new CRMReportQuery();
            query.CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue);
            query.CRMReportType = CRMReportType.客流量登记表;
            query.MonthStr = txtDate.Text.Replace("-",string.Empty);

            List<CRMReportInfo> list = CRMReports.Instance.GetList(query, true);
            rptData.DataSource = list;
            rptData.DataBind();

        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CRMReportInfo info = (CRMReportInfo)e.Item.DataItem;
                System.Web.UI.WebControls.DropDownList ddlCFVisitway = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCFVisitway");
                System.Web.UI.WebControls.DropDownList ddlCFVisitNature = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCFVisitNature");
                System.Web.UI.WebControls.DropDownList ddlCFVisitChannel = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCFVisitChannel");
                System.Web.UI.WebControls.DropDownList ddlCFBuyType = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCFBuyType");
                System.Web.UI.WebControls.DropDownList ddlCFLevel = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCFLevel");
                System.Web.UI.WebControls.DropDownList ddlCFCardInfo = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCFCardInfo");

                if (ddlCFVisitway != null)
                {
                    ddlCFVisitway.DataSource = CFVisitwayTable;
                    ddlCFVisitway.DataTextField = "Name";
                    ddlCFVisitway.DataValueField = "Value";
                    ddlCFVisitway.DataBind();

                    SetSelectedByValue(ddlCFVisitway, ((int)info.CFVisitway).ToString());
                }
                if (ddlCFVisitNature != null)
                {
                    ddlCFVisitNature.DataSource = CFVisitNatureTable;
                    ddlCFVisitNature.DataTextField = "Name";
                    ddlCFVisitNature.DataValueField = "Value";
                    ddlCFVisitNature.DataBind();

                    SetSelectedByValue(ddlCFVisitNature, ((int)info.CFVisitNature).ToString());
                }
                if (ddlCFVisitChannel != null)
                {
                    ddlCFVisitChannel.DataSource = CFVisitChannelTable;
                    ddlCFVisitChannel.DataTextField = "Name";
                    ddlCFVisitChannel.DataValueField = "Value";
                    ddlCFVisitChannel.DataBind();

                    SetSelectedByValue(ddlCFVisitChannel, ((int)info.CFVisitChannel).ToString());
                }
                if (ddlCFBuyType != null)
                {
                    ddlCFBuyType.DataSource = CFBuyTypeTable;
                    ddlCFBuyType.DataTextField = "Name";
                    ddlCFBuyType.DataValueField = "Value";
                    ddlCFBuyType.DataBind();

                    SetSelectedByValue(ddlCFBuyType, ((int)info.CFBuyType).ToString());
                }
                if (ddlCFLevel != null)
                {
                    ddlCFLevel.DataSource = CFLevelTable;
                    ddlCFLevel.DataTextField = "Name";
                    ddlCFLevel.DataValueField = "Value";
                    ddlCFLevel.DataBind();

                    SetSelectedByValue(ddlCFLevel, ((int)info.CFLevel).ToString());
                }
                if (ddlCFCardInfo != null)
                {
                    ddlCFCardInfo.DataSource = CFCardInfoTable;
                    ddlCFCardInfo.DataTextField = "Name";
                    ddlCFCardInfo.DataValueField = "Value";
                    ddlCFCardInfo.DataBind();

                    SetSelectedByValue(ddlCFCardInfo, ((int)info.CFCardInfo).ToString());
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                CRMReports.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string date = Request["txtDate" + i];
                    int cfvisitway = DataConvert.SafeInt(Request["ddlCFVisitway" + i]);
                    string cfvisittime = Request["txtCFVisitTime" + i];
                    string cfleavetime = Request["txtCFLeaveTime" + i];
                    string cfreceiver = Request["txtCFReceiver" + i];
                    string cfcustomername = Request["txtCFCustomerName" + i];
                    int cfvisitnature = DataConvert.SafeInt(Request["ddlCFVisitNature" + i]);
                    string cfphonenum = Request["txtCFPhoneNum" + i];
                    string cfintentionmodel = Request["txtCFIntentionModel" + i];
                    int cfvisitchannel = DataConvert.SafeInt(Request["ddlCFVisitChannel" + i]);
                    int cfbuytype = DataConvert.SafeInt(Request["ddlCFBuyType" + i]);
                    string cfmodelinuse = Request["txtCFModelInUse" + i];
                    int cfisloan = DataConvert.SafeInt(Request["hdnCFIsLoan" + i]);
                    string cffromarea = Request["txtCFFromArea" + i];
                    int cflevel = DataConvert.SafeInt(Request["ddlCFLevel" + i]);
                    int cfcardinfo = DataConvert.SafeInt(Request["ddlCFCardInfo" + i]);
                    int cfisride = DataConvert.SafeInt(Request["hdnCFIsRide" + i]);
                    int cfisorder = DataConvert.SafeInt(Request["hdnCFIsOrder" + i]);
                    int cfisinvoice = DataConvert.SafeInt(Request["hdnCFIsInvoice" + i]);
                    int cfisturnover = DataConvert.SafeInt(Request["hdnCFIsTurnover" + i]);

                    if (!string.IsNullOrEmpty(cfvisittime) 
                        && !string.IsNullOrEmpty(cfleavetime) 
                        && !string.IsNullOrEmpty(cfcustomername)
                        && !string.IsNullOrEmpty(date))
                    {
                        CRMReportInfo entity = new CRMReportInfo
                        {
                            CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                            Creator = CurrentUser.UserName,
                            LastUpdateUser = CurrentUser.UserName,
                            CRMReportType = CRMReportType.客流量登记表,
                            Date = DataConvert.SafeDate(date),
                            CFVisitway = cfvisitway,
                            CFVisitTime = cfvisittime,
                            CFLeaveTime = cfleavetime,
                            CFReceiver = cfreceiver,
                            CFCustomerName = cfcustomername,
                            CFVisitNature = cfvisitnature,
                            CFPhoneNum = cfphonenum,
                            CFIntentionModel = cfintentionmodel,
                            CFVisitChannel = cfvisitchannel,
                            CFBuyType = cfbuytype,
                            CFModelInUse = cfmodelinuse,
                            CFIsLoan = cfisloan,
                            CFFromArea = cffromarea,
                            CFLevel = cflevel,
                            CFCardInfo = cfcardinfo,
                            CFIsRide = cfisride,
                            CFIsOrder = cfisorder,
                            CFIsInvoice = cfisinvoice,
                            CFIsTurnover = cfisturnover
                        };
                        CRMReports.Instance.CreateAndUpdate(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtDate1 = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtDate");
                    System.Web.UI.WebControls.DropDownList ddlCFVisitway = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCFVisitway");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFVisitTime = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFVisitTime");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFLeaveTime = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFLeaveTime");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFReceiver = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFReceiver");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFCustomerName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFCustomerName");
                    System.Web.UI.WebControls.DropDownList ddlCFVisitNature = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCFVisitNature");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFPhoneNum = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFPhoneNum");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFIntentionModel = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFIntentionModel");
                    System.Web.UI.WebControls.DropDownList ddlCFVisitChannel = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCFVisitChannel");
                    System.Web.UI.WebControls.DropDownList ddlCFBuyType = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCFBuyType");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFModelInUse = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFModelInUse");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxCFIsLoan = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxCFIsLoan");
                    System.Web.UI.HtmlControls.HtmlInputText txtCFFromArea = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtCFFromArea");
                    System.Web.UI.WebControls.DropDownList ddlCFLevel = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCFLevel");
                    System.Web.UI.WebControls.DropDownList ddlCFCardInfo = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCFCardInfo");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxCFIsRide = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxCFIsRide");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxCFIsOrder = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxCFIsOrder");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxCFIsInvoice = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxCFIsInvoice");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxCFIsTurnover = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxCFIsTurnover");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            CRMReportInfo entity = new CRMReportInfo
                            {
                                ID = DataConvert.SafeInt(hdnID.Value),
                                LastUpdateUser = CurrentUser.UserName,
                                Date = DataConvert.SafeDate(txtDate1.Value),
                                CFVisitway = DataConvert.SafeInt(ddlCFVisitway.SelectedValue),
                                CFVisitTime = txtCFVisitTime.Value,
                                CFLeaveTime = txtCFLeaveTime.Value,
                                CFReceiver = txtCFReceiver.Value,
                                CFCustomerName = txtCFCustomerName.Value,
                                CFVisitNature = DataConvert.SafeInt(ddlCFVisitNature.SelectedValue),
                                CFPhoneNum = txtCFPhoneNum.Value,
                                CFIntentionModel = txtCFIntentionModel.Value,
                                CFVisitChannel = DataConvert.SafeInt(ddlCFVisitChannel.SelectedValue),
                                CFBuyType = DataConvert.SafeInt(ddlCFBuyType.SelectedValue),
                                CFModelInUse = txtCFModelInUse.Value,
                                CFIsLoan = cbxCFIsLoan.Checked ? 1 : 0,
                                CFFromArea = txtCFFromArea.Value,
                                CFLevel = DataConvert.SafeInt(ddlCFLevel.SelectedValue),
                                CFCardInfo = DataConvert.SafeInt(ddlCFCardInfo.SelectedValue),
                                CFIsRide = cbxCFIsRide.Checked ? 1 : 0,
                                CFIsOrder = cbxCFIsOrder.Checked ? 1 : 0,
                                CFIsInvoice = cbxCFIsInvoice.Checked ? 1 : 0,
                                CFIsTurnover = cbxCFIsTurnover.Checked ? 1 : 0
                            };
                            CRMReports.Instance.CreateAndUpdate(entity);
                        }
                    }
                }
            }

            CRMReportQuery query = new CRMReportQuery 
            { 
                CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                CRMReportType = CRMReportType.客流量登记表,
                MonthStr = txtDate.Text.Replace("-",string.Empty)
            };
            CRMReports.Instance.ReloadCRMReportListCache(query);

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? ("~/dayreport/" + GetNavUrl((int)CRMReportType.客流量登记表)) : FromUrl);
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void ddlCorp_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 检查用户是否有权限
        /// </summary>
        /// <returns></returns>
        private bool CheckUser()
        {
            string[] crmpowers = CurrentUser.CRMReportInputPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!crmpowers.Contains(((int)CRMReportType.客流量登记表).ToString()))
                return false;

            return true;
        }

        protected string GetNavUrl(object v)
        {
            return CRMReports.Instance.GetNavUrl((CRMReportType)DataConvert.SafeInt(v), Nm, Id, Mm);
        }

        protected bool HasReportInputPower(object v)
        {
            string[] p = CurrentUser.CRMReportInputPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return p.Contains(v.ToString());
        }
    }
}