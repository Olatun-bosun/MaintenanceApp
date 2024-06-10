using MaintenanceLog.MyHelpers;
using MaintenanceLog.Pages.Admin.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages.Client.Orders
{
    [RequireAuth(RequiredRole = "client")]

    public class CreateModel : PageModel

    {


        public List<UtilityInfo> listUtilitys = new List<UtilityInfo>();

        [BindProperty]
        [Required(ErrorMessage = "The Utility is required")]
        public string Utility { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The Room is required")]
        public string Room { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Message is required")]
        [MinLength(5, ErrorMessage = "The Message should be at least 5 characters")]
        [MaxLength(1024, ErrorMessage = "The Message should be less than 1024 characters")]
        public string Description { get; set; } = "";

        public string Status = "PENDING";

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=MaintenanceLogDb;User ID=Arise;Password=2004Bos16..;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM utilities";





                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {


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

        public void OnPost()
        {

            int client_id = HttpContext.Session.GetInt32("id") ?? 0;

            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }
            try
            {
                string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=MaintenanceLogDb;User ID=Arise;Password=2004Bos16..;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO complains " +
                      "(client_id, utilities, room, description, status) VALUES " +
                      "(@client_id, @utilities, @room, @description, @status);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@client_id", client_id);

                        command.Parameters.AddWithValue("@utilities", Utility);
                        command.Parameters.AddWithValue("@room", Room);
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@status", Status);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            successMessage = "Data saved correctly";
            Response.Redirect("/Client/Orders/Index");
        }
    }
}
