using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quán_CAFE.BUS
{
    public class ProductBUS
    {
        public static DataTable GetAllProducts() =>
            DataProvider.Instance.ExecuteQuery("SELECT p.*, c.CategoryName FROM Product p JOIN Category c ON p.CategoryID = c.CategoryID");
    }
}