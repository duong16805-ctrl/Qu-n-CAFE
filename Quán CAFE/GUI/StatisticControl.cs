using Quán_CAFE.BUS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Quán_CAFE
{
    public class StatisticControl : UserControl
    {
        private Chart chartRevenue;
        private DateTimePicker dtpFrom, dtpTo;
        private Label lblTotalRevenue;

        public StatisticControl()
        {
            this.Padding = new Padding(20);
            this.BackColor = Color.White;

            // 1. Thanh công cụ (Toolbar)
            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60 };

            dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(100, 15), Width = 120 };
            dtpFrom.Value = DateTime.Today.AddDays(-7); // Mặc định xem 7 ngày qua

            dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(350, 15), Width = 120 };

            Button btnFilter = new Button { Text = "THỐNG KÊ", Location = new Point(500, 12), Size = new Size(100, 30), BackColor = Form1.NavyPrimary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnFilter.Click += (s, e) => LoadStatistic();

            pnlToolbar.Controls.Add(new Label { Text = "Từ ngày:", Location = new Point(20, 18), AutoSize = true });
            pnlToolbar.Controls.Add(dtpFrom);
            pnlToolbar.Controls.Add(new Label { Text = "Đến ngày:", Location = new Point(270, 18), AutoSize = true });
            pnlToolbar.Controls.Add(dtpTo);
            pnlToolbar.Controls.Add(btnFilter);

            // 2. Tổng doanh thu
            lblTotalRevenue = new Label { Text = "TỔNG DOANH THU: 0đ", Dock = DockStyle.Bottom, Height = 50, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.Red };

            // 3. Biểu đồ
            chartRevenue = new Chart { Dock = DockStyle.Fill };
            ChartArea area = new ChartArea("MainArea");
            chartRevenue.ChartAreas.Add(area);

            Series series = new Series("Doanh Thu");
            series.ChartType = SeriesChartType.Column; // Biểu đồ cột
            series.Color = Form1.NavyPrimary;
            series.IsValueShownAsLabel = true; // Hiển thị số tiền trên cột
            chartRevenue.Series.Add(series);

            chartRevenue.Titles.Add("BIỂU ĐỒ DOANH THU THEO THỜI GIAN");

            // Thứ tự thêm control rất quan trọng để Dock hoạt động đúng
            this.Controls.Add(chartRevenue);      // Fill thêm trước
            this.Controls.Add(pnlToolbar);        // Top thêm sau
            this.Controls.Add(lblTotalRevenue);   // Bottom thêm sau

            // Đảm bảo các thanh công cụ nổi lên trên biểu đồ
            pnlToolbar.BringToFront();
            lblTotalRevenue.BringToFront();

            // Tải dữ liệu lần đầu (có try-catch để an toàn)
            try { LoadStatistic(); } catch { }
        }

        private void LoadStatistic()
        {
            try
            {
                DateTime from = dtpFrom.Value;
                DateTime to = dtpTo.Value.AddDays(1).AddSeconds(-1); // Lấy hết ngày cuối cùng

                DataTable dt = InvoiceBUS.GetRevenueListByDateRange(from, to);

                chartRevenue.Series["Doanh Thu"].Points.Clear();
                long total = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Gom nhóm theo ngày (vì DB lưu cả giờ phút giây)
                    Dictionary<string, long> dailyRevenue = new Dictionary<string, long>();

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["OrderDate"] == DBNull.Value) continue;

                        DateTime date = (DateTime)row["OrderDate"];
                        long amount = (row["TotalAmount"] != DBNull.Value) ? Convert.ToInt64(row["TotalAmount"]) : 0;
                        string key = date.ToString("dd/MM");

                        if (dailyRevenue.ContainsKey(key)) dailyRevenue[key] += amount;
                        else dailyRevenue.Add(key, amount);

                        total += amount;
                    }

                    foreach (var item in dailyRevenue)
                    {
                        chartRevenue.Series["Doanh Thu"].Points.AddXY(item.Key, item.Value);
                    }
                }

                lblTotalRevenue.Text = "TỔNG DOANH THU: " + total.ToString("N0") + "đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị biểu đồ: " + ex.Message + "\nVui lòng kiểm tra lại kết nối CSDL hoặc tham chiếu Chart.");
            }
        }
    }
}