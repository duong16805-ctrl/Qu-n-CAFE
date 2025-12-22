using Quán_CAFE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Quán_CAFE
{
    public partial class Form1 : Form
    {
        // Màu sắc chủ đạo (Giữ nguyên)
        // Thay đổi trong Form1.cs
        public static Color NavyPrimary = Color.FromArgb(137, 207, 240); // Xanh Baby (Baby Blue)
        public static Color PinkSecondary = Color.FromArgb(245, 245, 220); // Trắng Be (Beige)
        public static Color OrangeWait = Color.FromArgb(252, 163, 17); // Giữ nguyên màu chờ
        public static Color BgLight = Color.FromArgb(255, 255, 245); // Màu nền be nhạt

        private Panel pnlSidebar, pnlHeader, pnlContent;
        private Label lblHeaderTitle;
        private int _accountType; // 1: Admin, 0: User

        public Form1(int accountType)
        {
            _accountType = accountType;
            InitializeComponent();
            SetupLayout();
            // Mặc định hiển thị sơ đồ bàn khi mở app
            SwitchView(new DashboardControl(this));
        }

        private void SetupLayout()
        {
            this.Text = "HỆ THỐNG QUÁN CAFE";
            this.Size = new Size(1300, 850);
            this.WindowState = FormWindowState.Maximized; // Mở toàn màn hình

            // Sidebar
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = NavyPrimary };
            this.Controls.Add(pnlSidebar);

            Label lblLogo = new Label
            {
                Text = "MIU COFFEE",
                ForeColor = PinkSecondary,
                Height = 120,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlSidebar.Controls.Add(lblLogo);

            // Header
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            this.Controls.Add(pnlHeader);

            lblHeaderTitle = new Label
            {
                Text = "TRANG CHỦ",
                ForeColor = NavyPrimary,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(25, 22),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblHeaderTitle);

            // Content Area
            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = BgLight };
            this.Controls.Add(pnlContent);
            pnlContent.BringToFront();

            AddMenuButtons();
        }

        private void AddMenuButtons()
        {
            var navItems = new Dictionary<string, Func<UserControl>> {
                { "Sơ đồ bàn", () => new DashboardControl(this) },
                { "Quản lý món", () => new MenuControl() },
                { "Lịch sử HD", () => new InvoiceControl() },
                { "Báo cáo", () => new StatisticControl() }
            };

            int top = 140;
            foreach (var nav in navItems)
            {
                // PHÂN QUYỀN: Nếu là User (0), ẩn "Báo cáo" và "Quản lý món"
                if (_accountType == 0 && (nav.Key == "Báo cáo" || nav.Key == "Quản lý món"))
                    continue; 
                Button btn = new Button
                {
                    Text = "   " + nav.Key,
                    Size = new Size(240, 60),
                    Location = new Point(0, top),
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => {
                    lblHeaderTitle.Text = nav.Key.ToUpper();
                    SwitchView(nav.Value());
                };
                btn.MouseEnter += (s, e) => { btn.BackColor = PinkSecondary; btn.ForeColor = NavyPrimary; };
                btn.MouseLeave += (s, e) => { btn.BackColor = Color.Transparent; btn.ForeColor = Color.White; };
                pnlSidebar.Controls.Add(btn);
                top += 65;
            }
        }

        public void SwitchView(UserControl uc)
        {
            pnlContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(uc);
        }

        public void SetTitle(string t) => lblHeaderTitle.Text = t;
    }
}