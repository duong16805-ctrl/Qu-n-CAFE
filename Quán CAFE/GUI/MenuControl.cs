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
    public class MenuControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtID, txtName, txtPrice;
        private ComboBox cbCategory;

        public MenuControl()
        {
            this.Padding = new Padding(20);
            this.BackColor = Color.White;

            // --- GRID VIEW (BẢNG DANH SÁCH MÓN) ---
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(245, 245, 245),
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 35;

            // --- PANEL NHẬP LIỆU (THIẾT KẾ TO VÀ RÕ RÀNG) ---
            Panel pnlInput = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 220,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(250, 250, 250)
            };
            pnlInput.BorderStyle = BorderStyle.FixedSingle;

            Font labelFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Font inputFont = new Font("Segoe UI", 12);

            // Các trường nhập liệu
            txtID = new TextBox { Location = new Point(130, 30), Width = 150, Font = inputFont };
            txtName = new TextBox { Location = new Point(430, 30), Width = 300, Font = inputFont };
            cbCategory = new ComboBox { Location = new Point(130, 85), Width = 250, Font = inputFont, DropDownStyle = ComboBoxStyle.DropDownList };
            txtPrice = new TextBox { Location = new Point(530, 85), Width = 200, Font = inputFont };

            pnlInput.Controls.Add(new Label { Text = "Mã món:", Location = new Point(20, 35), Font = labelFont, AutoSize = true });
            pnlInput.Controls.Add(txtID);
            pnlInput.Controls.Add(new Label { Text = "Tên món:", Location = new Point(330, 35), Font = labelFont, AutoSize = true });
            pnlInput.Controls.Add(txtName);
            pnlInput.Controls.Add(new Label { Text = "Loại:", Location = new Point(20, 90), Font = labelFont, AutoSize = true });
            pnlInput.Controls.Add(cbCategory);
            pnlInput.Controls.Add(new Label { Text = "Giá bán (đ):", Location = new Point(410, 90), Font = labelFont, AutoSize = true });
            pnlInput.Controls.Add(txtPrice);

            // Nút Thêm món
            Button btnAdd = new Button
            {
                Text = "THÊM MÓN MỚI",
                Location = new Point(20, 145),
                Size = new Size(180, 55),
                BackColor = Form1.NavyPrimary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.Click += (s, e) => AddNewProduct();

            // Nút Xóa món
            Button btnDelete = new Button
            {
                Text = "XÓA MÓN ĐANG CHỌN",
                Location = new Point(220, 145),
                Size = new Size(200, 55),
                BackColor = Color.White,
                ForeColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderColor = Color.Red;
            btnDelete.FlatAppearance.BorderSize = 2;
            btnDelete.Click += (s, e) => DeleteSelectedProduct();

            pnlInput.Controls.Add(btnAdd);
            pnlInput.Controls.Add(btnDelete);

            this.Controls.Add(dgv);
            this.Controls.Add(pnlInput);

            LoadCategories();
            LoadMenuData();
        }

        private void LoadCategories()
        {
            try
            {
                DataTable dt = CategoryBUS.GetListCategory();
                if (dt != null && dt.Rows.Count > 0)
                {
                    cbCategory.DataSource = dt;
                    cbCategory.DisplayMember = "CategoryName";
                    cbCategory.ValueMember = "CategoryID";
                }
            }
            catch { }
        }

        private void LoadMenuData()
        {
            try
            {
                // Lấy dữ liệu từ BUS (Đã xóa GlobalMenu)
                DataTable dt = ProductBUS.GetAllProducts();
                dgv.DataSource = dt;

                // Cấu hình lại tiêu đề bảng cho chuyên nghiệp
                if (dgv.Columns.Count > 0)
                {
                    if (dgv.Columns.Contains("ProductID")) dgv.Columns["ProductID"].HeaderText = "MÃ MÓN";
                    if (dgv.Columns.Contains("ProductName")) dgv.Columns["ProductName"].HeaderText = "TÊN SẢN PHẨM";
                    if (dgv.Columns.Contains("Price"))
                    {
                        dgv.Columns["Price"].HeaderText = "GIÁ BÁN (VNĐ)";
                        dgv.Columns["Price"].DefaultCellStyle.Format = "N0";
                    }
                    if (dgv.Columns.Contains("CategoryName")) dgv.Columns["CategoryName"].HeaderText = "DANH MỤC";

                    // Ẩn cột ID loại nếu có
                    if (dgv.Columns.Contains("CategoryID")) dgv.Columns["CategoryID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thực đơn: " + ex.Message);
            }
        }

        private void AddNewProduct()
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (string.IsNullOrEmpty(txtID.Text) || string.IsNullOrEmpty(txtName.Text) || !int.TryParse(txtPrice.Text, out int price))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã, Tên và Giá (số nguyên)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại món!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int categoryID = (int)cbCategory.SelectedValue;
                // Sử dụng SQL trực tiếp qua DataProvider để đảm bảo tính nhất quán
                string query = "INSERT INTO Product (ProductID, ProductName, Price, CategoryID) VALUES ( @id , @name , @price , @cat )";
                int result = BUS.DataProvider.Instance.ExecuteNonQuery(query, new object[] { txtID.Text.Trim(), txtName.Text.Trim(), price, categoryID });

                if (result > 0)
                {
                    MessageBox.Show("Thêm món vào thực đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMenuData();
                    // Xóa trắng input sau khi thêm
                    txtID.Clear(); txtName.Clear(); txtPrice.Clear();
                    txtID.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: Mã món này đã tồn tại hoặc có lỗi CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSelectedProduct()
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một món trong bảng để xóa!", "Thông báo");
                return;
            }

            string id = dgv.SelectedRows[0].Cells["ProductID"].Value.ToString();
            string name = dgv.SelectedRows[0].Cells["ProductName"].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa món '{name}' khỏi thực đơn không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Product WHERE ProductID = @id";
                    int result = BUS.DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });

                    if (result > 0)
                    {
                        LoadMenuData();
                        MessageBox.Show("Đã xóa món thành công.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa món này vì đã có trong hóa đơn lịch sử: " + ex.Message, "Lỗi ràng buộc", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}