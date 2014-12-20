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
    public partial class crmreportoutwardvisit : AdminBase
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

        /// <summary>
        /// 检查用户是否有权限
        /// </summary>
        /// <returns></returns>
        private bool CheckUser()
        {
            string[] crmpowers = CurrentUser.CRMReportInputPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!crmpowers.Contains(((int)CRMReportType.活动外出访客信息).ToString()))
                return false;

            return true;
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

        private DataTable ovcustomernaturetable = null;
        public DataTable OVCustomerNatureTable
        {
            get
            {
                if (ovcustomernaturetable == null)
                    ovcustomernaturetable = EnumExtensions.ToTable<OVCustomerNature>();
                return ovcustomernaturetable;
            }
        }

        private DataTable ovvisitchanneltable = null;
        public DataTable OVVisitChannelTable
        {
            get
            {
                if (ovvisitchanneltable == null)
                    ovvisitchanneltable = EnumExtensions.ToTable<CFVisitChannel>();
                return ovvisitchanneltable;
            }
        }

        private DataTable ovleveltable = null;
        public DataTable OVLevelTable
        {
            get
            {
                if (ovleveltable == null)
                    ovleveltable = EnumExtensions.ToTable<CFLevel>();
                return ovleveltable;
            }
        }

        private DataTable ovcardinfotable = null;
        public DataTable OVCardInfoTable
        {
            get
            {
                if (ovcardinfotable == null)
                    ovcardinfotable = EnumExtensions.ToTable<CFCardInfo>();
                return ovcardinfotable;
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

            ddlOVCustomerNature.DataSource = OVCustomerNatureTable;
            ddlOVCustomerNature.DataTextField = "Name";
            ddlOVCustomerNature.DataValueField = "Value";
            ddlOVCustomerNature.DataBind();

            ddlOVVisitChannel.DataSource = OVVisitChannelTable;
            ddlOVVisitChannel.DataTextField = "Name";
            ddlOVVisitChannel.DataValueField = "Value";
            ddlOVVisitChannel.DataBind();

            ddlOVLevel.DataSource = OVLevelTable;
            ddlOVLevel.DataTextField = "Name";
            ddlOVLevel.DataValueField = "Value";
            ddlOVLevel.DataBind();

            ddlOVCardInfo.DataSource = OVCardInfoTable;
            ddlOVCardInfo.DataTextField = "Name";
            ddlOVCardInfo.DataValueField = "Value";
            ddlOVCardInfo.DataBind();
        }      

        private void LoadData()
        {
            rptNav.DataSource = EnumExtensions.ToTable<CRMReportType>();
            rptNav.DataBind();

            CRMReportQuery query = new CRMReportQuery();
            query.CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue);
            query.CRMReportType = CRMReportType.活动外出访客信息;
            query.MonthStr = txtDate.Text.Replace("-", string.Empty);

            List<CRMReportInfo> list = CRMReports.Instance.GetList(query, true);
            rptData.DataSource = list;
            rptData.DataBind();

        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CRMReportInfo info = (CRMReportInfo)e.Item.DataItem;
                System.Web.UI.WebControls.DropDownList ddlOVCustomerNature = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlOVCustomerNature");
                System.Web.UI.WebControls.DropDownList ddlOVVisitChannel = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlOVVisitChannel");
                System.Web.UI.WebControls.DropDownList ddlOVLevel = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlOVLevel");
                System.Web.UI.WebControls.DropDownList ddlOVCardInfo = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlOVCardInfo");

                if (ddlOVCustomerNature != null)
                {
                    ddlOVCustomerNature.DataSource = OVCustomerNatureTable;
                    ddlOVCustomerNature.DataTextField = "Name";
                    ddlOVCustomerNature.DataValueField = "Value";
                    ddlOVCustomerNature.DataBind();

                    SetSelectedByValue(ddlOVCustomerNature, ((int)info.OVCustomerNature).ToString());
                }
                if (ddlOVVisitChannel != null)
                {
                    ddlOVVisitChannel.DataSource = OVVisitChannelTable;
                    ddlOVVisitChannel.DataTextField = "Name";
                    ddlOVVisitChannel.DataValueField = "Value";
                    ddlOVVisitChannel.DataBind();

                    SetSelectedByValue(ddlOVVisitChannel, ((int)info.OVVisitChannel).ToString());
                }
                if (ddlOVLevel != null)
                {
                    ddlOVLevel.DataSource = OVLevelTable;
                    ddlOVLevel.DataTextField = "Name";
                    ddlOVLevel.DataValueField = "Value";
                    ddlOVLevel.DataBind();

                    SetSelectedByValue(ddlOVLevel, ((int)info.OVLevel).ToString());
                }
                if (ddlOVCardInfo != null)
                {
                    ddlOVCardInfo.DataSource = OVCardInfoTable;
                    ddlOVCardInfo.DataTextField = "Name";
                    ddlOVCardInfo.DataValueField = "Value";
                    ddlOVCardInfo.DataBind();

                    SetSelectedByValue(ddlOVCardInfo, ((int)info.OVCardInfo).ToString());
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
                    int ovcustomernature = DataConvert.SafeInt(Request["ddlOVCustomerNature" + i]);
                    string ovactivename = Request["txtOVActiveName" + i];
                    string ovcustomername = Request["txtOVCustomerName" + i];
                    string ovphonenum = Request["txtOVPhoneNum" + i];
                    string ovreceiver = Request["txtOVReceiver" + i];
                    string ovintentionmodel = Request["txtOVIntentionModel" + i];
                    int ovvisitchannel = DataConvert.SafeInt(Request["ddlOVVisitChannel" + i]);
                    int ovlevel = DataConvert.SafeInt(Request["ddlOVLevel" + i]);
                    int ovisride = DataConvert.SafeInt(Request["hdnOVIsRide" + i]);
                    int ovcardinfo = DataConvert.SafeInt(Request["ddlOVCardInfo" + i]);
                    string ovvisitnexttime = Request["txtOVVisitNextTime" + i];

                    if (!string.IsNullOrEmpty(ovactivename)
                        && !string.IsNullOrEmpty(date)
                        && !string.IsNullOrEmpty(ovcustomername))
                    {
                        CRMReportInfo entity = new CRMReportInfo
                        {
                            CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                            Creator = CurrentUser.UserName,
                            LastUpdateUser = CurrentUser.UserName,
                            CRMReportType = CRMReportType.活动外出访客信息,
                            Date = DataConvert.SafeDate(date),
                            OVCustomerNature = ovcustomernature,
                            OVActiveName = ovactivename,
                            OVCustomerName = ovcustomername,
                            OVPhoneNum = ovphonenum,
                            OVReceiver = ovreceiver,
                            OVIntentionModel = ovintentionmodel,
                            OVVisitChannel = ovvisitchannel,
                            OVLevel = ovlevel,
                            OVIsRide = ovisride,
                            OVCardInfo = ovcardinfo,
                            OVVisitNexttime = ovvisitnexttime
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
                    System.Web.UI.WebControls.DropDownList ddlOVCustomerNature = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlOVCustomerNature");
                    System.Web.UI.HtmlControls.HtmlInputText txtOVActiveName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtOVActiveName");
                    System.Web.UI.HtmlControls.HtmlInputText txtOVCustomerName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtOVCustomerName");
                    System.Web.UI.HtmlControls.HtmlInputText txtOVPhoneNum = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtOVPhoneNum");
                    System.Web.UI.HtmlControls.HtmlInputText txtOVReceiver = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtOVReceiver");
                    System.Web.UI.HtmlControls.HtmlInputText txtOVIntentionModel = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtOVIntentionModel");
                    System.Web.UI.WebControls.DropDownList ddlOVVisitChannel = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlOVVisitChannel");
                    System.Web.UI.WebControls.DropDownList ddlOVLevel = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlOVLevel");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxOVIsRide = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxOVIsRide");
                    System.Web.UI.WebControls.DropDownList ddlOVCardInfo = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlOVCardInfo");
                    System.Web.UI.HtmlControls.HtmlInputText txtOVVisitNexttime = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtOVVisitNexttime");
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
                                OVCustomerNature = DataConvert.SafeInt(ddlOVCustomerNature.SelectedValue),
                                OVActiveName = txtOVActiveName.Value,
                                OVCustomerName = txtOVCustomerName.Value,
                                OVPhoneNum = txtOVPhoneNum.Value,
                                OVReceiver = txtOVReceiver.Value,
                                OVIntentionModel = txtOVIntentionModel.Value,
                                OVVisitChannel = DataConvert.SafeInt(ddlOVVisitChannel.SelectedValue),
                                OVLevel = DataConvert.SafeInt(ddlOVLevel.SelectedValue),
                                OVIsRide = cbxOVIsRide.Checked ? 1 : 0,
                                OVCardInfo = DataConvert.SafeInt(ddlOVCardInfo.SelectedValue),
                                OVVisitNexttime = txtOVVisitNexttime.Value
                            };
                            CRMReports.Instance.CreateAndUpdate(entity);
                        }
                    }
                }
            }

            CRMReportQuery query = new CRMReportQuery
            {
                CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                CRMReportType = CRMReportType.活动外出访客信息,
                MonthStr = txtDate.Text.Replace("-", string.Empty)
            };
            CRMReports.Instance.ReloadCRMReportListCache(query);

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? ("~/dayreport/" + GetNavUrl((int)CRMReportType.活动外出访客信息)) : FromUrl);
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void ddlCorp_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected string GetNavUrl(object v)
        {
            return CRMReports.Instance.GetNavUrl((CRMReportType)DataConvert.SafeInt(v),Nm,Id,Mm);
        }

        protected bool HasReportInputPower(object v)
        {
            string[] p = CurrentUser.CRMReportInputPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return p.Contains(v.ToString());
        }
    }
}