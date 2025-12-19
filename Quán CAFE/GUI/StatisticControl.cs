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
            long day = Form1.InvoiceHistory.Where(i => i.Date.Date == DateTime.Today).Sum(i => i.Total);
            long month = Form1.InvoiceHistory.Where(i => i.Date.Month == DateTime.Today.Month).Sum(i => i.Total);

            FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill };
            f.Controls.Add(Card("DOANH THU NGÀY", day.ToString("N0") + "đ", Form1.NavyPrimary, Color.White));
            f.Controls.Add(Card("DOANH THU THÁNG", month.ToString("N0") + "đ", Form1.PinkSecondary, Form1.NavyPrimary));

            this.Controls.Add(f);
        }
        private Panel Card(string t, string v, Color b, Color f)
        {
            Panel p = new Panel { Size = new Size(350, 160), BackColor = b, Margin = new Padding(15) };
            p.Controls.Add(new Label { Text = v, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = f, Font = new Font("Segoe UI", 22, FontStyle.Bold) });
            p.Controls.Add(new Label { Text = t, Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.BottomCenter, ForeColor = f, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            return p;
        }
    }
}