using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quán_CAFE.BUS
{
    public class DataProvider
    {

        private static DataProvider instance;
        public static DataProvider Instance
        {
            get { if (instance == null) instance = new DataProvider(); return instance; }
        }

        // LƯU Ý: Thay đổi tên Server SQL của bạn tại đây (ví dụ: .\SQLEXPRESS)
        private string connectionSTR = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\dell\OneDrive\ドキュメント\CAFE NAVY.mdf"";Integrated Security=True;Connect Timeout=30";

        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                        if (item.Contains('@')) { command.Parameters.AddWithValue(item, parameter[i]); i++; }
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);
            }
            return data;
        }

        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                        if (item.Contains('@')) { command.Parameters.AddWithValue(item, parameter[i]); i++; }
                }
                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string query, object[] parameter = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                        if (item.Contains('@')) { command.Parameters.AddWithValue(item, parameter[i]); i++; }
                }
                return command.ExecuteScalar();
            }
        }
    }
}