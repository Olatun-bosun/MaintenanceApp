using MaintenanceLog.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace MaintenanceLog.Pages.Admin.Books
{
    [RequireAuth(RequiredRole ="admin")]
    public class CreateModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The Name is required")]
        public string Name { get; set; } = "";

        [BindProperty, Required]
        public string Class { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Message is required")]
        [MinLength(5, ErrorMessage = "The Message should be at least 5 characters")]
        [MaxLength(1024, ErrorMessage = "The Message should be less than 1024 characters")]
        public string Description { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The Image File is required")]
        public IFormFile ImageFile { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public CreateModel (IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }

        
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

            if (Description == null) Description = "";

            // save the image file on the server
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(ImageFile.FileName);

            string imageFolder = webHostEnvironment.WebRootPath + "/images/books/";

            string imageFullPath = Path.Combine(imageFolder, newFileName);
            Console.WriteLine("New image: " + imageFullPath);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                ImageFile.CopyTo(stream);
            }

            // save the new book in the database
            try
            {
                string connectionString = "Data Source=LAPTOP-HTBOKT77;Initial Catalog=MaintenanceLogDb;User ID=Arise;Password=2004Bos16..;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO utilities " +
                      "(name, class, description, image_filename) VALUES " +
                      "(@name, @class, @description, @image_filename);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", Name);
                        command.Parameters.AddWithValue("@class", Class);
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@image_filename", newFileName);

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
            Response.Redirect("/Admin/Books/Index");
        }
    }
}
