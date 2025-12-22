using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quán_CAFE.BUS
{
    public class InvoiceBUS
    {
        public static int GetUnpaidInvoiceIDByTable(int tableID)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT InvoiceID FROM Invoice WHERE TableID = @id AND IsPaid = 0", new object[] { tableID });
            return data.Rows.Count > 0 ? (int)data.Rows[0]["InvoiceID"] : -1;
        }

        public static DataTable GetInvoiceDetails(int invoiceID) =>
            DataProvider.Instance.ExecuteQuery("SELECT id.ProductID, p.ProductName, id.Quantity, id.PriceAtTime, (id.Quantity * id.PriceAtTime) as Total FROM InvoiceDetail id JOIN Product p ON id.ProductID = p.ProductID WHERE id.InvoiceID = @id", new object[] { invoiceID });

        public static int CreateNewInvoice(int tableID)
        {
            // Dùng SELECT SCOPE_IDENTITY() để lấy ID vừa tạo chính xác hơn
            string query = "INSERT INTO Invoice (TableID, OrderDate, TotalAmount, IsPaid) VALUES ( @tableID , GETDATE(), 0, 0); SELECT SCOPE_IDENTITY();";
            object result = DataProvider.Instance.ExecuteScalar(query, new object[] { tableID });
            int id = Convert.ToInt32(result);
            TableBUS.UpdateTableStatus(tableID, "Có khách");
            return id;
        }

        public static void PayInvoice(int invoiceID) => DataProvider.Instance.ExecuteNonQuery("EXEC USP_PayInvoice @id", new object[] { invoiceID });

        public static DataTable GetInvoiceHistory() =>
            DataProvider.Instance.ExecuteQuery("SELECT i.InvoiceNo, i.OrderDate, t.TableName, i.TotalAmount FROM Invoice i JOIN DiningTable t ON i.TableID = t.TableID WHERE i.IsPaid = 1 ORDER BY i.OrderDate DESC");

        public static long GetRevenueByDate(DateTime date)
        {
            object res = DataProvider.Instance.ExecuteScalar("SELECT SUM(TotalAmount) FROM Invoice WHERE CAST(OrderDate AS DATE) = @d AND IsPaid = 1", new object[] { date.ToString("yyyy-MM-dd") });
            return (res == null || res == DBNull.Value) ? 0 : Convert.ToInt64(res);
        }

        public static long GetRevenueByMonth(int month, int year)
        {
            object res = DataProvider.Instance.ExecuteScalar("SELECT SUM(TotalAmount) FROM Invoice WHERE MONTH(OrderDate) = @m AND YEAR(OrderDate) = @y AND IsPaid = 1", new object[] { month, year });
            return (res == null || res == DBNull.Value) ? 0 : Convert.ToInt64(res);
        }

        // --- CÁC HÀM MỚI (BẮT BUỘC ĐỂ SỬA LỖI) ---

        // 1. Lấy doanh thu theo khoảng thời gian (cho StatisticControl)
        public static DataTable GetRevenueListByDateRange(DateTime fromDate, DateTime toDate)
        {
            return DataProvider.Instance.ExecuteQuery(
                "SELECT OrderDate, TotalAmount FROM Invoice WHERE OrderDate >= @from AND OrderDate <= @to AND IsPaid = 1",
                new object[] { fromDate, toDate });
        }

        // 2. Xử lý chuyển bàn (cho OrderControl)
        public static bool SwitchTable(int idTableOld, int idTableNew)
        {
            try
            {
                int idInvoiceOld = GetUnpaidInvoiceIDByTable(idTableOld);

                // Nếu bàn cũ không có hóa đơn thì không cần chuyển
                if (idInvoiceOld == -1) return false;

                // Cập nhật TableID cho hóa đơn
                DataProvider.Instance.ExecuteNonQuery("UPDATE Invoice SET TableID = @new WHERE InvoiceID = @inv", new object[] { idTableNew, idInvoiceOld });

                // Cập nhật trạng thái 2 bàn
                TableBUS.UpdateTableStatus(idTableOld, "Trống");
                TableBUS.UpdateTableStatus(idTableNew, "Có khách");

                return true;
            }
            catch { return false; }
        }
    }
}