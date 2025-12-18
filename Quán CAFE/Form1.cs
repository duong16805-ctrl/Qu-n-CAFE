using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quán_CAFE
{
    public partial class Form1 : Form
    {
        // Khai báo bảng màu chủ đạo
        Color navyPrimary = Color.FromArgb(20, 33, 61);    // Xanh Navy đậm
        Color pinkSecondary = Color.FromArgb(255, 182, 193); // Hồng nhạt (Light Pink)
        Color pinkHover = Color.FromArgb(255, 209, 220);     // Hồng rất nhạt khi di chuột
        Color bgLight = Color.FromArgb(248, 249, 250);      // Nền xám trắng cực nhẹ

        private Panel pnlSidebar;
        private Panel pnlHeader;
        private FlowLayoutPanel flowPnlTables;
        public Form1()
        {
            InitializeComponent(); // Giữ nguyên hàm này của hệ thống
            SetupNavyPinkUI();

        }
        private void SetupNavyPinkUI()
        {
            // 1. Cấu hình Form chính
            this.Text = "PREMIUM COFFEE MANAGER";
            this.Size = new Size(1100, 700);
            this.BackColor = bgLight;

            // 2. Sidebar Xanh Navy
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 220;
            pnlSidebar.BackColor = navyPrimary;
            this.Controls.Add(pnlSidebar);

            // Logo vùng Header của Sidebar
            Label lblLogo = new Label();
            lblLogo.Text = "NAVY COFFEE";
            lblLogo.ForeColor = pinkSecondary;
            lblLogo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblLogo.Dock = DockStyle.Top;
            lblLogo.Height = 100;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            pnlSidebar.Controls.Add(lblLogo);

            // 3. Header phía trên
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = Color.White;
            this.Controls.Add(pnlHeader);

            Label lblTitle = new Label();
            lblTitle.Text = "SƠ ĐỒ BÀN KHÁCH";
            lblTitle.ForeColor = navyPrimary;
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.Location = new Point(20, 22);
            lblTitle.AutoSize = true;
            pnlHeader.Controls.Add(lblTitle);

            // 4. Khu vực hiển thị bàn
            flowPnlTables = new FlowLayoutPanel();
            flowPnlTables.Dock = DockStyle.Fill;
            flowPnlTables.Padding = new Padding(25);
            flowPnlTables.AutoScroll = true;
            this.Controls.Add(flowPnlTables);
            flowPnlTables.BringToFront();

            AddMenuButtons();
            GenerateTables(15);
        }

        private void AddMenuButtons()
        {
            string[] items = { "Trang chủ", "Đặt món", "Hóa đơn", "Thống kê", "Cài đặt" };
            int top = 120;
            foreach (var item in items)
            {
                Button btn = new Button();
                btn.Text = item;
                btn.Size = new Size(220, 55);
                btn.Location = new Point(0, top);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(30, 0, 0, 0);

                // Hiệu ứng Hover chuyển sang màu hồng nhạt
                btn.MouseEnter += (s, e) =>
                {
                    btn.BackColor = pinkSecondary;
                    btn.ForeColor = navyPrimary;
                };
                btn.MouseLeave += (s, e) =>
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = Color.White;
                };

                pnlSidebar.Controls.Add(btn);
                top += 60;
            }
        }

        private void GenerateTables(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                Button btnTable = new Button();
                btnTable.Size = new Size(110, 110);
                btnTable.Margin = new Padding(12);
                btnTable.Text = "BÀN " + i;
                btnTable.FlatStyle = FlatStyle.Flat;
                btnTable.FlatAppearance.BorderColor = pinkSecondary;
                btnTable.FlatAppearance.BorderSize = 2;
                btnTable.BackColor = Color.White;
                btnTable.ForeColor = navyPrimary;
                btnTable.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                // Giả lập bàn có khách sẽ tô nguyên màu hồng
                if (i == 2 || i == 5 || i == 8)
                {
                    btnTable.BackColor = pinkSecondary;
                    btnTable.FlatAppearance.BorderSize = 0;
                }

                flowPnlTables.Controls.Add(btnTable);
            }
        }
    }
}
