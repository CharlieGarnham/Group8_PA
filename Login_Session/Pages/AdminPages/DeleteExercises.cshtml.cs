using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Login_Session.Models;
using Login_Session.Pages.DatabaseConnection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;
using PdfSharpCore.Utils;
using SixLabors.ImageSharp.PixelFormats;

namespace Login_Session.Pages.AdminPages
{
    public class DeleteExercisesModel : PageModel
    {
        [BindProperty]
        public List<Exercise> Exercise { get; set; }
        [BindProperty]
        public List<bool> IsSelect { get; set; } //this is needed to allow the user to select the checkbox in the form of html page
        public List<Exercise> ExerciseToDelete { get; set; } //this variable is a list to collect the selected modules to be deleted


        public List<string> EArea { get; set; } = new List<string> { "Whole Body", "Arm", "Leg", "Back", "Core", "Cardio" };

        public string UserName;
        public const string SessionKeyName1 = "username";


        public string FirstName;
        public const string SessionKeyName2 = "fname";

        public string SessionID;
        public const string SessionKeyName3 = "sessionID";

        IWebHostEnvironment _env;

        public DeleteExercisesModel(IWebHostEnvironment env)
        {
            _env = env;
        }


        public IActionResult OnGet()
        {
            //get the session first!
            UserName = HttpContext.Session.GetString(SessionKeyName1);
            FirstName = HttpContext.Session.GetString(SessionKeyName2);
            SessionID = HttpContext.Session.GetString(SessionKeyName3);

            if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(SessionID))
            {
                return RedirectToPage("/Login/Login");
            }


            DatabaseConnect dbstring = new DatabaseConnect(); //creating an object from the class
            string DbConnection = dbstring.DatabaseString(); //calling the method from the class
            Console.WriteLine(DbConnection);
            SqlConnection conn = new SqlConnection(DbConnection);
            conn.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandText = @"SELECT * FROM AllExercises";

                var reader = command.ExecuteReader();

                Exercise = new List<Exercise>();
                IsSelect = new List<bool>();
                while (reader.Read())
                {
                    Exercise Row = new Exercise(); //each record found from the table
                    Row.Id = reader.GetInt32(0);
                    Row.ExerciseName = reader.GetString(1);
                    Row.RepNoTime = reader.GetString(2);
                    Row.SetNo = reader.GetString(3);
                    Row.ExerciseDescription = reader.GetString(4);
                    Row.ExerciseArea = reader.GetString(5);
                    Exercise.Add(Row);
                    IsSelect.Add(false);
                }

            }

            return Page();
        }
        public IActionResult OnPost()
        {
            ExerciseToDelete = new List<Exercise>();//create the object for Module to be deleted. This variable now an empty list
            for (int i = 0; i < Exercise.Count; i++) //Read all rows from Module. Each row has a checkbox!
            {
                if (IsSelect[i] == true) //if the checkbox of the row is true
                {
                    ExerciseToDelete.Add(Exercise[i]); //collect the item for the row
                }
            }

            Console.WriteLine("Exercises to be deleted : ");

            for (int i = 0; i < ExerciseToDelete.Count(); i++)
            {
                Console.WriteLine(ExerciseToDelete[i].Id); // we need to write the SQL statement to delete the module based on this Id
                                                       //We can have more fields 
            }

            DatabaseConnect dbstring = new DatabaseConnect();
            string DbConnection = dbstring.DatabaseString();
            SqlConnection conn = new SqlConnection(DbConnection);
            conn.Open();

            for (int i = 0; i < ExerciseToDelete.Count(); i++)
            {

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = @"DELETE FROM AllExercises WHERE Id = @ExerciseID";
                    command.Parameters.AddWithValue("@ExerciseID", ExerciseToDelete[i].Id);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/AdminPages/ViewExercises");


        }
    }

}