using Quán_CAFE.BUS;
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
            DataGridView dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true };
            try { dgv.DataSource = InvoiceBUS.GetInvoiceHistory(); } catch { }
            this.Controls.Add(dgv);
        }
    }
}