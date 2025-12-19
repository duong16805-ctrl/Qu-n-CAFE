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
    public class MenuControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtN, txtP;
        private ComboBox cbC;

        public MenuControl()
        {
            this.Padding = new Padding(20);
            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false };
            dgv.Columns.Add("ID", "Mã"); dgv.Columns.Add("N", "Tên món"); dgv.Columns.Add("C", "Loại"); dgv.Columns.Add("P", "Giá");
            Load();

            Panel p = new Panel { Dock = DockStyle.Bottom, Height = 150 };
            txtN = new TextBox { Location = new Point(80, 20), Width = 150 };
            cbC = new ComboBox { Location = new Point(310, 20), Width = 120 };
            cbC.Items.AddRange(new string[] { "Cà phê", "Trà", "Bánh", "Khác" }); cbC.SelectedIndex = 0;
            txtP = new TextBox { Location = new Point(510, 20), Width = 100 };

            p.Controls.Add(new Label { Text = "Tên:", Location = new Point(10, 22) }); p.Controls.Add(txtN);
            p.Controls.Add(new Label { Text = "Loại:", Location = new Point(260, 22) }); p.Controls.Add(cbC);
            p.Controls.Add(new Label { Text = "Giá:", Location = new Point(460, 22) }); p.Controls.Add(txtP);

            Button btnA = new Button { Text = "THÊM MÓN", Location = new Point(10, 70), Size = new Size(120, 45), BackColor = Form1.NavyPrimary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnA.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtN.Text) || !int.TryParse(txtP.Text, out int pr)) return;
                Form1.GlobalMenu.Add(new CoffeeDish { ID = "M" + (Form1.GlobalMenu.Count + 1), Name = txtN.Text, Category = cbC.Text, Price = pr });
                Load();
            };
            p.Controls.Add(btnA); this.Controls.Add(dgv); this.Controls.Add(p);
        }
        private void Load() { dgv.Rows.Clear(); foreach (var i in Form1.GlobalMenu) dgv.Rows.Add(i.ID, i.Name, i.Category, i.Price.ToString("N0")); }
    }
}
