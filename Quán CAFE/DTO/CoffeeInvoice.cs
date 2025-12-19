using Quán_CAFE.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quán_CAFE
{// Lớp lưu trữ thông tin hóa đơn //
    public class CoffeeInvoice
    {
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public string TableName { get; set; }
        public long Total { get; set; }
        public List<OrderItem> Details { get; set; }
    }
}