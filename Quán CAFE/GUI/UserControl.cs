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
                Button btn = new Button
                {
                    Text = tName + (isFull ? "\n(FULL)" : "\n(Trống)"),
                    Size = new Size(130, 130),
                    Margin = new Padding(15),
                    BackColor = isFull ? Form1.PinkSecondary : Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Form1.NavyPrimary
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
