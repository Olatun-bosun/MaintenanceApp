using MaintenanceLog.Pages.Admin.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages
{
    //[BindProperties(SupportsGet = true)]
    public class BooksModel : PageModel
    {
        public string? Search { get; set; }
        //public string PriceRange { get; set; } = "any";
        //public string PageRange { get; set; } = "any";
        public string Class { get; set; } = "any";

        public List<UtilityInfo> listUtilitys = new List<UtilityInfo>();


        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // books per page

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }

            try
            {
                string connectionString = "Data Source=OLATUNBOSUN\\OLATUNBOSUN;Initial Catalog=MaintenanceLogDb;User ID=sa;Password=2004Bos16..;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM utilities";
                    sqlCount += " WHERE (name LIKE @search )";

                    //if (PriceRange.Equals("0_50"))
                    //{
                    //    sqlCount += " AND price <= 50";
                    //}
                    //else if (PriceRange.Equals("50_100"))
                    //{
                    //    sqlCount += " AND price >= 50 AND price <= 100";
                    //}
                    //else if (PriceRange.Equals("above100"))
                    //{
                    //    sqlCount += " AND price >= 100";
                    //}


                    //if (PageRange.Equals("0_100"))
                    //{
                    //    sqlCount += " AND num_pages <= 100";
                    //}
                    //else if (PageRange.Equals("100_299"))
                    //{
                    //    sqlCount += " AND num_pages >= 100 AND num_pages <= 299";
                    //}
                    //else if (PageRange.Equals("above300"))
                    //{
                    //    sqlCount += " AND num_pages >= 300";
                    //}


                    if (!Class.Equals("any"))
                    {
                        sqlCount += " AND class=@class";
                    }

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        command.Parameters.AddWithValue("@class", Class);

                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }



                    string sql = "SELECT * FROM utilities";
                    sql += " WHERE (name LIKE @search)";

                    //if (PriceRange.Equals("0_50"))
                    //{
                    //    sql += " AND price <= 50";
                    //}
                    //else if (PriceRange.Equals("50_100"))
                    //{
                    //    sql += " AND price >= 50 AND price <= 100";
                    //}
                    //else if (PriceRange.Equals("above100"))
                    //{
                    //    sql += " AND price >= 100";
                    //}


                    //if (PageRange.Equals("0_100"))
                    //{
                    //    sql += " AND num_pages <= 100";
                    //}
                    //else if (PageRange.Equals("100_299"))
                    //{
                    //    sql += " AND num_pages >= 100 AND num_pages <= 299";
                    //}
                    //else if (PageRange.Equals("above300"))
                    //{
                    //    sql += " AND num_pages >= 300";
                    //}


                    if (!Class.Equals("any"))
                    {
                        sql += " AND class=@class";
                    }

                    sql += " ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + Search + "%");
                        command.Parameters.AddWithValue("@class", Class);
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UtilityInfo utilityInfo = new UtilityInfo();
                                utilityInfo.Id = reader.GetInt32(0);
                                utilityInfo.Name = reader.GetString(1);
                                utilityInfo.Class = reader.GetString(2);
                                utilityInfo.Description = reader.GetString(3);
                                utilityInfo.ImageFileName = reader.GetString(4);
                                utilityInfo.CreatedAt = reader.GetDateTime(5).ToString("MM/dd/yyyy");

                                listUtilitys.Add(utilityInfo);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
