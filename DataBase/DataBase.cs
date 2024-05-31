using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AppForEmployees.DataBase
{
    static class DataBase
    {
        public static SqlCommand connectionOpen(string querry)
        {
            SqlConnection connection =
               new SqlConnection("data source=ARTEM-PK;initial catalog=RCC;integrated security=True;trustservercertificate=True");
            connection.Open();
            SqlCommand cmd = new SqlCommand(querry, connection);
            return cmd;
        }
        public static void AddToCMB(string querry, ComboBox cmb)
        {
            var cmd = connectionOpen(querry);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var NameCmb = reader.GetValue(0).ToString();

                cmb.Items.Add($"{NameCmb}");
            }
        }
        public static void AddEditDel(string querry)
        {
            var cmd = connectionOpen(querry);
            cmd.ExecuteNonQuery();
        }

        public static void ShowOrUpdateDT(string querry, DataGrid datagr)
        {
            var con = connectionOpen(querry);

            SqlCommand cmd = new SqlCommand(querry, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            datagr.ItemsSource = dt.DefaultView;
        }
    }
}
