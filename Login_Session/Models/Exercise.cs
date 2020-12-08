using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Login_Session.Models
{
    public class Exercise
    {
        public int Id { get; set; }


        [Display(Name = "Exercise Name")]
        public string ExerciseName { get; set; }

        [Required]
        [Display(Name = "Number/Time of Reps")]
        public string RepNoTime { get; set; }

        [Required]
        [Display(Name = "Number of Sets")]
        public string SetNo { get; set; }

        [Display(Name = "Description of Exercise")]
        public string ExerciseDescription { get; set; }

        [Display(Name = "Area of Exercise")]
        public string ExerciseArea { get; set; }

    }
}
