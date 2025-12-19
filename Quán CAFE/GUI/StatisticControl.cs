using Quán_CAFE.BUS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quán_CAFE
{
    public class StatisticControl : UserControl
    {
        public StatisticControl()
        {
            this.Padding = new Padding(20);
            FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
            try
            {
                long today = InvoiceBUS.GetRevenueByDate(DateTime.Today);
                long month = InvoiceBUS.GetRevenueByMonth(DateTime.Today.Month, DateTime.Today.Year);
                f.Controls.Add(Card("DOANH THU HÔM NAY", today.ToString("N0") + "đ", Form1.NavyPrimary, Color.White));
                f.Controls.Add(Card("DOANH THU THÁNG NÀY", month.ToString("N0") + "đ", Form1.PinkSecondary, Form1.NavyPrimary));
            }
            catch (Exception ex)
            {
                f.Controls.Add(new Label { Text = "Lỗi: " + ex.Message, ForeColor = Color.Red, AutoSize = true });
            }
            this.Controls.Add(f);
        }

        private Panel Card(string title, string value, Color backColor, Color foreColor)
        {
            Panel p = new Panel { Size = new Size(380, 160), BackColor = backColor, Margin = new Padding(15) };
            Label lblValue = new Label { Text = value, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = foreColor, Font = new Font("Segoe UI", 24, FontStyle.Bold) };
            Label lblTitle = new Label { Text = title, Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.BottomCenter, ForeColor = foreColor, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            p.Controls.Add(lblValue); p.Controls.Add(lblTitle);
            return p;
        }
    }
}