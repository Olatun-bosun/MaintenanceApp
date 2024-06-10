using MaintenanceLog.MyHelpers;
using MaintenanceLog.Pages.Admin.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages.Client.Orders
{
    //[RequireAuth(RequiredRole = "client")]
    public class IndexModel : PageModel
    {
        public List<ComplainInfo> listComplains = new List<ComplainInfo>();

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 3; // orders per page

        public void OnGet()
        {
            int clientId = HttpContext.Session.GetInt32("id") ?? 0;

            try
            {
                string requestPage = Request.Query["page"];
                page = int.Parse(requestPage);
            }
            catch (Exception ex)
            {
                page = 1;
            }

            try
            {
                string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=MaintenanceLogDb;User ID=Arise;Password=2004Bos16..;Encrypt=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM complains WHERE client_id=@client_id";
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@client_id", clientId);

                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM complains WHERE client_id=@client_id ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@client_id", clientId);
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ComplainInfo complainInfo = new ComplainInfo();
                                complainInfo.id = reader.GetInt32(0);
                                complainInfo.clientId = reader.GetInt32(1);
                                complainInfo.utilities = reader.GetString(2);
                                complainInfo.room = reader.GetString(3);
                                complainInfo.description = reader.GetString(4);
                                complainInfo.Status = reader.GetString(5);
                                complainInfo.CreatedAt = reader.GetDateTime(6);



                                listComplains.Add(complainInfo);
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
}
