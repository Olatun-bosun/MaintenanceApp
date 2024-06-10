using MaintenanceLog.MyHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages
{
    [RequireAuth]
    [BindProperties]
    public class ProfileModel : PageModel
    {
        [Required(ErrorMessage = "The First Name is required")]
        public string Firstname { get; set; } = "";

        [Required(ErrorMessage = "The Last Name is required")]
        public string Lastname { get; set; } = "";

        [Required(ErrorMessage = "The Matric Number is required")]
        public string MatricNo { get; set; } = "";

        [Required(ErrorMessage = "The Email is required"), EmailAddress]
        public string Email { get; set; } = "";

        public string? Phone { get; set; } = "";

        [Required(ErrorMessage = "The Hall is required")]
        public string Hall { get; set; } = "";


        public string? Password { get; set; } = "";
        public string? ConfirmPassword { get; set; } = "";



        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            Firstname = HttpContext.Session.GetString("firstname") ?? "";
            Lastname = HttpContext.Session.GetString("lastname") ?? "";
            MatricNo = HttpContext.Session.GetString("matricno") ?? "";

            Email = HttpContext.Session.GetString("email") ?? "";
            Phone = HttpContext.Session.GetString("phone");
            Hall = HttpContext.Session.GetString("hall") ?? "";
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            // successful data validation
            if (Phone == null) Phone = "";


            // update the user profile or the password
            string submitButton = Request.Form["action"];

            string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=MaintenanceLogDb;User ID=Arise;Password=2004Bos16..;Encrypt=False";

            if (submitButton.Equals("profile"))
            {
                // update the user profile in the database
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE users SET firstname=@firstname, lastname=@lastname, matricno=@matricno " +
                            "email=@email, phone=@phone, hall=@hall WHERE id=@id";

                        int? id = HttpContext.Session.GetInt32("id");
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@firstname", Firstname);
                            command.Parameters.AddWithValue("@lastname", Lastname);
                            command.Parameters.AddWithValue("@matricno", MatricNo);
                            command.Parameters.AddWithValue("@email", Email);

                            command.Parameters.AddWithValue("@phone", Phone);
                            command.Parameters.AddWithValue("@hall", Hall);
                            command.Parameters.AddWithValue("@id", id);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }

                // update the session data
                HttpContext.Session.SetString("firstname", Firstname);
                HttpContext.Session.SetString("lastname", Lastname);
                HttpContext.Session.SetString("matricno", MatricNo);
                HttpContext.Session.SetString("email", Email);
                HttpContext.Session.SetString("phone", Phone);
                HttpContext.Session.SetString("hall", Hall);

                successMessage = "Profile updated correctly";
            }
            else if (submitButton.Equals("password"))
            {
                // validate Password and ConfirmPassword
                if (Password == null || Password.Length < 5 || Password.Length > 50)
                {
                    errorMessage = "Password length should be between 5 and 50 characters";
                    return;
                }

                if (ConfirmPassword == null || !ConfirmPassword.Equals(Password))
                {
                    errorMessage = "Password and Confirm Password do not match";
                    return;
                }

                // update the password in the database
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE users SET password=@password WHERE id=@id";

                        int? id = HttpContext.Session.GetInt32("id");

                        var passwordHasher = new PasswordHasher<IdentityUser>();
                        string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@password", hashedPassword);
                            command.Parameters.AddWithValue("@id", id);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }

                successMessage = "Password updated correctly";
            }

            
        }
    }
}
