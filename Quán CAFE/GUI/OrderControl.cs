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
        private Label lblTotal;
        private string _tName;
        private Form1 _main;
        private long _sum = 0;

        public OrderControl(Form1 main, string tName)
        {
            _main = main; _tName = tName;
            main.SetTitle("GỌI MÓN: " + tName.ToUpper());

            // Left: Menu
            FlowLayoutPanel flowMenu = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
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

            Panel pnlBot = new Panel { Dock = DockStyle.Bottom, Height = 250 };
            lblTotal = new Label { Text = "TỔNG: 0đ", Dock = DockStyle.Top, Height = 50, Font = new Font("Segoe UI", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, ForeColor = Form1.NavyPrimary };

            Button btnDel = new Button { Text = "XÓA MÓN ĐANG CHỌN", Dock = DockStyle.Top, Height = 45, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDel.Click += (s, e) => { if (dgvBill.SelectedRows.Count > 0) { dgvBill.Rows.RemoveAt(dgvBill.SelectedRows[0].Index); UpdateTotal(); } };

            Button btnPay = new Button { Text = "THANH TOÁN (LƯU HD)", Dock = DockStyle.Top, Height = 60, BackColor = Form1.NavyPrimary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnPay.Click += (s, e) =>
            {
                if (_sum == 0) return;
                Form1.InvoiceHistory.Add(new CoffeeInvoice { InvoiceNo = "HD" + (Form1.InvoiceHistory.Count + 1).ToString("D3"), Date = DateTime.Now, TableName = _tName, Total = _sum });
                MessageBox.Show("Đã lưu hóa đơn. Bàn vẫn FULL cho tới khi Dọn Bàn.");
                _main.SwitchView(new DashboardControl(_main));
            };

            Button btnClear = new Button { Text = "DỌN BÀN (MỞ TRỐNG)", Dock = DockStyle.Top, Height = 50, BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat };
            btnClear.Click += (s, e) => { Form1.OccupiedTables.Remove(_tName); _main.SwitchView(new DashboardControl(_main)); };

            pnlBot.Controls.Add(btnClear); pnlBot.Controls.Add(btnPay); pnlBot.Controls.Add(btnDel); pnlBot.Controls.Add(lblTotal);
            pnlBill.Controls.Add(dgvBill); pnlBill.Controls.Add(pnlBot);

            this.Controls.Add(flowMenu); this.Controls.Add(pnlBill);
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
                    row.Cells[1].Value = qty; row.Cells[3].Value = (qty * price).ToString("N0");
                    exists = true; break;
                }
            }
            if (!exists) dgvBill.Rows.Add(name, 1, price.ToString("N0"), price.ToString("N0"));
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            _sum = 0;
            foreach (DataGridViewRow r in dgvBill.Rows) _sum += long.Parse(r.Cells[3].Value.ToString().Replace(",", ""));
            lblTotal.Text = "TỔNG: " + _sum.ToString("N0") + "đ";
        }
    }
}
