using MaintenanceLog.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MaintenanceLog.Pages.Admin.Books
{
    [RequireAuth(RequiredRole = "admin")]
    public class EditModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "The Name is required")]
        public string Name { get; set; } = "";

        [BindProperty, Required]
        public string Class { get; set; } = "";

        [BindProperty]
        [MaxLength(1000, ErrorMessage = "The Description cannot exceed 1000 characters")]
        public string? Description { get; set; } = "";

        [BindProperty]
        public string ImageFileName { get; set; } = "";

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public EditModel (IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }

        
        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try
            {
                string connectionString = "Data Source=OLATUNBOSUN\\OLATUNBOSUN;Initial Catalog=MaintenanceLogDb;User ID=sa;Password=2004Bos16..;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM utilities WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Id = reader.GetInt32(0);
                                Name = reader.GetString(1);
                              
                                Class = reader.GetString(2);
                                Description = reader.GetString(3);
                                ImageFileName = reader.GetString(4);
                            }
                            else
                            {
                                Response.Redirect("/Admin/Books/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Books/Index");
            }
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

            // if we have a new ImageFile => upload the new image and delete the old image
            string newFileName = ImageFileName;
            if (ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(ImageFile.FileName);

                string imageFolder = webHostEnvironment.WebRootPath + "/images/books/";
                string imageFullPath = Path.Combine(imageFolder, newFileName);
                Console.WriteLine("New image (Edit): " + imageFullPath);

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    ImageFile.CopyTo(stream);
                }

                // delete old image
                string oldImageFullPath = Path.Combine(imageFolder, ImageFileName);
                System.IO.File.Delete(oldImageFullPath);
                Console.WriteLine("Delete Image " + oldImageFullPath);
            }

            // update the book data in the database
            try
            {
                string connectionString = "Data Source=OLATUNBOSUN\\OLATUNBOSUN;Initial Catalog=MaintenanceLogDb;User ID=sa;Password=2004Bos16..;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE utilities SET name=@name, class=@class, " +
                        "description=@description, image_filename=@image_filename WHERE id=@id;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", Name);
                       
                        command.Parameters.AddWithValue("@class", Class);
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@image_filename", newFileName);
                        command.Parameters.AddWithValue("@id", Id);

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
