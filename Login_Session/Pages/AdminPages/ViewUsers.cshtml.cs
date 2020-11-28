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
    public class ViewUsersModel : PageModel
    {
        [BindProperty]
        public List<User> User { get; set; }

        public List<string> URole { get; set; } = new List<string> { "User", "Admin" };

        public string UserName;
        public const string SessionKeyName1 = "username";


        public string FirstName;
        public const string SessionKeyName2 = "fname";

        public string SessionID;
        public const string SessionKeyName3 = "sessionID";

        IWebHostEnvironment _env;

        public ViewUsersModel (IWebHostEnvironment env)
        {
            _env = env;
        }


        public IActionResult OnGet(string pdf)
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
                command.CommandText = @"SELECT * FROM UserTable";

                var reader = command.ExecuteReader();

                User = new List<User>();
                while (reader.Read())
                {
                    User Row = new User(); //each record found from the table
                    Row.Id = reader.GetInt32(0);
                    Row.FirstName = reader.GetString(1);
                    Row.UserName = reader.GetString(2);
                    Row.Role = reader.GetString(4); // We dont get the password. The role field is in the 5th position
                    User.Add(Row);
                }
                
            }

            //PDF code here!
            if (pdf=="1")
            {
                //Create an object for pdf document
                Document doc = new Document();
                Section sec = doc.AddSection();
                Paragraph para = sec.AddParagraph();

                para.Format.Font.Name = "Arial";
                para.Format.Font.Size = 14;
                para.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100); //black colour
                para.AddFormattedText("List of Users", TextFormat.Bold);
                para.Format.SpaceAfter = "1.0cm";

                //Adding picture
                ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();
                Paragraph para2 = sec.AddParagraph();
                var picpath = Path.Combine(_env.WebRootPath, "Files", "Pic.png");
                var image = para2.AddImage(ImageSource.FromFile(picpath));
                image.Width = Unit.FromCentimeter(4);
                para2.Format.SpaceAfter = Unit.FromCentimeter(2);

                //Table
                Table tab = new Table();
                tab.Borders.Width = 0.75;
                tab.TopPadding = 5;
                tab.BottomPadding = 5;

                //Column
                Column col = tab.AddColumn(Unit.FromCentimeter(1.5));
                col.Format.Alignment = ParagraphAlignment.Justify;
                tab.AddColumn(Unit.FromCentimeter(3));
                tab.AddColumn(Unit.FromCentimeter(3));
                tab.AddColumn(Unit.FromCentimeter(2));

                //Row
                Row row = tab.AddRow();
                row.Shading.Color = Colors.Coral;

                //Cell for header
                Cell cell = new Cell();
                cell = row.Cells[0];
                cell.AddParagraph("No.");
                cell = row.Cells[1];
                cell.AddParagraph("First Name");
                cell = row.Cells[2];
                cell.AddParagraph("User Name");
                cell = row.Cells[3];
                cell.AddParagraph("Role");


                //Add data to table 
                for (int i=0; i<User.Count; i++)
                {
                    row = tab.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph(Convert.ToString(i+1));
                    cell = row.Cells[1];
                    cell.AddParagraph(User[i].FirstName);
                    cell = row.Cells[2];
                    cell.AddParagraph(User[i].UserName);
                    cell = row.Cells[3];
                    cell.AddParagraph(User[i].Role);
                }

                tab.SetEdge(0, 0, 4, (User.Count + 1), Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
                sec.Add(tab);

                //Rendering
                PdfDocumentRenderer pdfRen = new PdfDocumentRenderer();
                pdfRen.Document = doc;
                pdfRen.RenderDocument();

                //Create a memory stream
                MemoryStream stream = new MemoryStream();
                pdfRen.PdfDocument.Save(stream); //saving the file into the stream

                Response.Headers.Add("content-disposition", new[] { "inline; filename = ListofUser.pdf" });
                return File(stream, "application/pdf");

            }

            return Page();

        }
    }
}
