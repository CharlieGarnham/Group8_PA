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

namespace Login_Session.Pages.Login
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegUser User { get; set; }

        public string Role = ("User");
        public string UserName;
        
        
        
        public IActionResult OnGet()
        {
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

            Console.WriteLine(User.FirstName);
            Console.WriteLine(User.UserName);
            Console.WriteLine(User.Password);
            Console.WriteLine(Role);

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = conn;
                command.CommandText = @"INSERT INTO UserTable (FirstName, UserName, UserPassword, UserRole) VALUES (@FName, @UName, @Pwd, @Role)";

                command.Parameters.AddWithValue("@FName", User.FirstName);
                command.Parameters.AddWithValue("@UName", User.UserName);
                command.Parameters.AddWithValue("@Pwd", User.Password);
                command.Parameters.AddWithValue("@Role", Role);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Login/Login");




        }


    }
}