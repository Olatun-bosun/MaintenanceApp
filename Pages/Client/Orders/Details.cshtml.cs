using MaintenanceLog.MyHelpers;
using MaintenanceLog.Pages.Admin.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages.Client.Orders
{
    [RequireAuth(RequiredRole = "client")]
    public class DetailsModel : PageModel
    {
        public ComplainInfo complainInfo = new ComplainInfo();

        public void OnGet(int id)
        {
            int clientId = HttpContext.Session.GetInt32("id") ?? 0;

            if (id < 1)
            {
                Response.Redirect("/Client/Orders/Index");
                return;
            }


            try
            {
                string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=MaintenanceLogDb;User ID=Arise;Password=2004Bos16..;Encrypt=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    string sql = "SELECT * FROM complains WHERE id=@id AND client_id=@client_id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@client_id", clientId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                complainInfo.id = reader.GetInt32(0);
                                complainInfo.clientId = reader.GetInt32(1);
                                complainInfo.utilities = reader.GetString(2);
                                complainInfo.room = reader.GetString(3);
                                complainInfo.description = reader.GetString(4);
                                complainInfo.Status = reader.GetString(5);
                                complainInfo.CreatedAt = reader.GetDateTime(6);


                            }
                            else
                            {
                                //Response.Redirect("/Client/Orders/Index");
                                return;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Client/Orders/Index");
            }
        }
    }
}
