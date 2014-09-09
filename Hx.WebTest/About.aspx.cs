using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
        }
    }

    private void LoadData()
    {
        DataTable tbRooms = new DataTable();
        tbRooms.Columns.Add("ID");
        tbRooms.Columns.Add("Name");
        string _con = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        string sql = "SELECT * FROM dbo.Rooms";
        using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
        {
            while (reader.Read())
            {
                DataRow row = tbRooms.NewRow();
                row["ID"] = (int)reader["ID"];
                row["Name"] = reader["Name"].ToString();
                tbRooms.Rows.Add(row);
            }
        }

        rptRooms.DataSource = tbRooms;
        rptRooms.DataBind();
    }
}
