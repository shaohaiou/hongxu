using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Components.BasePage;
using Hx.Tools;
using System.Text.RegularExpressions;
using System.Data;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Enumerations;
using System.Web.UI.HtmlControls;
using Hx.Car.Enum;

namespace Hx.JcbWeb.inventory
{
    public partial class addcar : JcbBase
    {
        private static System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        private static object sync_data = new object();
        private CarInfo _car = null;
        protected CarInfo CurrentCar
        {
            get
            {
                if (_car == null)
                {
                    lock (sync_data)
                    {
                        if (_car == null)
                        {
                            string cChangs = ddlcChangs.SelectedItem.Value;
                            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(cChangs, true);
                            int carid = DataConvert.SafeInt(ddlcCxmc.SelectedValue);
                            _car = carlist.Find(c => c.id == carid);
                        }
                    }
                }
                return _car;
            }
        }

        private List<CarBrandInfo> currentCarbrandlist = null;
        private List<CarBrandInfo> CurrentCarbrandlist
        {
            get
            {
                if (currentCarbrandlist == null)
                {
                    currentCarbrandlist = CarBrands.Instance.GetCarBrandListByCorporation("-1");
                }

                return currentCarbrandlist;
            }
        }

        private DataTable pictypelist = null;
        public DataTable PicTypeList
        {
            get
            {
                if (pictypelist == null)
                    pictypelist = EnumExtensions.ToTable<JcbPicType>();
                return pictypelist;
            }
        }
        public bool IsCardeploy { get; set; }

        #region 年月列表

        /// <summary>
        /// 月份
        /// </summary>
        private List<KeyValuePair<int, int>> monthlist = null;
        protected List<KeyValuePair<int, int>> Monthlist
        {
            get
            {
                if (monthlist == null)
                {
                    monthlist = new List<KeyValuePair<int, int>>();
                    for (int i = 1; i <= 12; i++)
                    {
                        monthlist.Add(new KeyValuePair<int, int>(i, i));
                    }
                }
                return monthlist;
            }
        }

        /// <summary>
        /// 首次上牌年份
        /// </summary>
        private List<KeyValuePair<int, int>> yearlistscsp = null;
        protected List<KeyValuePair<int, int>> Yearlistscsp
        {
            get
            {
                if (yearlistscsp == null)
                {
                    yearlistscsp = new List<KeyValuePair<int, int>>();
                    for (int i = 0; i <= 20; i++)
                    {
                        yearlistscsp.Add(new KeyValuePair<int, int>(DateTime.Today.Year - i, DateTime.Today.Year - i));
                    }
                }
                return yearlistscsp;
            }
        }

        /// <summary>
        /// 年检有效期年份
        /// </summary>
        private List<KeyValuePair<int, int>> yearlistnjyxq = null;
        protected List<KeyValuePair<int, int>> Yearlistnjyxq
        {
            get
            {
                if (yearlistnjyxq == null)
                {
                    yearlistnjyxq = new List<KeyValuePair<int, int>>();
                    for (int i = 2; i >= 0; i--)
                    {
                        yearlistnjyxq.Add(new KeyValuePair<int, int>(DateTime.Today.Year + i, DateTime.Today.Year + i));
                    }
                }
                return yearlistnjyxq;
            }
        }

        /// <summary>
        /// 交强险年份
        /// </summary>
        private List<KeyValuePair<int, int>> yearlistjqxyxq = null;
        protected List<KeyValuePair<int, int>> Yearlistjqxyxq
        {
            get
            {
                if (yearlistjqxyxq == null)
                {
                    yearlistjqxyxq = new List<KeyValuePair<int, int>>();
                    for (int i = 1; i >= 0; i--)
                    {
                        yearlistjqxyxq.Add(new KeyValuePair<int, int>(DateTime.Today.Year + i, DateTime.Today.Year + i));
                    }
                }
                return yearlistjqxyxq;
            }
        }

        /// <summary>
        /// 商业险年份
        /// </summary>
        private List<KeyValuePair<int, int>> yearlistsyxyxq = null;
        protected List<KeyValuePair<int, int>> Yearlistsyxyxq
        {
            get
            {
                if (yearlistsyxyxq == null)
                {
                    yearlistsyxyxq = new List<KeyValuePair<int, int>>();
                    for (int i = 1; i >= 0; i--)
                    {
                        yearlistsyxyxq.Add(new KeyValuePair<int, int>(DateTime.Today.Year + i, DateTime.Today.Year + i));
                    }
                }
                return yearlistsyxyxq;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
                BindData();
            }
        }

