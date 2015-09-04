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
using System.Text;
using Hx.Tools;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;

namespace Hx.BackAdmin.weixin
{
    public partial class scenecodestatall : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.微信活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }

        }

        protected string Count
        {
            get
            {
                int count = 0;
                List<ScenecodeSettingInfo> settinglist = WeixinActs.Instance.GetScenecodeSettingList(true);
                foreach (ScenecodeSettingInfo setting in settinglist)
                {
                    List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(setting.ID, true);
                    count += list.Sum(l => l.ScanNum);
                }

                return count.ToString();
            }
        }

        protected string TblStr
        {
            get
            {
                StringBuilder strb = new StringBuilder();
                List<ScenecodeSettingInfo> settinglist = WeixinActs.Instance.GetScenecodeSettingList(true);
                for (int k = 0; k < settinglist.Count; k++)
                {
                    List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(settinglist[k].ID, true);

                    if (list.Count > 0)
                    {
                        strb.AppendLine(string.Format("<span style=\"font-weight:bold;\">{0}</span><br>", settinglist[k].Name));
                        strb.AppendLine("<table style=\"border-spacing: 0;\">");
                        strb.AppendLine("<tr style=\"background:#ccc;font-weight:bold;\">");
                        for (int i = 0; i < list.Count; i++)
                        {
                            strb.AppendLine("<td class=\"w80\">" + list[i].SceneName + "</td>");
                        }
                        strb.AppendLine("</tr>");
                        strb.AppendLine("<tr>");
                        for (int i = 0; i < list.Count; i++)
                        {
                            strb.AppendLine("<td class=\"w80\">" + list[i].ScanNum + "</td>");
                        }
                        strb.AppendLine("</tr>");

                        strb.AppendLine("</table>");
                    }
                }

                return strb.ToString();
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
            rpcg.DataSource = WeixinActs.Instance.GetScenecodeSettingList(true);
            rpcg.DataBind();
        }

        protected string SetScenecodeSettingStatus(string id)
        {
            string result = string.Empty;

            if (!Admin.Administrator)
            {
                ScenecodeSettingInfo setting = WeixinActs.Instance.GetScenecodeSetting(DataConvert.SafeInt(id), true);

                if (setting != null)
                {
                    string[] powerusers = setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!powerusers.Contains(AdminID.ToString()))
                        result = "style=\"display:none;\"";
                }
            }

            return result;
        }

        #region 导出Excel

        static HSSFWorkbook hssfworkbook;

        public void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();
            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "红旭集团";
            hssfworkbook.DocumentSummaryInformation = dsi;
            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "xxx";
            hssfworkbook.SummaryInformation = si;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            InitializeWorkbook();
            HSSFSheet sheet1 = (HSSFSheet)hssfworkbook.CreateSheet("Sheet1");
            HSSFRow rowFirst = (HSSFRow)sheet1.CreateRow(0);
            rowFirst.CreateCell(0).SetCellValue("总数：" + Count.ToString());
            List<ScenecodeSettingInfo> settinglist = WeixinActs.Instance.GetScenecodeSettingList(true);
            int hasvalue = 0;
            for (int k = 0; k < settinglist.Count; k++)
            {
                List<ScenecodeInfo> list = WeixinActs.Instance.GetScenecodeList(settinglist[k].ID, true);
                if (list.Count > 0)
                {
                    ICellStyle cellStyleTop = hssfworkbook.CreateCellStyle();
                    IFont fontTop = hssfworkbook.CreateFont();
                    fontTop.Boldweight = (short)FontBoldWeight.Bold;
                    cellStyleTop.SetFont(fontTop);
                    HSSFRow rowTop = (HSSFRow)sheet1.CreateRow(hasvalue * 4 + 1);
                    rowTop.CreateCell(0).SetCellValue(settinglist[k].Name);
                    rowTop.GetCell(0).CellStyle = cellStyleTop;

                    ICellStyle cellStyleHeader = hssfworkbook.CreateCellStyle();
                    IFont fontHeader = hssfworkbook.CreateFont();
                    fontHeader.Boldweight = (short)FontBoldWeight.Bold;
                    cellStyleHeader.SetFont(fontHeader);
                    cellStyleHeader.FillForegroundColor = HSSFColor.BlueGrey.Index;
                    cellStyleHeader.FillPattern = FillPattern.SolidForeground;
                    cellStyleHeader.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleHeader.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleHeader.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleHeader.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleHeader.TopBorderColor = HSSFColor.Black.Index;
                    cellStyleHeader.RightBorderColor = HSSFColor.Black.Index;
                    cellStyleHeader.BottomBorderColor = HSSFColor.Black.Index;
                    cellStyleHeader.LeftBorderColor = HSSFColor.Black.Index;
                    HSSFRow rowHeader = (HSSFRow)sheet1.CreateRow(hasvalue * 4 + 2);
                    for (int i = 0; i < list.Count; i++)
                    {
                        rowHeader.CreateCell(i).SetCellValue(list[i].SceneName);
                        rowHeader.GetCell(i).CellStyle = cellStyleHeader;
                    }

                    ICellStyle cellStyleValue = hssfworkbook.CreateCellStyle();
                    cellStyleValue.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleValue.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleValue.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleValue.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyleValue.TopBorderColor = HSSFColor.Black.Index;
                    cellStyleValue.RightBorderColor = HSSFColor.Black.Index;
                    cellStyleValue.BottomBorderColor = HSSFColor.Black.Index;
                    cellStyleValue.LeftBorderColor = HSSFColor.Black.Index;
                    HSSFRow rowValue = (HSSFRow)sheet1.CreateRow(hasvalue * 4 + 3);
                    for (int i = 0; i < list.Count; i++)
                    {
                        rowValue.CreateCell(i).SetCellValue(list[i].ScanNum);
                        rowValue.GetCell(i).CellStyle = cellStyleValue;
                    }
                    hasvalue++;
                }
            }

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                hssfworkbook.Write(ms);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("微信场景二维码数据统计", Encoding.UTF8).ToString() + ".xls");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                hssfworkbook = null;
            }
        }

        #endregion
    }
}