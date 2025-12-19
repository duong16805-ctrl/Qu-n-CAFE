using Quán_CAFE.BUS;
using Quán_CAFE.DTO;
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
    public class OrderControl : UserControl
    {
        private DataGridView dgvBill;
        private Label lblTotal;
        private int _tableID;
        private string _tableName;
        private Form1 _main;

        public OrderControl(Form1 main, int tableID, string tableName)
        {
            _main = main;
            _tableID = tableID;
            _tableName = tableName;
            main.SetTitle("CHI TIẾT: " + tableName.ToUpper());

            // --- GIAO DIỆN CHÍNH ---
            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 750,
                BorderStyle = BorderStyle.None
            };

            // 1. Bên trái: Menu món ăn
            FlowLayoutPanel pnlMenu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(15),
                BackColor = Color.White
            };
            LoadMenu(pnlMenu);

            // 2. Bên phải: Hóa đơn tạm tính
            Panel pnlBill = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            // Bảng hiển thị món đã chọn
            dgvBill = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(245, 245, 245),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };
            dgvBill.Columns.Add("ID", "Mã"); // Cột ẩn chứa ProductID
            dgvBill.Columns.Add("Name", "Món");
            dgvBill.Columns.Add("Qty", "SL");
            dgvBill.Columns.Add("Price", "Giá");
            dgvBill.Columns.Add("Sum", "T.Tiền");
            dgvBill.Columns["ID"].Visible = false; // Ẩn cột ID sản phẩm
   

            // Panel điều khiển (Tổng tiền + Nút bấm)
            Panel pnlAction = new Panel { Dock = DockStyle.Bottom, Height = 260 };

            lblTotal = new Label
            {
                Text = "TỔNG: 0đ",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Form1.NavyPrimary,
                Padding = new Padding(0, 0, 10, 0)
            };

            // Nút Xóa món đang chọn
            Button btnRemove = new Button
            {
                Text = "XÓA MÓN ĐÃ CHỌN",
                Dock = DockStyle.Top,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Red,
                Margin = new Padding(0, 5, 0, 5)
            };
            btnRemove.FlatAppearance.BorderColor = Color.Red;
            btnRemove.Click += (s, e) => RemoveSelectedItem();

            // Nút Thanh toán
            Button btnPay = new Button
            {
                Text = "XÁC NHẬN THANH TOÁN",
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = Form1.NavyPrimary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPay.Click += (s, e) => HandlePayment();

            // Nút Dọn bàn / Quay lại
            Panel pnlBottomGroup = new Panel { Dock = DockStyle.Bottom, Height = 55 };

            Button btnBack = new Button
            {
                Text = "QUAY LẠI",
                Width = 150,
                Dock = DockStyle.Left,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnBack.Click += (s, e) => _main.SwitchView(new DashboardControl(_main));

            Button btnClear = new Button
            {
                Text = "DỌN BÀN (LÀM TRỐNG)",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.White
            };
            btnClear.Click += (s, e) => {
                if (MessageBox.Show("Xác nhận làm trống bàn này? Thao tác này sẽ đặt trạng thái bàn về 'Trống'.", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    TableBUS.UpdateTableStatus(_tableID, "Trống");
                    _main.SwitchView(new DashboardControl(_main));
                }
            };

            pnlBottomGroup.Controls.Add(btnClear);
            pnlBottomGroup.Controls.Add(btnBack);

            pnlAction.Controls.Add(btnRemove);
            pnlAction.Controls.Add(lblTotal);
            pnlAction.Controls.Add(btnPay);
            pnlAction.Controls.Add(new Label { Dock = DockStyle.Bottom, Height = 10 }); // Khoảng cách
            pnlAction.Controls.Add(pnlBottomGroup);

            pnlBill.Controls.Add(dgvBill);
            pnlBill.Controls.Add(pnlAction);

            split.Panel1.Controls.Add(pnlMenu);
            split.Panel2.Controls.Add(pnlBill);
            this.Controls.Add(split);

            RefreshBill();
        }

        private void LoadMenu(FlowLayoutPanel pnl)
        {
            try
            {
                DataTable dt = ProductBUS.GetAllProducts();
                foreach (DataRow r in dt.Rows)
                {
                    string proID = r["ProductID"].ToString();
                    string proName = r["ProductName"].ToString();
                    int proPrice = Convert.ToInt32(r["Price"]);

                    Button b = new Button
                    {
                        Text = proName + "\n" + proPrice.ToString("N0") + "đ",
                        Size = new Size(135, 100),
                        Margin = new Padding(8),
                        BackColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Form1.NavyPrimary,
                        Cursor = Cursors.Hand
                    };
                    b.FlatAppearance.BorderColor = Form1.PinkSecondary;
                    b.FlatAppearance.BorderSize = 2;

                    b.Click += (s, e) => {
                        OrderBUS.AddProductToOrder(_tableID, proID, 1, proPrice);
                        RefreshBill();
                    };
                    pnl.Controls.Add(b);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải menu: " + ex.Message);
            }
        }

        private void RefreshBill()
        {
            dgvBill.Rows.Clear();
            int invID = InvoiceBUS.GetUnpaidInvoiceIDByTable(_tableID);
            if (invID == -1)
            {
                lblTotal.Text = "TỔNG: 0đ";
                return;
            }

            DataTable details = InvoiceBUS.GetInvoiceDetails(invID);
            long total = 0;
            foreach (DataRow r in details.Rows)
            {
                // Lấy ProductID để hỗ trợ chức năng Xóa (Yêu cầu InvoiceBUS.GetInvoiceDetails trả về ProductID)
                string id = details.Columns.Contains("ProductID") ? r["ProductID"].ToString() : "";
                string name = r["ProductName"].ToString();
                int qty = Convert.ToInt32(r["Quantity"]);
                int price = Convert.ToInt32(r["PriceAtTime"]);
                long sum = Convert.ToInt64(r["Total"]);

                dgvBill.Rows.Add(id, name, qty, price.ToString("N0"), sum.ToString("N0"));
                total += sum;
            }
            lblTotal.Text = "TỔNG: " + total.ToString("N0") + "đ";
        }

        private void HandlePayment()
        {
            int invID = InvoiceBUS.GetUnpaidInvoiceIDByTable(_tableID);
            if (invID == -1)
            {
                MessageBox.Show("Bàn chưa có món ăn nào để thanh toán!");
                return;
            }

            if (MessageBox.Show("Xác nhận thanh toán hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                InvoiceBUS.PayInvoice(invID);
                MessageBox.Show("Thanh toán thành công!", "Thông báo");
                _main.SwitchView(new DashboardControl(_main));
            }
        }

        private void RemoveSelectedItem()
        {
            if (dgvBill.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một món trong danh sách để xóa!");
                return;
            }

            string productName = dgvBill.SelectedRows[0].Cells["Name"].Value.ToString();
            string proID = dgvBill.SelectedRows[0].Cells["ID"].Value.ToString();

            if (string.IsNullOrEmpty(proID))
            {
                MessageBox.Show("Lỗi: Không tìm thấy mã sản phẩm để xóa. Hãy kiểm tra lại câu truy vấn trong InvoiceBUS (phải SELECT ProductID)!");
                return;
            }

            if (MessageBox.Show($"Bạn có chắc muốn xóa món '{productName}' khỏi hóa đơn?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int invID = InvoiceBUS.GetUnpaidInvoiceIDByTable(_tableID);
                if (invID != -1)
                {
                    try
                    {
                        // Thực hiện xóa trực tiếp qua DataProvider
                        string query = "DELETE FROM InvoiceDetail WHERE InvoiceID = @inv AND ProductID = @pro";
                        DataProvider.Instance.ExecuteNonQuery(query, new object[] { invID, proID });

                        // Cập nhật lại tổng tiền của Invoice chính sau khi xóa chi tiết
                        string updateQuery = "UPDATE Invoice SET TotalAmount = (SELECT ISNULL(SUM(Quantity * PriceAtTime), 0) FROM InvoiceDetail WHERE InvoiceID = @inv) WHERE InvoiceID = @inv";
                        DataProvider.Instance.ExecuteNonQuery(updateQuery, new object[] { invID });

                        RefreshBill();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa món: " + ex.Message);
                    }
                }
            }
        }
    }
}