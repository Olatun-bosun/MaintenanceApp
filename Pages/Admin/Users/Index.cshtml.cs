using MaintenanceLog.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages.Admin.Users
{
    [RequireAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<UserInfo> listUsers = new List<UserInfo>();

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // users per page

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

                    // find the number of users
                    string sqlCount = "SELECT COUNT(*) FROM users";
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }

                    string sql = "SELECT * FROM users ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo userInfo = new UserInfo();

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

                                listUsers.Add(userInfo);
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

    public class UserInfo
    {
        public int id;
        public string firstname;
        public string lastname;
        public string matricno;
        public string email;
        public string phone;
        public string hall;
        public string password;
        public string role;
        public string createdAt;
    }
}
