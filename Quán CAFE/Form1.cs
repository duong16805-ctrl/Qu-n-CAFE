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
        public static Color NavyPrimary = Color.FromArgb(20, 33, 61);
        public static Color PinkSecondary = Color.FromArgb(255, 182, 193);
        public static Color OrangeWait = Color.FromArgb(252, 163, 17);
        public static Color BgLight = Color.FromArgb(248, 249, 250);

        // --- KHO DỮ LIỆU TĨNH ---
        public static HashSet<string> OccupiedTables = new HashSet<string>();
        public static HashSet<string> PaidTables = new HashSet<string>();
        public static List<CoffeeInvoice> InvoiceHistory = new List<CoffeeInvoice>();

        // MỚI: Lưu danh sách món đang gọi của từng bàn (Để thoát ra vào lại không mất món)
        public static Dictionary<string, List<OrderItem>> CurrentOrders = new Dictionary<string, List<OrderItem>>();

        public static List<CoffeeDish> GlobalMenu = new List<CoffeeDish> {
            new CoffeeDish { ID="M01", Name="Espresso", Price=35000, Category="Cà phê" },
            new CoffeeDish { ID="M02", Name="Latte Hồng", Price=45000, Category="Cà phê" },
            new CoffeeDish { ID="M03", Name="Capuchino", Price=50000, Category="Cà phê" },
            new CoffeeDish { ID="M04", Name="Trà Đào Cam Sả", Price=40000, Category="Trà" },
            new CoffeeDish { ID="M05", Name="Bánh Croissant", Price=25000, Category="Bánh" }
        };

        private Panel pnlSidebar, pnlHeader, pnlContent;
        private Label lblHeaderTitle;

        public Form1()
        {
            InitializeComponent();
            LoadData(); // Tải dữ liệu cũ khi mở ứng dụng
            SetupLayout();
            SwitchView(new DashboardControl(this));
        }

        // --- CƠ CHẾ LƯU TRỮ FILE ---
        private static string dataPath = "coffee_data.xml";

        public static void SaveData()
        {
            try
            {
                // Chúng ta gom tất cả vào một đối tượng để lưu XML
                var data = new AppData
                {
                    Occupied = OccupiedTables.ToList(),
                    Paid = PaidTables.ToList(),
                    History = InvoiceHistory,
                    // Chuyển Dictionary sang List để dễ Serialized
                    Orders = CurrentOrders.Select(x => new TableOrder { TableName = x.Key, Items = x.Value }).ToList()
                };
                XmlSerializer ser = new XmlSerializer(typeof(AppData));
                using (TextWriter tw = new StreamWriter(dataPath))
                {
                    ser.Serialize(tw, data);
                }
            }
            catch { }
        }

        private void LoadData()
        {
            if (!File.Exists(dataPath)) return;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(AppData));
                using (TextReader tr = new StreamReader(dataPath))
                {
                    var data = (AppData)ser.Deserialize(tr);
                    OccupiedTables = new HashSet<string>(data.Occupied);
                    PaidTables = new HashSet<string>(data.Paid);
                    InvoiceHistory = data.History;
                    CurrentOrders = data.Orders.ToDictionary(x => x.TableName, x => x.Items);
                }
            }
            catch { }
        }

        // Class phụ trợ để lưu file
        public class AppData
        {
            public List<string> Occupied { get; set; }
            public List<string> Paid { get; set; }
            public List<CoffeeInvoice> History { get; set; }
            public List<TableOrder> Orders { get; set; }
        }
        public class TableOrder
        {
            public string TableName { get; set; }
            public List<OrderItem> Items { get; set; }
        }

        // (Các hàm SetupLayout, AddMenuButtons, SwitchView giữ nguyên như cũ...)
        private void SetupLayout()
        {
            this.Text = "GEMINI COFFEE MANAGER PRO - v1.7 (Auto-Save)";
            this.Size = new Size(1300, 850);
            this.StartPosition = FormStartPosition.CenterScreen;

            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = NavyPrimary };
            this.Controls.Add(pnlSidebar);

            Label lblLogo = new Label
            {
                Text = "NAVY & PINK\nCOFFEE",
                ForeColor = PinkSecondary,
                Height = 120,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlSidebar.Controls.Add(lblLogo);

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
                btn.Click += (s, e) => { lblHeaderTitle.Text = nav.Key.ToUpper(); SwitchView(nav.Value()); };
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