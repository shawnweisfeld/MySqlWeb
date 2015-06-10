using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using Microsoft.Reporting.WebForms;


namespace MySqlWeb
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DropDownList1.Items.Clear();
                DropDownList1.Items.AddRange(GetPosts()
                    .Tables["post_ids"].Rows.Cast<DataRow>()
                    .Select(x => new ListItem(x["post_id"].ToString(), x["post_id"].ToString())).ToArray());
                DropDownList1.SelectedIndex = 0;
            }
        }


        private DataSet GetPosts()
        {
            var sql = "SELECT DISTINCT post_id FROM wp_postmeta";
            var ds = new PostsDataSet();

            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(ds, "post_ids");
                }
            }

            return ds;
        }

        private DataSet GetDetails(int postID)
        {
            var sql = "SELECT meta_id, post_id, meta_key, meta_value FROM wp_postmeta WHERE post_id = @post_id";
            var ds = new PostsDataSet();

            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@post_id", postID);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "wp_postmeta");
                    }
                }
            }

            return ds;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var ds = GetDetails(int.Parse(DropDownList1.SelectedValue));
            string path = HttpContext.Current.Server.MapPath("~");
            ReportViewer1.Visible = true;
            ReportViewer1.LocalReport.ReportPath = path + "MyReport" + ".rdlc";
            ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables["wp_postmeta"]);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(datasource);
            ReportViewer1.LocalReport.Refresh();
        }
    }
}