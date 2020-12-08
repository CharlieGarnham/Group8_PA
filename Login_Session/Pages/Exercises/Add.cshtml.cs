using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Login_Session.Models;
using Login_Session.Pages.DatabaseConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Login_Session.Pages.Exercises
{
    public class AddModel : PageModel
    {
        [BindProperty]
        public Exercise Exercise { get; set; }

        public List<string> EArea { get; set; } = new List<string> { "Whole Body", "Arm", "Leg", "Back", "Core", "Cardio" };
        public string UserName;
        public const string SessionKeyName1 = "username";


        public string FirstName;
        public const string SessionKeyName2 = "fname";

        public string SessionID;
        public const string SessionKeyName3 = "sessionID";


        public IActionResult OnGet()
        {
            UserName = HttpContext.Session.GetString(SessionKeyName1);
            FirstName = HttpContext.Session.GetString(SessionKeyName2);
            SessionID = HttpContext.Session.GetString(SessionKeyName3);

            if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(SessionID))
            {
                return RedirectToPage("/Login/Login");
            }
            return Page();


        }

        public IActionResult OnPost()
        {
            DatabaseConnect dbstring = new DatabaseConnect(); //creating an object from the class
            string DbConnection = dbstring.DatabaseString(); //calling the method from the class
            Console.WriteLine(DbConnection);
            SqlConnection conn = new SqlConnection(DbConnection);
            conn.Open();

            Console.WriteLine(Exercise.ExerciseName);
            Console.WriteLine(Exercise.RepNoTime);
            Console.WriteLine(Exercise.SetNo);
            Console.WriteLine(Exercise.ExerciseDescription);
            Console.WriteLine(Exercise.ExerciseArea);

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandText = @"INSERT INTO AllExercises (ExerciseName, RepNo/Time, SetNo, ExerciseDescription, ExerciseArea) VALUES (@EName, @RepNT, @SNum, @Desc, @EArea)";
                                                                                                                                                              
                command.Parameters.AddWithValue("@EName", Exercise.ExerciseName);                                                                                 
                command.Parameters.AddWithValue("@RepNT", Exercise.RepNoTime);
                command.Parameters.AddWithValue("@SNum", Exercise.SetNo);
                command.Parameters.AddWithValue("@Desc", Exercise.ExerciseDescription);
                command.Parameters.AddWithValue("@EArea", Exercise.ExerciseArea);
                command.ExecuteNonQuery();                      
            }

            return RedirectToPage("/Index");
        }
    }
}