        private void BindControler()
        {
            ddlcChangs.DataSource = CurrentCarbrandlist;
            ddlcChangs.DataTextField = "BindName";
            ddlcChangs.DataValueField = "Name";
            ddlcChangs.DataBind();
            ddlcChangs.Items.Insert(0, new ListItem("-车辆品牌-", "-1"));
            ddlcChangs.SelectedIndex = 0;

            ddlcCxmc.Items.Add(new ListItem("-选择车型-"));
            ddlWgys.Items.Add(new ListItem("-选择颜色-", ""));

            ddlCarPromary.DataSource = Areas.Instance.GetPromaryList(true);
            ddlCarPromary.DataTextField = "Name";
            ddlCarPromary.DataValueField = "ID";
            ddlCarPromary.DataBind();
            ddlCarPromary.Items.Insert(0, new ListItem("-选择省份-", "-1"));

            ddlCarCity.Items.Add(new ListItem("-选择城市-"));

            ddlYearScsprq.DataSource = Yearlistscsp;
            ddlYearScsprq.DataTextField = "Key";
            ddlYearScsprq.DataValueField = "Value";
            ddlYearScsprq.DataBind();

            ddlMonthScsprq.DataSource = Monthlist;
            ddlMonthScsprq.DataTextField = "Key";
            ddlMonthScsprq.DataValueField = "Value";
            ddlMonthScsprq.DataBind();

            ddlYearNjyxq.DataSource = Yearlistnjyxq;
            ddlYearNjyxq.DataTextField = "Key";
            ddlYearNjyxq.DataValueField = "Value";
            ddlYearNjyxq.DataBind();

            ddlMonthNjyxq.DataSource = Monthlist;
            ddlMonthNjyxq.DataTextField = "Key";
            ddlMonthNjyxq.DataValueField = "Value";
            ddlMonthNjyxq.DataBind();

            ddlYearJqxyxq.DataSource = Yearlistjqxyxq;
            ddlYearJqxyxq.DataTextField = "Key";
            ddlYearJqxyxq.DataValueField = "Value";
            ddlYearJqxyxq.DataBind();

            ddlMonthJqxyxq.DataSource = Monthlist;
            ddlMonthJqxyxq.DataTextField = "Key";
            ddlMonthJqxyxq.DataValueField = "Value";
            ddlMonthJqxyxq.DataBind();

            ddlYearSyxyxq.DataSource = Yearlistsyxyxq;
            ddlYearSyxyxq.DataTextField = "Key";
            ddlYearSyxyxq.DataValueField = "Value";
            ddlYearSyxyxq.DataBind();

            ddlMonthSyxyxq.DataSource = Monthlist;
            ddlMonthSyxyxq.DataTextField = "Key";
            ddlMonthSyxyxq.DataValueField = "Value";
            ddlMonthSyxyxq.DataBind();

            ddlPicType.DataSource = PicTypeList;
            ddlPicType.DataTextField = "Name";
            ddlPicType.DataValueField = "Name";
            ddlPicType.DataBind();
            ddlPicType.Items.Insert(0, new ListItem("-选择类型-", ""));
        }

