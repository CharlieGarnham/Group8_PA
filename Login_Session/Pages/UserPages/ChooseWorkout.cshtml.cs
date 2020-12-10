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

namespace Login_Session.Pages.UserPages
{
    public class ChooseWorkoutModel : PageModel
    {
        [BindProperty]
        public ChooseExercise Exercise { get; set; }

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            DatabaseConnect dbstring = new DatabaseConnect(); //creating an object from the class
            string DbConnection = dbstring.DatabaseString(); //calling the method from the class
            Console.WriteLine(DbConnection);
            SqlConnection conn = new SqlConnection(DbConnection);
            conn.Open();

            Console.WriteLine(Exercise.WholeBody);
            Console.WriteLine(Exercise.Arm);
            Console.WriteLine(Exercise.Leg);
            Console.WriteLine(Exercise.Back);
            Console.WriteLine(Exercise.Core);
            Console.WriteLine(Exercise.Cardio);

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandText = @"INSERT INTO Workout (Whole Body, Arm, Leg, Back, Core, Cardio) VALUES (@WBody, @Arm, @Leg, @Back, @Core, @Cardio)";
                                                                                                                                                              
                command.Parameters.AddWithValue("@WBody", Exercise.WholeBody);                                                                                 
                command.Parameters.AddWithValue("@Arm", Exercise.Arm);
                command.Parameters.AddWithValue("@Leg", Exercise.Leg);
                command.Parameters.AddWithValue("@Back", Exercise.Back);
                command.Parameters.AddWithValue("@Core", Exercise.Core);
                command.Parameters.AddWithValue("@Cardio", Exercise.Cardio);
                command.ExecuteNonQuery();                      
            }

            return RedirectToPage("/UserPages/GenWorkout");
        }
    }
}