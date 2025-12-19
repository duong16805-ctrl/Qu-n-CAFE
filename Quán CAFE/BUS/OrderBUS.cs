using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quán_CAFE.BUS
{
    public class OrderBUS
    {
        public static void AddProductToOrder(int tableID, string productID, int qty, int price)
        {
            int invoiceID = InvoiceBUS.GetUnpaidInvoiceIDByTable(tableID);
            if (invoiceID == -1) invoiceID = InvoiceBUS.CreateNewInvoice(tableID);
            DataProvider.Instance.ExecuteNonQuery("EXEC USP_InsertInvoiceDetail @inv , @pro , @qty , @pri", new object[] { invoiceID, productID, qty, price });
        }
    }
}