using MaintenanceLog.MyHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages
{

    public class CalebEmailAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string email && email.Contains("@calebuniversity", StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The email address must be a school email address");
        }
    }
    [RequireNoAuth]
    [BindProperties]
    public class IndexModel : PageModel
    {
        [Required(ErrorMessage = "The Email is required")]
        [CalebEmailAddress(ErrorMessage = "The email address must end with @caleb.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}

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

            // connect to database and check the user credentials
            try
            {
                string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=MaintenanceLogDb;User ID=Arise;Password=2004Bos16..;Encrypt=False";
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
                                string hashedPassword = reader.GetString(7);
                                string role = reader.GetString(8);
                                string created_at = reader.GetDateTime(9).ToString("MM/dd/yyyy");
                                // verify the password
                                var passwordHasher = new PasswordHasher<IdentityUser>();
                                var result = passwordHasher.VerifyHashedPassword(new IdentityUser(),
                                    hashedPassword, Password);

                                if (result == PasswordVerificationResult.Success
                                    || result == PasswordVerificationResult.SuccessRehashNeeded)
                                {
                                    // successful password verification => initialize the session
                                    HttpContext.Session.SetInt32("id", id);
                                    HttpContext.Session.SetString("firstname", firstname);
                                    HttpContext.Session.SetString("lastname", lastname);
                                    HttpContext.Session.SetString("matricno", matricno);
                                    HttpContext.Session.SetString("email", email);
                                    HttpContext.Session.SetString("phone", phone);
                                    HttpContext.Session.SetString("hall", hall);
                                    HttpContext.Session.SetString("role", role);
                                    HttpContext.Session.SetString("created_at", created_at);

                                    // the user is authenticated successfully => redirect to the home page
                                    Response.Redirect("/Home");
                                }
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

            // Wrong Email or Password
            errorMessage = "Wrong Email or Password";
        }
    }
}
