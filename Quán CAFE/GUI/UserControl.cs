using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quán_CAFE
{
    public class DashboardControl : UserControl
    {
        public DashboardControl(Form1 main)
        {
            FlowLayoutPanel flow = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), AutoScroll = true };
            for (int i = 1; i <= 16; i++)
            {
                string tName = "Bàn " + i;
                bool isFull = Form1.OccupiedTables.Contains(tName);
                bool isPaid = Form1.PaidTables.Contains(tName);

                string statusText = "Trống";
                Color bgColor = Color.White;
                Color textColor = Form1.NavyPrimary;

                if (isPaid)
                {
                    statusText = "Chờ dọn";
                    bgColor = Form1.OrangeWait;
                    textColor = Color.White;
                }
                else if (isFull)
                {
                    statusText = "Có khách";
                    bgColor = Form1.PinkSecondary;
                    textColor = Form1.NavyPrimary;
                }

                Button btn = new Button
                {
                    Text = tName + "\n(" + statusText + ")",
                    Size = new Size(130, 130),
                    Margin = new Padding(15),
                    BackColor = bgColor,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = textColor
                };
                btn.FlatAppearance.BorderColor = Form1.PinkSecondary;
                btn.FlatAppearance.BorderSize = 2;
                btn.Click += (s, e) => main.SwitchView(new OrderControl(main, tName));
                flow.Controls.Add(btn);
            }
            this.Controls.Add(flow);
        }
    }
}