        private void BindData()
        {
            int id = GetInt("id");
            if (id > 0)
            {
                JcbCarInfo entity = JcbCars.Instance.GetModel(id, true);
                txtVINCode.Value = entity.VINCode;
                SetSelectedByText(ddlcChangs, entity.cChangs);
                ddlcChangs_SelectedIndexChanged(null, null);
                SetSelectedByText(ddlcCxmc, entity.cCxmc);
                ddlcCxmc_SelectedIndexChanged(null, null);
                txtPailiang.Text = entity.Pailiang;
                cbxIswlzy.Checked = entity.Iswlzy;
                SetSelectedByText(rblBsx, entity.Bsx);
                SetSelectedByText(ddlPfbz, entity.Pfbz);
                cbxIsobd.Checked = entity.Isobd;
                if (txtWgys.Visible)
                    txtWgys.Text = entity.Wgys;
                else
                    SetSelectedByText(ddlWgys, entity.Wgys);
                SetSelectedByText(ddlNsys, entity.Nsys);
                if (string.IsNullOrEmpty(entity.Cardeploydd)
                    || string.IsNullOrEmpty(entity.Cardeployzy)
                    || string.IsNullOrEmpty(entity.Cardeployyx)
                    || string.IsNullOrEmpty(entity.Cardeployaqqn)
                    || string.IsNullOrEmpty(entity.Cardeployqt))
                {
                    btnCardeploy.InnerHtml = "车辆配置-";
                    IsCardeploy = true;
                }
                SetMultSelectedByText(cblCardeploydd, entity.Cardeploydd, ",");
                SetMultSelectedByText(cblCardeployzy, entity.Cardeployzy, ",");
                SetMultSelectedByText(cblCardeployyx, entity.Cardeployyx, ",");
                SetMultSelectedByText(cblCardeployaqqn, entity.Cardeployaqqn, ",");
                SetMultSelectedByText(cblCardeployqt, entity.Cardeployqt, ",");
                txtBxlc.Text = entity.Bxlc;
                SetSelectedByText(rblSfysc, entity.Sfysc);
                SetSelectedByText(ddlCarPromary, entity.CarPromary);
                ddlCarPromary_SelectedIndexChanged(null,null);
                SetSelectedByText(ddlCarCity, entity.CarCity);
                SetSelectedByText(ddlSyxz, entity.Syxz);
                SetSelectedByText(rblYwzdsg, entity.Ywzdsg);
                SetSelectedByText(rblWgcs, entity.Wgcs);
                SetSelectedByText(rblNszt, entity.Nszt);
                SetSelectedByText(ddlYbtzt, entity.Ybtzt);
                SetSelectedByText(ddlZysyqk, entity.Zysyqk);
                SetSelectedByText(rblDqby, entity.Dqby);
                SetSelectedByText(rblWxbyjl, entity.Wxbyjl);
                if (!string.IsNullOrEmpty(entity.Yjhgg))
                {
                    txtYjhgg.Text = entity.Yjhgg;
                    txtYjhgg.CssClass = "srk1";
                }
                txtZsyh.Text = entity.Zsyh;
                txtByfy.Text = entity.Byfy;
                SetSelectedByText(rblZjsfqq, entity.Zjsfqq);
                SetSelectedByText(rblXsz, entity.Xsz);
                if (entity.Xsz == "有")
                {
                    trXsz.Attributes["class"] = "";
                    if (!string.IsNullOrEmpty(entity.XszPic))
                    {
                        imgxsz.Src = entity.XszPic;
                        hdnPicxsz.Value = entity.XszPic;
                    }
                }
                SetSelectedByText(rblDjz, entity.Djz);
                if (entity.Djz == "有")
                {
                    trDjz.Attributes["class"] = "";
                    if (!string.IsNullOrEmpty(entity.DjzPic))
                    {
                        imgdjz.Src = entity.DjzPic;
                        hdnPicdjz.Value = entity.XszPic;
                    }
                }
                SetSelectedByText(rblGcfp, entity.Gcfp);
                if (entity.Gcfp == "有")
                {
                    trGcfp.Attributes["class"] = "";
                    if (!string.IsNullOrEmpty(entity.GcfpPic))
                    {
                        imggcfp.Src = entity.GcfpPic;
                        hdnPicgcfp.Value = entity.GcfpPic;
                    }
                }
                SetSelectedByText(rblGzs, entity.Gzs);
                SetSelectedByText(rblSfkywq, entity.Sfkywq);
                DateTime scsprq = DateTime.Today;
                if (DateTime.TryParse(entity.Scsprq, out scsprq))
                {
                    SetSelectedByText(ddlYearScsprq, scsprq.Year.ToString());
                    SetSelectedByText(ddlMonthScsprq, scsprq.Month.ToString());
                }
                cbxIsWsp.Checked = entity.Iswsp;
                DateTime njyxq = DateTime.Today;
                if (DateTime.TryParse(entity.Njyxq, out njyxq))
                {
                    SetSelectedByText(ddlYearNjyxq, njyxq.Year.ToString());
                    SetSelectedByText(ddlMonthNjyxq, njyxq.Month.ToString());
                }
                cbxIsNjygq.Checked = entity.Isnjygq;
                DateTime jqxyxq = DateTime.Today;
                if (DateTime.TryParse(entity.Jqxyxq, out jqxyxq))
                {
                    SetSelectedByText(ddlYearJqxyxq, jqxyxq.Year.ToString());
                    SetSelectedByText(ddlMonthJqxyxq, jqxyxq.Month.ToString());
                }
                cbxIsJqxygq.Checked = entity.Isjqxygq;
                DateTime syxyxq = DateTime.Today;
                if (DateTime.TryParse(entity.Syxyxq, out syxyxq))
                {
                    SetSelectedByText(ddlYearSyxyxq, syxyxq.Year.ToString());
                    SetSelectedByText(ddlMonthSyxyxq, syxyxq.Month.ToString());
                }
                cbxIsSyxygq.Checked = entity.Issyxygq;
                txtYsj.Text = entity.Ysj.ToString();
                cbxIsYsjbhghf.Checked = entity.IsYsjbhghf;
                cbxIsYsjykj.Checked = entity.IsYsjykj;
                cbxIsYsjkaj.Checked = entity.IsYsjkaj;
                SetSelectedByText(rblShzb, entity.Shzb);
                if (entity.Shzb == "商家延保")
                {
                    trShzb.Attributes["class"] = "";
                    txtSjybnry.Text = entity.Sjybnry;
                    txtSjybnrgl.Text = entity.Sjybnrgl;
                }
                txtCdjg.Text = entity.Cdjg.ToString();
                cbxIsCdjgkyj.Checked = entity.IsCdjgkyj;
                txtCzzs.Text = entity.Czzs;
                rptPics.DataSource = entity.Picslist;
                rptPics.DataBind();
            }
        }

