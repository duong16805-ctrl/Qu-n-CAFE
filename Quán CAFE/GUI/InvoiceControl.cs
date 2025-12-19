using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quán_CAFE
{
    public class InvoiceControl : UserControl
    {
        public InvoiceControl()
        {
            this.Padding = new Padding(20);
            DataGridView dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false };
            dgv.Columns.Add("No", "Số HD"); dgv.Columns.Add("D", "Ngày giờ"); dgv.Columns.Add("T", "Bàn"); dgv.Columns.Add("S", "Tiền");
            foreach (var i in Form1.InvoiceHistory) dgv.Rows.Add(i.InvoiceNo, i.Date.ToString("HH:mm dd/MM"), i.TableName, i.Total.ToString("N0") + "đ");
            this.Controls.Add(dgv);
        }
    }
}