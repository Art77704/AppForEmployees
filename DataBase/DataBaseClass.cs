using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace AppForEmployees.DataBase
{
    class DataBaseClass
    {
        //public static SqlConnection connectionOpen

        static string connectionString = "data source=DESKTOP-RETOSMJ;initial catalog=RCC;integrated security=True;trustservercertificate=True";

        public static SqlConnection Connection()
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            return con;
        }
        public static SqlCommand connectionOpen(string querry)
        {
            var connection = Connection();
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
        public static string[] GetData(string querry)
        {
            string[] data = new string[5];
            int countdata = 0;
            var cmd = connectionOpen(querry);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                countdata++;
            }

            cmd = connectionOpen(querry);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < countdata; i++)
                {
                    data[i] = reader.GetValue(i).ToString();
                }
            }
            return data.ToArray();
        }

        public static void AddEditDel(string querry)
        {
            var cmd = connectionOpen(querry);
            cmd.ExecuteNonQuery();
        }

        public static void ShowOrUpdateDT(string querry, DataGrid datagr)
        {

            var cmd = connectionOpen(querry);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            datagr.ItemsSource = dt.DefaultView;
        }
    }
}
