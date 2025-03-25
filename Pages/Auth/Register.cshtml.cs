using MaintenanceLog.MyHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages.Auth
{
    public class BabcockEmailAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string email && email.Contains("@babcockuniversity", StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The email address must be a school email address");
        }
    }

    [RequireNoAuth]
    [BindProperties]
    public class RegisterModel : PageModel
    {
        [Required(ErrorMessage = "The First Name is required")]
        public string Firstname { get; set; } = "";

        [Required(ErrorMessage = "The Last Name is required")]
        public string Lastname { get; set; } = "";

        [Required(ErrorMessage = "The Email is required")]
        [BabcockEmailAddress(ErrorMessage = "The email address must end with @babcock.")]
        public string Email { get; set; } = "";

        public string? Phone { get; set; } = "";

        [Required(ErrorMessage = "The Hall is required")]
        public string Hall { get; set; } = "";

        [Required(ErrorMessage = "The Matric No is required")]
        public string MatricNo { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]

        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = "";


        public string errorMessage = "";
        public string successMessage = "";



        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            // successfull data validation
            if (Phone == null) Phone = "";

            // add the user details to the database
            string connectionString = "Data Source=OLATUNBOSUN\\OLATUNBOSUN;Initial Catalog=MaintenanceLogDb;User ID=sa;Password=2004Bos16..;Encrypt=False";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO users " +
                    "(firstname, lastname,matricno, email, phone, hall, password, role) VALUES " +
                    "(@firstname, @lastname,@matricno, @email, @phone, @hall, @password, 'client');";

                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", Firstname);
                        command.Parameters.AddWithValue("@lastname", Lastname);
                        command.Parameters.AddWithValue("@matricno", MatricNo);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@hall", Hall);
                        command.Parameters.AddWithValue("@password", hashedPassword);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Email))
                {
                    errorMessage = "Email Address already used";
                }
                else
                {
                    errorMessage = ex.Message;
                }

                return;
            }

            // send confirmation email to the user
            string username = Firstname + " " + Lastname;
            string subject = "Account created successfully";
            string message = "Dear " + username + ",\n\n" +
                "Your account has been created successfully.\n\n" +
                "Best Regards";
            //EmailSender.SendEmail(Email, username, subject, message).Wait();

            // initialize the authenticated session => add the user details to the session data
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM users WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string firstname = reader.GetString(1);
                                string lastname = reader.GetString(2);
                                string matricno = reader.GetString(3);
                                string email = reader.GetString(4);
                                string phone = reader.GetString(5);
                                string hall = reader.GetString(6);
                                //string hashedPassword = reader.GetString(7);
                                string role = reader.GetString(8);
                                string created_at = reader.GetDateTime(9).ToString("MM/dd/yyyy");

                                HttpContext.Session.SetInt32("id", id);
                                HttpContext.Session.SetString("firstname", firstname);
                                HttpContext.Session.SetString("lastname", lastname);
                                HttpContext.Session.SetString("matricno", matricno);
                                HttpContext.Session.SetString("email", email);
                                HttpContext.Session.SetString("phone", phone);
                                HttpContext.Session.SetString("hall", hall);
                                HttpContext.Session.SetString("role", role);
                                HttpContext.Session.SetString("created_at", created_at);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }


            successMessage = "Account created successfully";

            // redirect to the home page
            Response.Redirect("/Home");
        }
    }
}
