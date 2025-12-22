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
        public static Color NavyPrimary = Color.FromArgb(20, 33, 61);
        public static Color PinkSecondary = Color.FromArgb(255, 182, 193);
        public static Color OrangeWait = Color.FromArgb(252, 163, 17);
        public static Color BgLight = Color.FromArgb(248, 249, 250);

        private Panel pnlSidebar, pnlHeader, pnlContent;
        private Label lblHeaderTitle;

        public Form1()
        {
            InitializeComponent();
            SetupLayout();
            // Mặc định hiển thị sơ đồ bàn khi mở app
            SwitchView(new DashboardControl(this));
        }

        private void SetupLayout()
        {
            this.Text = "HỆ THỐNG QUÁN CAFE";
            this.Size = new Size(1300, 850);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Sidebar
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = NavyPrimary };
            this.Controls.Add(pnlSidebar);

            Label lblLogo = new Label
            {
                Text = "NAVY COFFEE",
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