        protected void ddlcChangs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlcChangs.SelectedIndex == 0)
            {
                ddlcCxmc.Items.Clear();
                ddlcCxmc.Items.Add(new ListItem("-选择车型-", "-1"));
                ddlWgys.Items.Clear();
                ddlWgys.Items.Add(new ListItem("-选择颜色-", ""));
                return;
            }

            string cChangs = ddlcChangs.SelectedItem.Value;
            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(cChangs, true);

            ddlcCxmc.DataSource = carlist;
            ddlcCxmc.DataTextField = "cCxmc";
            ddlcCxmc.DataValueField = "id";
            ddlcCxmc.DataBind();
            ddlcCxmc.SelectedIndex = 0;
            ddlcCxmc_SelectedIndexChanged(null, null);
        }

        protected void ddlcCxmc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentCar != null)
            {
                double pl = Math.Round(DataConvert.SafeDouble(CurrentCar.fPail) / 1000, 1);
                txtPailiang.Text = Math.Round(pl, 1).ToString();
                cbxIswlzy.Checked = CurrentCar.cJqxs == "涡轮增压";
                SetSelectedByValue(rblBsx, CurrentCar.cBsxlx.IndexOf("手动") >= 0 ? "手动" : "自动");
                string pfbz = CurrentCar.cHbbz.Replace("III", "Ⅲ").Replace("II", "Ⅱ").Replace("IV", "Ⅳ").Replace("V", "Ⅴ").Replace("I", "Ⅰ");
                SetSelectedByValue(ddlPfbz, pfbz.Substring(0, CurrentCar.cHbbz.Length > 2 ? 2 : CurrentCar.cHbbz.Length));
                cbxIsobd.Checked = pfbz.IndexOf("OBD") > 0;

                #region 汽车颜色

                DataTable t = new DataTable();
                t.Columns.Add("Name");
                t.Columns.Add("Color");

                foreach (string colorinfo in CurrentCar.cQcys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] color = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (color.Length == 2)
                    {
                        DataRow row = t.NewRow();
                        row["Name"] = color[0];
                        row["Color"] = color[1];
                        t.Rows.Add(row);
                    }
                }

                if (t.Rows.Count > 0)
                {
                    ddlWgys.DataSource = t;
                    ddlWgys.DataTextField = "Name";
                    ddlWgys.DataValueField = "Name";
                    ddlWgys.DataBind();
                    ddlWgys.Items.Insert(0, new ListItem("-选择颜色-", ""));
                    ddlWgys.Visible = true;
                    txtWgys.Visible = false;
                }
                else
                {
                    ddlWgys.Visible = false;
                    txtWgys.Visible = true;
                }

                #endregion

                if (btnCardeploy.InnerHtml == "车辆配置-")
                {
                    trCardploydd1.Attributes["class"] = "cardeploy";
                    trCardploydd2.Attributes["class"] = "cardeploy";
                    trCardeployzy1.Attributes["class"] = "cardeploy";
                    trCardeployzy2.Attributes["class"] = "cardeploy";
                    trCardeployyx1.Attributes["class"] = "cardeploy";
                    trCardeployyx2.Attributes["class"] = "cardeploy";
                    trCardeployaqqn1.Attributes["class"] = "cardeploy";
                    trCardeployaqqn2.Attributes["class"] = "cardeploy";
                    trCardeployqt1.Attributes["class"] = "cardeploy";
                    trCardeployqt2.Attributes["class"] = "cardeploy";
                }
            }
        }

        protected void ddlCarPromary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCarPromary.SelectedIndex == 0)
            {
                ddlCarCity.Items.Clear();
                ddlCarCity.Items.Add(new ListItem("-选择城市-", "-1"));
                return;
            }

            List<CityInfo> citylist = Areas.Instance.GetCityList(true);
            ddlCarCity.DataSource = citylist.FindAll(c => c.PID == DataConvert.SafeInt(ddlCarPromary.SelectedValue));
            ddlCarCity.DataTextField = "Name";
            ddlCarCity.DataValueField = "ID";
            ddlCarCity.DataBind();
            ddlCarCity.Items.Insert(0, new ListItem("-选择城市-", "-1"));
            ddlCarCity.SelectedIndex = 0;
        }

        protected void rptPics_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                JcbcarpicInfo entity = (JcbcarpicInfo)e.Item.DataItem;
                DropDownList ddlPicType = (DropDownList)e.Item.FindControl("ddlPicType");
                HtmlInputRadioButton rbIsFirstpic = (HtmlInputRadioButton)e.Item.FindControl("rbIsFirstpic");
                HtmlInputHidden hdnPicUrl = (HtmlInputHidden)e.Item.FindControl("hdnPicUrl");
                HtmlInputHidden hdnIsFirstpic = (HtmlInputHidden)e.Item.FindControl("hdnIsFirstpic");
                if (ddlPicType != null)
                {
                    ddlPicType.DataSource = PicTypeList;
                    ddlPicType.DataTextField = "Name";
                    ddlPicType.DataValueField = "Name";
                    ddlPicType.DataBind();
                    ddlPicType.Items.Insert(0, new ListItem("-选择类型-", ""));

                    SetSelectedByText(ddlPicType, entity.JcbPicType);
                }
                if (rbIsFirstpic != null)
                {
                    rbIsFirstpic.Checked = entity.IsFirstpic;
                    hdnIsFirstpic.Value = entity.IsFirstpic ? "1" : "0";
                }
                if (hdnPicUrl != null)
                    hdnPicUrl.Value = entity.PicUrl;
            }
        }

        private void FillData(JcbCarInfo entity)
        {
            entity.VINCode = txtVINCode.Value;
            entity.cChangs = ddlcChangs.SelectedItem.Text;
            entity.cCxmc = ddlcCxmc.SelectedItem.Text;
            entity.Pailiang = txtPailiang.Text;
            entity.Iswlzy = cbxIswlzy.Checked;
            entity.Bsx = rblBsx.SelectedValue;
            entity.Pfbz = ddlPfbz.SelectedValue;
            entity.Isobd = cbxIsobd.Checked;
            entity.Wgys = ddlWgys.Visible ? ddlWgys.SelectedValue : txtWgys.Text;
            entity.Nsys = ddlNsys.SelectedValue;
            entity.Cardeploydd = GetMultSeletctedText(cblCardeploydd, ",");
            entity.Cardeployzy = GetMultSeletctedText(cblCardeployzy, ",");
            entity.Cardeployyx = GetMultSeletctedText(cblCardeployyx, ",");
            entity.Cardeployaqqn = GetMultSeletctedText(cblCardeployaqqn, ",");
            entity.Cardeployqt = GetMultSeletctedText(cblCardeployqt, ",");
            entity.Bxlc = txtBxlc.Text;
            entity.Sfysc = rblSfysc.SelectedValue;
            entity.CarPromary = ddlCarPromary.SelectedItem.Text;
            entity.CarCity = ddlCarCity.SelectedItem.Text;
            entity.Syxz = ddlSyxz.SelectedValue;
            entity.Ywzdsg = rblYwzdsg.SelectedValue;
            entity.Wgcs = rblWgcs.SelectedValue;
            entity.Nszt = rblNszt.SelectedValue;
            entity.Ybtzt = ddlYbtzt.SelectedValue;
            entity.Zysyqk = ddlZysyqk.SelectedValue;
            entity.Dqby = rblDqby.SelectedValue;
            entity.Wxbyjl = rblWxbyjl.SelectedValue;
            entity.Yjhgg = txtYjhgg.Text == "车况好，无事故，手续全，原车原图" ? string.Empty : txtYjhgg.Text;
            entity.Zsyh = txtZsyh.Text;
            entity.Byfy = txtByfy.Text;
            entity.Cph = txtCph.Text;
            entity.Zjsfqq = rblZjsfqq.SelectedValue;
            entity.Xsz = rblXsz.SelectedValue;
            entity.XszPic = hdnPicxsz.Value;
            entity.Djz = rblDjz.SelectedValue;
            entity.DjzPic = hdnPicdjz.Value;
            entity.Gcfp = rblGcfp.SelectedValue;
            entity.GcfpPic = hdnPicgcfp.Value;
            entity.Gzs = rblGzs.SelectedValue;
            entity.Sfkywq = rblSfkywq.SelectedValue;
            entity.Scsprq = DataConvert.SafeDate(ddlYearScsprq.SelectedValue + "-" + ddlMonthScsprq.SelectedValue + "-01").ToShortDateString();
            entity.Iswsp = cbxIsWsp.Checked;
            entity.Njyxq = DataConvert.SafeDate(ddlYearNjyxq.SelectedValue + "-" + ddlMonthNjyxq.SelectedValue + "-01").ToShortDateString();
            entity.Isnjygq = cbxIsNjygq.Checked;
            entity.Jqxyxq = DataConvert.SafeDate(ddlYearJqxyxq.SelectedValue + "-" + ddlMonthJqxyxq.SelectedValue + "-01").ToShortDateString();
            entity.Isjqxygq = cbxIsJqxygq.Checked;
            entity.Syxyxq = DataConvert.SafeDate(ddlYearSyxyxq.SelectedValue + "-" + ddlMonthSyxyxq.SelectedValue + "-01").ToShortDateString();
            entity.Issyxygq = cbxIsSyxygq.Checked;
            entity.Ysj = DataConvert.SafeDecimal(txtYsj.Text);
            entity.IsYsjbhghf = cbxIsYsjbhghf.Checked;
            entity.IsYsjykj = cbxIsYsjykj.Checked;
            entity.IsYsjkaj = cbxIsYsjkaj.Checked;
            entity.Shzb = rblShzb.SelectedValue;
            entity.Sjybnry = txtSjybnry.Text;
            entity.Sjybnrgl = txtSjybnrgl.Text;
            entity.Cdjg = DataConvert.SafeDecimal(txtCdjg.Text);
            entity.IsCdjgkyj = cbxIsCdjgkyj.Checked;
            entity.Czzs = txtCzzs.Text;
            entity.LastUpdateTime = DateTime.Now;

            #region 车辆图片
            
            List<JcbcarpicInfo> pics = new List<JcbcarpicInfo>();
            int addCount = DataConvert.SafeInt(hdnPicAddCount.Value);
            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string picurl = Request["hdnPicUrl" + i];
                    string pictype = Request["ddlPicType" + i];
                    bool isfirstpic = DataConvert.SafeInt(Request["hdnIsFirstpic" + i]) == 1;
                    if (!string.IsNullOrEmpty(picurl))
                    {
                        JcbcarpicInfo picinfo = new JcbcarpicInfo
                        {
                            PicUrl = picurl,
                            JcbPicType = pictype,
                            IsFirstpic = isfirstpic
                        };
                        pics.Add(picinfo);
                    }
                }
            }
            foreach (RepeaterItem item in rptPics.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {

                    DropDownList ddlPicType = (DropDownList)item.FindControl("ddlPicType");
                    HtmlInputRadioButton rbIsFirstpic = (HtmlInputRadioButton)item.FindControl("rbIsFirstpic");
                    HtmlInputHidden hdnPicUrl = (HtmlInputHidden)item.FindControl("hdnPicUrl");
                    if (hdnPicUrl != null && !string.IsNullOrEmpty(hdnPicUrl.Value))
                    {
                        JcbcarpicInfo picinfo = new JcbcarpicInfo
                        {
                            PicUrl = hdnPicUrl.Value,
                            JcbPicType = ddlPicType.SelectedValue,
                            IsFirstpic = rbIsFirstpic.Checked
                        };
                        pics.Add(picinfo);
                    }
                }
            }

            entity.Pics = json.Serialize(pics);
            #endregion
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            JcbCarInfo entity = new JcbCarInfo();
            int id = GetInt("id");
            if (id > 0)
                entity = JcbCars.Instance.GetModel(id, true);
            FillData(entity);

            JcbCars.Instance.CreateAndUpdate(entity);

            JcbCars.Instance.ReloadListCache();
        }
    }
}