using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quán_CAFE.BUS
{
    public class TableBUS
    {
        public static DataTable LoadTableList() => DataProvider.Instance.ExecuteQuery("EXEC USP_GetTableList");
        public static void UpdateTableStatus(int tableID, string status) =>
            DataProvider.Instance.ExecuteNonQuery("UPDATE DiningTable SET Status = @status WHERE TableID = @id", new object[] { status, tableID });
    }
}