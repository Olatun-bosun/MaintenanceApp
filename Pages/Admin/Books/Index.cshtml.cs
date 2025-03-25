using MaintenanceLog.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages.Admin.Books
{
    [RequireAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<UtilityInfo> listUtilitys = new List<UtilityInfo>();
        public string search = "";

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // books per page

        public string column = "id";
        public string order = "desc";

        

        public void OnGet()
        {
            search = Request.Query["search"];
            if (search == null) search = "";

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

            string[] validColumns = { "id", "name",  "class", "created_at" };
            column = Request.Query["column"];
            if (column == null || !validColumns.Contains(column))
            {
                column = "id";
            }

            order = Request.Query["order"];
            if (order == null || !order.Equals("asc"))
            {
                order = "desc";
            }

            try
            {
                string connectionString = "Data Source=OLATUNBOSUN\\OLATUNBOSUN;Initial Catalog=MaintenanceLogDb;User ID=sa;Password=2004Bos16..;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM utilities";
                    if (search.Length > 0)
                    {
                        sqlCount += " WHERE name LIKE @search OR authors LIKE @search";
                    }

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");

                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM utilities";
                    if (search.Length > 0)
                    {
                        sql += " WHERE name LIKE @search OR authors LIKE @search";
                    }
                    sql += " ORDER BY " + column + " " + order; //" ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class UtilityInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
       
        public string Class { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageFileName { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
