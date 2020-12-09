using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Login_Session.Models
{
    public class User
    {
        public int Id { get; set;}


        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set;}

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

    }

    public class RegUser
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

    }

    public class AddUser
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

    }
}
