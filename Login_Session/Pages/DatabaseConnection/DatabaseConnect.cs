﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Login_Session.Pages.DatabaseConnection
{
    public class DatabaseConnect
    {
        public string DatabaseString ()
        {
            string DbString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\charl\source\repos\Week-10-PdfExample-Solution\Login_Session\Data\Login_Session.mdf;Integrated Security=True;Connect Timeout=30";
            return DbString;
        }
    }
}
