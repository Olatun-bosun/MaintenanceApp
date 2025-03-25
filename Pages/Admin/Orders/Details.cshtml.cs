using MaintenanceLog.MyHelpers;
using MaintenanceLog.Pages.Admin.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages.Admin.Orders
{
    [RequireAuth(RequiredRole = "admin")]
    public class DetailsModel : PageModel
    {
        public ComplainInfo complainInfo = new ComplainInfo();
        public UserInfo userInfo = new UserInfo();

        public void OnGet(int id)
        {
            if (id < 1)
            {
                Response.Redirect("/Admin/Orders/Index");
                return;
            }

            string Status = Request.Query["status"];

            try
            {
                string connectionString = "Data Source=OLATUNBOSUN\\OLATUNBOSUN;Initial Catalog=MaintenanceLogDb;User ID=sa;Password=2004Bos16..;Encrypt=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //if (paymentStatus != null)
                    //{
                    //    string sqlUpdate = "UPDATE orders SET payment_status=@payment_status WHERE id=@id";
                    //    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    //    {
                    //        command.Parameters.AddWithValue("@payment_status", paymentStatus);
                    //        command.Parameters.AddWithValue("@id", id);

                    //        command.ExecuteNonQuery();
                    //    }
                    //}


                    if (Status != null)
                    {
                        string sqlUpdate = "UPDATE complains SET status=@status WHERE id=@id";
                        using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                        {
                            command.Parameters.AddWithValue("@status",Status);
                            command.Parameters.AddWithValue("@id", id);

                            command.ExecuteNonQuery();
                        }
                    }


                    string sql = "SELECT * FROM complains WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

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
                                //Response.Redirect("/Admin/Orders/Index");
                                return;
                            }
                        }
                    }

                     sql = "SELECT * FROM users WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", complainInfo.clientId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userInfo.id = reader.GetInt32(0);
                                userInfo.firstname = reader.GetString(1);
                                userInfo.lastname = reader.GetString(2);
                                userInfo.matricno = reader.GetString(3);
                                userInfo.email = reader.GetString(4);
                                userInfo.phone = reader.GetString(5);
                                userInfo.hall = reader.GetString(6);
                                userInfo.password = reader.GetString(7);
                                userInfo.role = reader.GetString(8);
                                userInfo.createdAt = reader.GetDateTime(9).ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                Console.WriteLine("Client not found, id=" + complainInfo.clientId);
                                //Response.Redirect("/Admin/Orders/Index");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Orders/Index");
            }
        }
    }
}
