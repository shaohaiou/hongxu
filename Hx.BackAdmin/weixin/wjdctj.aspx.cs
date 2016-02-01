using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components;
using Hx.Components.Entity;
using NPOI.SS.UserModel;
using Hx.Tools;
using System.IO;
using NPOI.XSSF.UserModel;
using System.Text;
using NPOI.HSSF.UserModel;

namespace Hx.BackAdmin.weixin
{
    public partial class wjdctj : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            string fileName = Utils.GetMapPath(@"\App_Data\2015年度总经理满意度（汇总表）.xls");
            string newfilename = "2015年度总经理满意度（汇总表）.xls";
            using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(file);
            }
            sheet = workbook.GetSheetAt(0);

            List<QuestionRecordInfo> listRecord = WeixinActs.Instance.GetQuestionRecordList();
            List<QuestionCompanyInfo> listCompany = WeixinActs.Instance.GetQuestionCompanyList().OrderBy(l => l.Index).ToList();
            List<QuestionInfo> listQuestion = WeixinActs.Instance.GetQuestionList();
            for (int i = 0; i < listCompany.Count; i++)
            {
                if (listRecord.Exists(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.主管))
                {
                    int num = listRecord.FindAll(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.主管).Count;
                    decimal sum = listRecord.FindAll(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.主管).Sum(l => l.QuestionScoreList.Sum(s => s.Score));
                    sheet.GetRow(2).Cells[3 * i + 1].SetCellValue(num);
                    sheet.GetRow(2).Cells[3 * i + 2].SetCellValue(sum.ToString());
                    sheet.GetRow(2).Cells[3 * i + 3].SetCellValue(Math.Round(sum / num, 1).ToString());

                    List<KeyValuePair<string, decimal>> listquestionscoresum = new List<KeyValuePair<string, decimal>>();
                    foreach (QuestionInfo question in listQuestion.FindAll(l=>l.QuestionType == Components.Enumerations.QuestionType.主管))
                    {
                        listquestionscoresum.Add(new KeyValuePair<string, decimal>(question.QuestionFacor, listRecord.FindAll(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.主管).Sum(l => l.QuestionScoreList.Find(s => s.QuestionID == question.ID).Score)));
                    }
                    listquestionscoresum = listquestionscoresum.OrderBy(l => l.Value).ToList();

                    sheet.GetRow(4).Cells[3 * i + 1].SetCellValue(listquestionscoresum[0].Key + "：" + Math.Round(listquestionscoresum[0].Value, 0).ToString());
                    sheet.GetRow(4).Cells[3 * i + 2].SetCellValue(listquestionscoresum[1].Key + "：" + Math.Round(listquestionscoresum[1].Value, 0).ToString());
                    sheet.GetRow(4).Cells[3 * i + 3].SetCellValue(listquestionscoresum[2].Key + "：" + Math.Round(listquestionscoresum[2].Value, 0).ToString());
                }
                if (listRecord.Exists(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.普通员工))
                {
                    int num = listRecord.FindAll(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.普通员工).Count;
                    decimal sum = listRecord.FindAll(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.普通员工).Sum(l => l.QuestionScoreList.Sum(s => s.Score));
                    sheet.GetRow(3).Cells[3 * i + 1].SetCellValue(num);
                    sheet.GetRow(3).Cells[3 * i + 2].SetCellValue(sum.ToString());
                    sheet.GetRow(3).Cells[3 * i + 3].SetCellValue(Math.Round(sum / num, 1).ToString());

                    List<KeyValuePair<string, decimal>> listquestionscoresum = new List<KeyValuePair<string, decimal>>();
                    foreach (QuestionInfo question in listQuestion.FindAll(l => l.QuestionType == Components.Enumerations.QuestionType.普通员工))
                    {
                        listquestionscoresum.Add(new KeyValuePair<string, decimal>(question.QuestionFacor, listRecord.FindAll(l => l.QuestionCompanyID == listCompany[i].ID && l.QuestionType == Components.Enumerations.QuestionType.普通员工).Sum(l => l.QuestionScoreList.Find(s => s.QuestionID == question.ID).Score)));                  
                    }
                    listquestionscoresum = listquestionscoresum.OrderBy(l => l.Value).ToList();

                    sheet.GetRow(5).Cells[3 * i + 1].SetCellValue(listquestionscoresum[0].Key + "：" + Math.Round(listquestionscoresum[0].Value,0).ToString());
                    sheet.GetRow(5).Cells[3 * i + 2].SetCellValue(listquestionscoresum[1].Key + "：" + Math.Round(listquestionscoresum[1].Value, 0).ToString());
                    sheet.GetRow(5).Cells[3 * i + 3].SetCellValue(listquestionscoresum[2].Key + "：" + Math.Round(listquestionscoresum[2].Value, 0).ToString());
                }
            }


            sheet.ForceFormulaRecalculation = true;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                workbook.Write(ms);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfilename, Encoding.UTF8).ToString() + "");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                workbook = null;
            }
        }
    }
}