using Quán_CAFE.BUS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quán_CAFE.GUI
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            // Gán sự kiện Click cho các nút đã tạo trong Designer
            button1.Click += button1_Click;
            button2.Click += (s, e) => Application.Exit();
            txtmatkhau.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = txtdangnhap.Text;
            string pass = txtmatkhau.Text;

            // Truy vấn kiểm tra đăng nhập và lấy loại tài khoản (Giả sử bảng Account có cột Type: 1-Admin, 0-User)
            string query = "SELECT Type FROM Account WHERE UserName = @user AND Password = @pass";
            object result = DataProvider.Instance.ExecuteScalar(query, new object[] { user, pass });

            if (result != null)
            {
                int accountType = Convert.ToInt32(result);
                Form1 f = new Form1 (accountType); // Truyền quyền vào Form chính
                this.Hide();
                f.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
