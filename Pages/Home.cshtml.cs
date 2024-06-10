using MaintenanceLog.Pages.Admin.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages
{
    public class HomeModel : PageModel
    {
        private readonly string connectionString;
        public HomeModel(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }
        public List<UtilityInfo> UtilityBooks = new List<UtilityInfo>();
        //public List<UtilityInfo> listTopSales = new List<BookInfo>();
        //public void OnGet()
        //{

        //    try
        //    {
        //        //string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=BestShop;User ID=Arise;Password=2004Bos16..";


        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            string sql = "SELECT TOP 4 * FROM books ORDER BY created_at DESC";
        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                       UtilityInfo bookInfo = new BookInfo();
        //                        bookInfo.Id = reader.GetInt32(0);
        //                        bookInfo.Title = reader.GetString(1);
        //                        bookInfo.Authors = reader.GetString(2);
        //                        bookInfo.Isbn = reader.GetString(3);
        //                      bookInfo.ImageFileName = reader.GetString(8);
        //                        bookInfo.CreatedAt = reader.GetDateTime(9).ToString("MM/dd/yyyy");

        //                        listNewestBooks.Add(bookInfo);
        //                    }
        //                }
        //            }
        //        }
        //        }
    }

    //public class UtilityInfo
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = "";
    //    public string Class { get; set; } = "";
    //    public string Description { get; set; } = "";
    //    public string CreatedAt { get; set; } = "";
    //}
}
