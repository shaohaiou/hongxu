using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car.Entity;
using Hx.Car;
using System.Data;
using Hx.Tools;
using Hx.Car.Enum;
using System.Web.UI.HtmlControls;

namespace Hx.JcbWeb.inventory
{
    public partial class yxtg : JcbBase
    {
        private JcbCarInfo _currentcar = null;
        protected JcbCarInfo CurrentCar
        {
            get
            {
                if (_currentcar == null)
                {
                    int id = GetInt("id");
                    _currentcar = JcbCars.Instance.GetModel(id,true);
                }
                return _currentcar;
            }
        }

        private List<JcbMarketrecordInfo> _listrecord = null;
        private List<JcbMarketrecordInfo> ListRecord
        {
            get
            {
                if (_listrecord == null)
                {
                    _listrecord = Jcbs.Instance.GetMarketrecordList(true);
                }
                return _listrecord;
            }
        }

        private List<JcbAccountInfo> _listaccount = null;
        private List<JcbAccountInfo> ListAccount
        {
            get
            {
                if (_listaccount == null)
                {
                    _listaccount = Jcbs.Instance.GetAccountList(true);
                }
                return _listaccount;
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
            if (CurrentCar == null)
            {
                Response.Write("车辆信息不存在");
                Response.End();
            }
            else
            {
#if DEBUG
                DataTable t = EnumExtensions.ToTable<JcbSiteType>();
                t.DefaultView.RowFilter = "[Text] <> '58同城' and [Text] <> '赶集网'";
                rptData.DataSource = t.DefaultView;
                rptData.DataBind();
#else
            rptData.DataSource = EnumExtensions.ToTable<JcbSiteType>();
            rptData.DataBind();
#endif
            }
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                Label lblMarketingStatus = (Label)e.Item.FindControl("lblMarketingStatus");
                Label lblUploadDate = (Label)e.Item.FindControl("lblUploadDate");
                HtmlAnchor btnAutoLogin = (System.Web.UI.HtmlControls.HtmlAnchor)e.Item.FindControl("btnAutoLogin");
                HtmlAnchor btnView = (System.Web.UI.HtmlControls.HtmlAnchor)e.Item.FindControl("btnView");
                if (ListAccount.Exists(a => a.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]) && a.UserID == UserID))
                {
                    btnAutoLogin.Attributes["val"] = ListAccount.Find(a => a.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]) && a.UserID == UserID).ID.ToString();
                }
                else
                {
                    btnAutoLogin.Attributes["class"] = "btnAutoLogin dis";
                }
                if (ListRecord.Exists(r => r.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]) && r.CarID == CurrentCar.ID && ListAccount.Exists(a=>a.ID == r.AccountID)))
                {
                    lblMarketingStatus.Text = "已营销";
                    if (ListRecord.Exists(r => r.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]) && r.CarID == CurrentCar.ID && r.UploadTime != null))
                    {
                        JcbMarketrecordInfo record = ListRecord.FindAll(r => r.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]) && r.CarID == CurrentCar.ID && r.UploadTime != null).ToList().First();
                        lblUploadDate.Text = record.UploadTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        btnView.HRef = record.ViewUrl;
                        btnView.Attributes["class"] = "";
                        btnAutoLogin.Attributes["val"] = record.AccountID.ToString();
                    }
                    else
                        lblUploadDate.Text = "未上传";
                }
                else
                {
                    lblMarketingStatus.Text = "未营销";
                    lblUploadDate.Text = "&nbsp;";
                }
            }
        }
    }
}