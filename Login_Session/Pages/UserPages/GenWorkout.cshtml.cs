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

namespace Login_Session.Pages.UserPages
{
    public class GenWorkoutModel : PageModel
    {
        [BindProperty]
        public List<Exercise> Exercise { get; set; }
        [BindProperty]
        public List<ChooseExercise> ExerciseChoice { get; set; }

        

        public string UserName;
        public const string SessionKeyName1 = "username";


        public string FirstName;
        public const string SessionKeyName2 = "fname";

        public string SessionID;
        public const string SessionKeyName3 = "sessionID";

        IWebHostEnvironment _env;

        public GenWorkoutModel(IWebHostEnvironment env)
        {
            _env = env;
        }


        public IActionResult OnGet(string pdf, int? WIdentity)
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

            ExerciseChoice = new List<ChooseExercise>();
            ChooseExercise Choice = new ChooseExercise();

            using (SqlCommand Ecommand = new SqlCommand())
            {
                
                Ecommand.Connection = conn;
                Ecommand.CommandText = @"SELECT * FROM Workout WHERE Id=@WIdentity";
                Ecommand.Parameters.AddWithValue("@WIdentity", WIdentity);
                var ExerciseCheck = Ecommand.ExecuteReader();

                while (ExerciseCheck.Read())
                {
                    Choice.WholeBody = ExerciseCheck.GetString(1);
                    Choice.Arm = ExerciseCheck.GetString(2);
                    Choice.Leg = ExerciseCheck.GetString(3);
                    Choice.Back = ExerciseCheck.GetString(4);
                    Choice.Core = ExerciseCheck.GetString(5);
                    Choice.Cardio = ExerciseCheck.GetString(6);
                    ExerciseChoice.Add(Choice);
                }
            }

            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = conn;
                command.CommandText = @"SELECT * FROM AllExercises WHERE ";
                int ExerciseCounter = (0);
                if (Choice.WholeBody == "true")                                                           
                {                                                                                                                                                  
                    command.CommandText = command.CommandText + "ExerciseArea ='Whole body'";
                    ExerciseCounter++;
                }
                if(Choice.Arm == "true")
                {

                    if (ExerciseCounter > 0)
                    {
                        command.CommandText = command.CommandText + " OR ";
                    }
                    command.CommandText = command.CommandText + "ExerciseArea ='Arm'";
                    ExerciseCounter++;
                }
                if (Choice.Leg == "true")
                {
                    if (ExerciseCounter > 0)
                    {
                        command.CommandText = command.CommandText + " OR ";
                    }
                    command.CommandText = command.CommandText + "ExerciseArea ='Leg'";
                    ExerciseCounter++;
                }
                if (Choice.Back == "true")
                {
                    if (ExerciseCounter > 0)
                    {
                        command.CommandText = command.CommandText + " OR ";
                    }
                    command.CommandText = command.CommandText + "ExerciseArea ='Back'";
                    ExerciseCounter++;
                }
                if (Choice.Core == "true")
                {
                    if (ExerciseCounter > 0)
                    {
                        command.CommandText = command.CommandText + " OR ";
                    }
                    command.CommandText = command.CommandText + "ExerciseArea ='Core'";
                    ExerciseCounter++;
                }
                if (Choice.Cardio == "true")
                {
                    if (ExerciseCounter > 0)
                    {
                        command.CommandText = command.CommandText + " OR ";
                    }
                    command.CommandText = command.CommandText + "ExerciseArea ='Cardio'";
                    ExerciseCounter++;
                }
                if (ExerciseCounter==0)
                {
                    //NoExercisesSelected();
                    return RedirectToPage("/UserPages/ChooseWorkout");
                }

                var reader = command.ExecuteReader();

                Exercise = new List<Exercise>();
                while (reader.Read())
                {
                    Exercise Row = new Exercise(); //each record found from the table
                    Row.ExerciseName = reader.GetString(1);
                    Row.RepNoTime = reader.GetString(2);
                    Row.SetNo = reader.GetString(3);
                    Row.ExerciseDescription = reader.GetString(4);
                    Row.ExerciseArea = reader.GetString(5); //ExerciseImage not yet included
                    Exercise.Add(Row);
                }

            }

            //PDF code here!
            if (pdf == "1")
            {
                //Create an object for pdf document
                Document doc = new Document();
                Section sec = doc.AddSection();
                Paragraph para = sec.AddParagraph();

                para.Format.Font.Name = "Arial";
                para.Format.Font.Size = 14;
                para.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100); //black colour
                para.AddFormattedText("Your Workout", TextFormat.Bold);
                para.Format.SpaceAfter = "1.0cm";

                //Adding picture
                ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();
                Paragraph para2 = sec.AddParagraph();
                var picpath = Path.Combine(_env.WebRootPath, "Files", "ExerciseImage.png");
                var image = para2.AddImage(ImageSource.FromFile(picpath));
                image.Width = Unit.FromCentimeter(4);
                para2.Format.SpaceAfter = Unit.FromCentimeter(2);

                //Table
                Table tab = new Table();
                tab.Borders.Width = 0.75;
                tab.TopPadding = 5;
                tab.BottomPadding = 5;

                //Column
                Column col = tab.AddColumn(Unit.FromCentimeter(2));
                col.Format.Alignment = ParagraphAlignment.Justify;
                tab.AddColumn(Unit.FromCentimeter(3));
                tab.AddColumn(Unit.FromCentimeter(3));
                tab.AddColumn(Unit.FromCentimeter(5));
                tab.AddColumn(Unit.FromCentimeter(3));

                //Row
                Row row = tab.AddRow();
                row.Shading.Color = Colors.Coral;

                //Cell for header
                Cell cell = new Cell();
                cell = row.Cells[0];
                cell.AddParagraph("Exercise Name");
                cell = row.Cells[1];
                cell.AddParagraph("Number/Time of Reps");
                cell = row.Cells[2];
                cell.AddParagraph("Number of Sets");
                cell = row.Cells[3];
                cell.AddParagraph("Exercise Description");
                cell = row.Cells[4];
                cell.AddParagraph("Exercise Area");


                //Add data to table 
                for (int i = 0; i < Exercise.Count; i++)
                {
                    row = tab.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph(Exercise[i].ExerciseName);
                    cell = row.Cells[1];
                    cell.AddParagraph(Exercise[i].RepNoTime);
                    cell = row.Cells[2];
                    cell.AddParagraph(Exercise[i].SetNo);
                    cell = row.Cells[3];
                    cell.AddParagraph(Exercise[i].ExerciseDescription);
                    cell = row.Cells[4];
                    cell.AddParagraph(Exercise[i].ExerciseArea);
                }

                tab.SetEdge(0, 0, 4, (Exercise.Count + 1), Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
                sec.Add(tab);

                //Rendering
                PdfDocumentRenderer pdfRen = new PdfDocumentRenderer();
                pdfRen.Document = doc;
                pdfRen.RenderDocument();

                //Create a memory stream
                MemoryStream stream = new MemoryStream();
                pdfRen.PdfDocument.Save(stream); //saving the file into the stream

                Response.Headers.Add("content-disposition", new[] { "inline; filename = ListofExercises.pdf" });
                return File(stream, "application/pdf");

            }

            return Page();

        }
    }
}
