using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Tools;
using Hx.Components;
using Hx.Components.Entity;

namespace Hx.BackAdmin.biz
{
    public partial class personaldatamg : AdminBase
    {
        protected override void Check()
        {
        }

        private int uid = 0;
        protected int UID
        {
            get
            {
                if (uid == 0)
                {
                    uid = GetInt("uid");
                }
                return uid;
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
            int userid = GetInt("id");
            if (userid > 0)
            {
                List<PersonaldataInfo> list = DayReportUsers.Instance.GetPersonaldataList(true);
                list = list.FindAll(p=>p.UserID == userid).OrderBy(l => l.ID).ToList();

                rptData.DataSource = list;
                rptData.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int userid = GetInt("id");
            if (userid > 0)
            {
                string delIds = hdnDelIds.Value;
                if (!string.IsNullOrEmpty(delIds))
                {
                    DayReportUsers.Instance.DeletePersonaldata(delIds);
                }

                int addCount = DataConvert.SafeInt(hdnAddCount.Value);

                if (addCount > 0)
                {
                    for (int i = 1; i <= addCount; i++)
                    {
                        string name = Request["txtName" + i];
                        string filepath = Request["hdnFilepath" + i];
                        if (!string.IsNullOrEmpty(name))
                        {
                            PersonaldataInfo entity = new PersonaldataInfo
                            {
                                UserID = userid,
                                Name = name,
                                Filepath = filepath
                            };
                            DayReportUsers.Instance.AddPersonaldata(entity);
                        }
                    }
                }

                foreach (RepeaterItem item in rptData.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        System.Web.UI.HtmlControls.HtmlInputText txtName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtName");
                        System.Web.UI.HtmlControls.HtmlInputHidden hdnFilepath = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnFilepath");
                        System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                        if (hdnID != null)
                        {
                            int id = DataConvert.SafeInt(hdnID.Value);
                            if (id > 0)
                            {
                                PersonaldataInfo entity = DayReportUsers.Instance.GetPersonaldata(id, true);
                                if (entity != null)
                                {
                                    entity.Name = txtName.Value;
                                    entity.Filepath = hdnFilepath.Value;
                                    DayReportUsers.Instance.UpdatePersonaldata(entity);
                                }
                            }
                        }
                    }
                }

                DayReportUsers.Instance.ReloadPersonaldataListCache();

                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? string.Format("~/biz/personaldatamg.aspx?id={0}&uid={1}", GetInt("id"), GetInt("uid")) : FromUrl);
            }
            else
            {
                WriteErrorMessage("错误提示！", "非法用户！", string.IsNullOrEmpty(FromUrl) ? string.Format("~/biz/personaldatamg.aspx?id={0}&uid={1}", GetInt("id"), GetInt("uid")) : FromUrl);

            }
        }
    }
}