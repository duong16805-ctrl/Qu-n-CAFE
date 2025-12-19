using Quán_CAFE.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quán_CAFE
{
    public class OrderControl : UserControl
    {
        private DataGridView dgvBill;
        private Label lblTotal, lblStatus;
        private string _tName;
        private Form1 _main;
        private long _sum = 0;

        public OrderControl(Form1 main, string tName)
        {
            _main = main; _tName = tName;
            main.SetTitle("CHI TIẾT: " + tName.ToUpper());

            bool isPaid = Form1.PaidTables.Contains(tName);

            // Left: Menu
            FlowLayoutPanel flowMenu = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
            flowMenu.Enabled = !isPaid;

            foreach (var item in Form1.GlobalMenu)
            {
                Button btn = new Button
                {
                    Text = item.Name + "\n" + item.Price.ToString("N0") + "đ",
                    Size = new Size(135, 95),
                    Margin = new Padding(10),
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Form1.NavyPrimary
                };
                btn.FlatAppearance.BorderColor = Form1.PinkSecondary;
                btn.Click += (s, e) => AddItem(item.Name, item.Price);
                flowMenu.Controls.Add(btn);
            }

            // Right: Bill
            Panel pnlBill = new Panel { Dock = DockStyle.Right, Width = 450, BackColor = Color.White, Padding = new Padding(10) };
            pnlBill.BorderStyle = BorderStyle.FixedSingle;

            lblStatus = new Label
            {
                Text = isPaid ? "TRẠNG THÁI: ĐÃ THANH TOÁN" : "TRẠNG THÁI: CHƯA THANH TOÁN",
                Dock = DockStyle.Top,
                Height = 40,
                ForeColor = isPaid ? Color.Green : Color.Red,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            dgvBill = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvBill.Columns.Add("N", "Tên món"); dgvBill.Columns.Add("Q", "SL"); dgvBill.Columns.Add("P", "Giá"); dgvBill.Columns.Add("S", "Tổng");

            // QUAN TRỌNG: Tải lại các món cũ nếu bàn này đang có khách
            if (Form1.CurrentOrders.ContainsKey(_tName))
            {
                foreach (var item in Form1.CurrentOrders[_tName])
                {
                    dgvBill.Rows.Add(item.Name, item.Qty, item.Price.ToString("N0"), (item.Qty * item.Price).ToString("N0"));
                }
            }

            Panel pnlBot = new Panel { Dock = DockStyle.Bottom, Height = 250 };
            lblTotal = new Label { Text = "TỔNG: 0đ", Dock = DockStyle.Top, Height = 50, Font = new Font("Segoe UI", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, ForeColor = Form1.NavyPrimary };

            Button btnDel = new Button { Text = "XÓA MÓN ĐANG CHỌN", Dock = DockStyle.Top, Height = 40, BackColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = !isPaid };
            btnDel.Click += (s, e) => {
                if (dgvBill.SelectedRows.Count > 0)
                {
                    dgvBill.Rows.RemoveAt(dgvBill.SelectedRows[0].Index);
                    SyncToGlobal(); // Cập nhật bộ nhớ sau khi xóa
                    UpdateTotal();
                }
            };

            Button btnPay = new Button
            {
                Text = "XÁC NHẬN THANH TOÁN",
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = isPaid ? Color.Gray : Form1.NavyPrimary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Enabled = !isPaid
            };
            btnPay.Click += (s, e) =>
            {
                if (_sum == 0) return;

                // Lấy chi tiết món để lưu vào hóa đơn lịch sử
                var details = Form1.CurrentOrders.ContainsKey(_tName) ? Form1.CurrentOrders[_tName] : new List<OrderItem>();

                Form1.InvoiceHistory.Add(new CoffeeInvoice
                {
                    InvoiceNo = "HD" + (Form1.InvoiceHistory.Count + 1).ToString("D3"),
                    Date = DateTime.Now,
                    TableName = _tName,
                    Total = _sum,
                    Details = details
                });

                Form1.PaidTables.Add(_tName);
                Form1.SaveData(); // Lưu file ngay
                MessageBox.Show("Thanh toán thành công!");
                _main.SwitchView(new DashboardControl(_main));
            };

            Button btnClear = new Button
            {
                Text = "XÁC NHẬN DỌN BÀN (LÀM TRỐNG)",
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = isPaid ? Form1.PinkSecondary : Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnClear.Click += (s, e) => {
                Form1.OccupiedTables.Remove(_tName);
                Form1.PaidTables.Remove(_tName);
                Form1.CurrentOrders.Remove(_tName); // Xóa món ăn của bàn này
                Form1.SaveData();
                _main.SwitchView(new DashboardControl(_main));
            };

            pnlBot.Controls.Add(btnClear);
            pnlBot.Controls.Add(btnPay);
            pnlBot.Controls.Add(btnDel);
            pnlBot.Controls.Add(lblTotal);

            pnlBill.Controls.Add(dgvBill);
            pnlBill.Controls.Add(lblStatus);
            pnlBill.Controls.Add(pnlBot);

            this.Controls.Add(flowMenu);
            this.Controls.Add(pnlBill);
            UpdateTotal();
        }

        private void AddItem(string name, int price)
        {
            Form1.OccupiedTables.Add(_tName);
            bool exists = false;
            foreach (DataGridViewRow row in dgvBill.Rows)
            {
                if (row.Cells[0].Value.ToString() == name)
                {
                    int qty = int.Parse(row.Cells[1].Value.ToString()) + 1;
                    row.Cells[1].Value = qty;
                    row.Cells[3].Value = (qty * price).ToString("N0");
                    exists = true; break;
                }
            }
            if (!exists) dgvBill.Rows.Add(name, 1, price.ToString("N0"), price.ToString("N0"));

            SyncToGlobal(); // Cập nhật món vào bộ nhớ và lưu file
            UpdateTotal();
        }

        // MỚI: Đồng bộ dữ liệu từ Grid vào Dictionary toàn cục
        private void SyncToGlobal()
        {
            List<OrderItem> items = new List<OrderItem>();
            foreach (DataGridViewRow r in dgvBill.Rows)
            {
                items.Add(new OrderItem
                {
                    Name = r.Cells[0].Value.ToString(),
                    Qty = int.Parse(r.Cells[1].Value.ToString()),
                    Price = int.Parse(r.Cells[2].Value.ToString().Replace(",", ""))
                });
            }
            Form1.CurrentOrders[_tName] = items;
            Form1.SaveData(); // Lưu xuống ổ cứng
        }

        private void UpdateTotal()
        {
            _sum = 0;
            foreach (DataGridViewRow r in dgvBill.Rows)
            {
                if (r.Cells[3].Value != null)
                    _sum += long.Parse(r.Cells[3].Value.ToString().Replace(",", ""));
            }
            lblTotal.Text = "TỔNG: " + _sum.ToString("N0") + "đ";
        }
    }
}