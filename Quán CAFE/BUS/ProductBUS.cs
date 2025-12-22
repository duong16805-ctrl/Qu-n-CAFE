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

        // Thêm hàm Insert 
        public static bool InsertProduct(string id, string name, int price, int catID)
        {
            string query = "INSERT INTO Product (ProductID, ProductName, Price, CategoryID) VALUES ( @id , @name , @price , @cat )";
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id, name, price, catID });
            return result > 0;
        }

        // Thêm hàm Delete 
        public static bool DeleteProduct(string id)
        {
            string query = "DELETE FROM Product WHERE ProductID = @id";
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });
            return result > 0;
        }
    }
}