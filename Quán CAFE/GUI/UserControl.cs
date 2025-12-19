using Quán_CAFE.BUS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quán_CAFE
{
    public class DashboardControl : UserControl
    {
        private Form1 _main;
        private FlowLayoutPanel flow;

        public DashboardControl(Form1 main)
        {
            _main = main;
            flow = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), AutoScroll = true };
            this.Controls.Add(flow);
            LoadTables();
        }

        private void LoadTables()
        {
            flow.Controls.Clear();
            try
            {
                DataTable dt = TableBUS.LoadTableList();
                foreach (DataRow row in dt.Rows)
                {
                    int id = (int)row["TableID"];
                    string name = row["TableName"].ToString();
                    string status = row["Status"].ToString();
                    Color bgColor = status == "Có khách" ? Form1.PinkSecondary : (status == "Chờ dọn" ? Form1.OrangeWait : Color.White);

                    Button btn = new Button { Text = name + "\n(" + status + ")", Size = new Size(130, 130), Margin = new Padding(15), BackColor = bgColor, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Tag = id };
                    btn.Click += (s, e) => _main.SwitchView(new OrderControl(_main, id, name));
                    flow.Controls.Add(btn);
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message); }
        }
    }
}