using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quán_CAFE.BUS
{
    public class CategoryBUS
    {
        public static DataTable GetListCategory() => DataProvider.Instance.ExecuteQuery("EXEC USP_GetCategoryList");
    }
}