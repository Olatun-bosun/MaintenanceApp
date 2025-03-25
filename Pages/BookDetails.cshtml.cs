using MaintenanceLog.Pages.Admin.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages
{
    public class BookDetailsModel : PageModel
    {
        public UtilityInfo utilityInfo = new UtilityInfo();

        public void OnGet(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/");
                return;
            }

            try
            {
                string connectionString = "Data Source=OLATUNBOSUN\\OLATUNBOSUN;Initial Catalog=MaintenanceLogDb;User ID=sa;Password=2004Bos16..;Encrypt=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM utilities WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                utilityInfo.Id = reader.GetInt32(0);
                                utilityInfo.Name = reader.GetString(1);
                                utilityInfo.Class = reader.GetString(2);
                                utilityInfo.Description = reader.GetString(3);
                                utilityInfo.ImageFileName = reader.GetString(4);
                                utilityInfo.CreatedAt = reader.GetDateTime(5).ToString("MM/dd/yyyy");

                            }
                            else
                            {
                                Response.Redirect("/");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/");
                return;
            }
        }
    }
}
